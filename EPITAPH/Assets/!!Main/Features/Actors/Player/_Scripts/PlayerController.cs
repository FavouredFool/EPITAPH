using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask _hitLayer;
    [SerializeField] LayerMask _boltLayer;
    
    [Header("Hit")]
    [SerializeField, Range(1, 20)] int _maxHP;
    [field: SerializeField, Range(1, 6)] public float HitSpeedMultiplier { get; set; } = 2.5f;
    [field: SerializeField, Range(0.1f, 10)] public float BatTime { get; set; } = 2f;
    [field: SerializeField] public GameObject Visual3DMesh { get; set; }
    [field: SerializeField] public GameObject VFXObject { get; set; }
    [field: SerializeField] public Collider2D Collider { get; set; }
    [field: SerializeField, Range(0, 10)] public float KnockbackStrength { get; set; }
    
    [Header("Movement")]
    [SerializeField, Range(1, 20)] float _speed;
    [SerializeField, Range(1, 20)] float _speedAimReduction;
    [SerializeField, Range(1, 20)] float _speedReloadReduction;
    [SerializeField, Range(0, 0.95f)] float _moveLockThreshold = 0.3f;
    [SerializeField, Range(1, 20)] float _knockbackDecay;
    //[SerializeField] AimAssistV3 _aimAssist;
    
    [Header("Shooting")]
    [SerializeField, Range(0, 50)] float _knockbackStrength = 2;
    [SerializeField] Transform _instantiationParent;
    [SerializeField] BoltController _projectileBlueprint;
    [SerializeField, Range(0, 4)] float _spawnDist;
    [SerializeField] Transform _bloodlineConnection;
    [SerializeField, Range(0.1f, 4)] float _reloadTime;
    
    [Header("Cam")]
    [SerializeField] Transform _cameraFollow;
    [SerializeField, Range(0, 10)] float _cameraAimOffset;

    [Header("Audio")]
    [SerializeField] PlayerAudioData PADScriptableObject;

    [Header("Animation")]
    [SerializeField] Animator _characterAnimator;
    [SerializeField] Animator _crossbowAnimator;
    
    [Header("Lunge")]
    [field:SerializeField, Range(1, 100)] public float LungeSpeed { get; private set; }
    [field:SerializeField, Range(1, 200)] public float LungeAcceleration { get; private set; }
    [field: SerializeField, Range(1, 6)] public float LungePower { get; private set; } = 3;
    [field: SerializeField, Range(0, 1)] public float InitalDelay { get; private set; } = 0.1f;
    
    public float Speed => _speed;
    public float SpeedAimReduction => _speedAimReduction;
    
    public Animator CharacterAnimator => _characterAnimator;
    public Animator CrossboxAnimator => _crossbowAnimator;
    public float SpawnDist => _spawnDist;
    public Transform InstantiationParent => _instantiationParent;
    public Transform BloodlineConnection => _bloodlineConnection;

    public BoltController ProjectileBlueprint => _projectileBlueprint;

    public float ReloadTime => _reloadTime;
    
    InputActions _inputActions;
    public Rigidbody2D Rb { get; set; }

    // Input
    public Vector2 MovementInput { get; set; }
    public Vector2 RotateInput  { get; set; }

    // Dir
    public Vector2 LookDirection { get; set; }
    
    // HP
    // TODO gotta hook this up to the UI
    public int CurrentHP { get; set; }
    public Vector2 LastHitDir { get; set; }
    
    // Bolts
    public bool BoltInChamber { get; set; } = true;
    public Dictionary<BoltType, BoltController> CurrentBoltsHeld { get; private set; }
    
    bool IsAiming => RotateInput.magnitude > _moveLockThreshold && BoltInChamber;
    
    // Lunge
    // TODO i really dislike doing this but i dont know how else i can convey the info to the state
    public BoltController CurrentLungeBolt { get; set; }

    // State Machine
    public StateMachine StateMachine { get; set; }
    
    // State Triggers
    public TriggerPredicate ShootTrigger { get; private set; }
    public TriggerPredicate LungeTrigger { get; private set; }
    public TriggerPredicate FinishLungeTrigger { get; private set; }
    public TriggerPredicate StartReloadTrigger { get; private set; }
    public TriggerPredicate StopReloadTrigger { get; private set; }
    public TriggerPredicate GetHitTrigger { get; private set; }
    public TriggerPredicate FinishBatTrigger { get; private set; }
    
    // Hashes
    // Crossbow
    public static readonly int ShootCrossbowTriggerAnim = Animator.StringToHash("Shoot");
    public static readonly int StartReloadTriggerAnim = Animator.StringToHash("StartReload");
    public static readonly int InterruptReloadTriggerAnim = Animator.StringToHash("InterruptReload");
    public static readonly int FinishReloadTriggerAnim = Animator.StringToHash("FinishReload");
    
    // Player
    public static readonly int IsMovingBoolAnim = Animator.StringToHash("IsMoving");
    public static readonly int IsAimingBoolAnim = Animator.StringToHash("IsAiming");
    public static readonly int IsReloadingBoolAnim = Animator.StringToHash("IsReloading");
    public static readonly int IsLungingBoolAnim = Animator.StringToHash("IsLunging");
    public static readonly int ShotCharacterTriggerAnim = Animator.StringToHash("Shot");
    

    public Vector2 AimAssistedLookDirection
    {
        get
        {
            // TODO re-add aim assist
            Vector2 rawAimDir = RotateInput.normalized;
            return rawAimDir;
            
            //float angle = Mathf.Atan2(rawAimDir.y, rawAimDir.x) * Mathf.Rad2Deg - 90;
            //float assistedAngle = _aimAssist.GetAssistedAngle(angle, transform.position);
            //
            //return Quaternion.Euler(0, 0, assistedAngle) * Vector2.up;
        }
    }

    public Vector2 MovementVelocity { get; set; }
    public Vector2 KnockbackVelocity { get; set; }
    
    void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        Rb = GetComponent<Rigidbody2D>();

        CurrentBoltsHeld = new Dictionary<BoltType, BoltController>
        {
            [BoltType.DOWN] = null,
            [BoltType.LEFT] = null,
            [BoltType.UP] = null,
            [BoltType.RIGHT] = null
        };

        PADScriptableObject.Setup(this.gameObject);
        
        Assert.IsNotNull(_characterAnimator);
        Assert.IsNotNull(_crossbowAnimator);

        CurrentHP = _maxHP;

        InitStateMachine();
    }

    void InitStateMachine()
    {
        StateMachine = new StateMachine();

        VampireStateContext ctx = new(this, _inputActions);

        MoveState moveState = new(ctx);
        AimState aimState = new(ctx);
        ReloadState reloadState = new(ctx);
        ShootState shootState = new(ctx);
        LungeState lungeState = new(ctx);
        HitRecoveryState hitRecoveryState = new(ctx);

        ShootTrigger = new TriggerPredicate();
        LungeTrigger = new TriggerPredicate();
        FinishLungeTrigger = new TriggerPredicate();
        StartReloadTrigger = new TriggerPredicate();
        StopReloadTrigger = new TriggerPredicate();
        GetHitTrigger = new TriggerPredicate();
        FinishBatTrigger = new TriggerPredicate();
        
        At(moveState, aimState, new FuncStatePredicate(() => IsAiming));
        At(aimState, moveState, new FuncStatePredicate(() => !IsAiming));
        
        At(moveState, reloadState, StartReloadTrigger);
        At(aimState, reloadState, StartReloadTrigger);
        
        At(reloadState, moveState, StopReloadTrigger);
        
        At(aimState, shootState, ShootTrigger);
        At(shootState, aimState, new FuncStatePredicate(() => true));
        
        At(moveState, lungeState, LungeTrigger);
        At(aimState, lungeState, LungeTrigger);
        
        At(lungeState, moveState, FinishLungeTrigger);
        
        At(moveState, hitRecoveryState, GetHitTrigger);
        At(aimState, hitRecoveryState, GetHitTrigger);
        At(reloadState, hitRecoveryState, GetHitTrigger);

        At(hitRecoveryState, moveState, FinishBatTrigger);
        
        StateMachine.SetState(moveState);
    }
    
    void At(IState from, IState to, IStatePredicate condition) =>
        StateMachine.AddTransition(from, to, condition);
    
    void Any(IState to, IStatePredicate condition) =>
        StateMachine.AddAnyTransition(to, condition);
    

    void Update()
    {
        StateMachine.Update();
        //Debug.Log(StateMachine.CurrentState);
    }
    
    void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    public void ReadInput()
    {
        MovementInput = _inputActions.Player.Movement.ReadValue<Vector2>();
        RotateInput = _inputActions.Player.Look.ReadValue<Vector2>();
    }
    
    public void SetCameraFollow(bool setFar)
    {
        if (!setFar)
        {
            _cameraFollow.localPosition = Vector3.zero;
        }
        else
        {
            _cameraFollow.localPosition = Vector3.up * _cameraAimOffset;
        }
    }

    public void CalculateVelocity()
    {
        // movement handled by the states
        
        // knockback
        KnockbackVelocity *= Mathf.Exp(-_knockbackDecay * Time.deltaTime);
        
        // Combine
        Rb.linearVelocity = MovementVelocity + KnockbackVelocity;
    }
    

    public void Rotation()
    {
        // LookDirection set by State
        
        float angle = Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg - 90f;
        Rb.MoveRotation(Quaternion.Euler(0f, 0f, angle));
    }
    
    public void Knockback(Vector2 dir)
    {
        KnockbackVelocity += dir * _knockbackStrength;
    }
    
    public void ShootBoltInput(InputAction.CallbackContext ctx)
    {
        ShootBolt();
    }

    public void ReloadInputStart(InputAction.CallbackContext ctx)
    {
        StartReload();
    }

    public void LungeRightInput(InputAction.CallbackContext ctx)
    {
        LungeToBolt(BoltType.RIGHT);
    }
    
    public void LungeDownInput(InputAction.CallbackContext ctx)
    {
        LungeToBolt(BoltType.DOWN);
    }
    
    public void LungeLeftInput(InputAction.CallbackContext ctx)
    {
        LungeToBolt(BoltType.LEFT);
    }
    
    public void LungeUpInput(InputAction.CallbackContext ctx)
    {
        LungeToBolt(BoltType.UP);
    }
    
    public void LungeToBolt(BoltType boltType)
    {
        if (CurrentBoltsHeld[boltType] == null) return;
        
        BoltController boltToLunge = CurrentBoltsHeld[boltType];

        if (!boltToLunge.IsLungeable) return;
 
        CurrentLungeBolt = boltToLunge;
        LungeTrigger.Trigger();
    }
    
    public void ShootBolt()
    {
        BoltType type = GetBoltTypeToShoot();
        if (type == BoltType.NONE) return;

        ShootTrigger.Trigger();
    }
    
    public void StartReload()
    {
        if (BoltInChamber) return;
        if (CurrentBoltsHeld.Values.All(e => e != null)) return;
        
        StartReloadTrigger.Trigger();
    }
    

    public BoltType GetBoltTypeToShoot()
    {
        foreach (var kv in CurrentBoltsHeld.Where(kv => kv.Value == null))
        {
            return kv.Key;
        }

        return BoltType.NONE;
    }

    public void PickupBolt(BoltController bolt)
    {
        CurrentBoltsHeld[bolt.BoltType] = null;

        FinishLungeTrigger.Trigger();
        
        Destroy(bolt.gameObject);
    }

    public void Hit()
    {
        Hit(Vector2.zero);
    }
    
    public void Hit(Vector2 dir)
    {
        LastHitDir = dir;
        GetHitTrigger.Trigger();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<BoltController>() is { } bolt)
        {
            if (LayerUtil.MaskContainsLayer(_boltLayer, bolt.gameObject.layer))
            {
                PickupBolt(bolt);
            }
        }

        if (LayerUtil.MaskContainsLayer(_hitLayer, other.gameObject.layer))
        {
            Hit();
        }
    }

    
}

public struct VampireStateContext
{
    public PlayerController PlayerController { get; set; }
    public InputActions InputActions { get; set; }

    public VampireStateContext(PlayerController playerController, InputActions inputActions)
    {
        PlayerController = playerController;
        InputActions = inputActions;
    }
}
