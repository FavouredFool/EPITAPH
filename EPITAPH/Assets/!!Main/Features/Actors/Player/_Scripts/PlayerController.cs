using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask _boltLayer;
    
    [Header("Movement")]
    [SerializeField, Range(1, 20)] float _speed;
    [SerializeField, Range(1, 20)] float _speedAimReduction;
    [SerializeField, Range(1, 20)] float _speedReloadReduction;
    [SerializeField, Range(0, 0.95f)] float _moveLockThreshold = 0.3f;
    [SerializeField, Range(1, 20)] float _knockbackDecay;
    [SerializeField] AimAssistV3 _aimAssist;
    
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

    public float Speed => _speed;
    public float SpeedAimReduction => _speedAimReduction;
    
    public Animator CharacterAnimator => _characterAnimator;
    public Animator CrossboxAnimator => _crossbowAnimator;

    public float ReloadTime => _reloadTime;
    
    InputActions _inputActions;
    Rigidbody2D _rb;

    // Input
    public Vector2 MovementInput { get; set; }
    public Vector2 RotateInput  { get; set; }

    // Dir
    public Vector2 LookDirection { get; set; }
    
    // Bolts
    public bool BoltInChamber { get; set; } = true;
    public Dictionary<BoltType, bool> CurrentBoltsHeld { get; private set; }

    // State Machine
    public StateMachine StateMachine { get; set; }
    
    // State Triggers
    public TriggerPredicate ShootTrigger { get; private set; }
    public TriggerPredicate LungeTrigger { get; private set; }
    public TriggerPredicate FinishLungeTrigger { get; private set; }
    public TriggerPredicate StartReloadTrigger { get; private set; }
    public TriggerPredicate StopReloadTrigger { get; private set; }
    
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
    public static readonly int IsLungingTriggerAnim = Animator.StringToHash("IsLunging");
    public static readonly int ShotCharacterTriggerAnim = Animator.StringToHash("Shot");
    

    public Vector2 AimAssistedLookDirection
    {
        get
        {
            Vector2 rawAimDir = RotateInput.normalized;
            
            float angle = Mathf.Atan2(rawAimDir.y, rawAimDir.x) * Mathf.Rad2Deg - 90;
            float assistedAngle = _aimAssist.GetAssistedAngle(angle, transform.position);
            
            return Quaternion.Euler(0, 0, assistedAngle) * Vector2.up;
        }
    }

    public Vector2 MovementVelocity { get; set; }
    public Vector2 KnockbackVelocity { get; set; }
    
    void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        _rb = GetComponent<Rigidbody2D>();

        CurrentBoltsHeld = new Dictionary<BoltType, bool>
        {
            [BoltType.DOWN] = true,
            [BoltType.LEFT] = true,
            [BoltType.UP] = true,
            [BoltType.RIGHT] = true
        };

        PADScriptableObject.Setup(this.gameObject);
        
        Assert.IsNotNull(_characterAnimator);
        Assert.IsNotNull(_crossbowAnimator);

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

        ShootTrigger = new TriggerPredicate();
        LungeTrigger = new TriggerPredicate();
        FinishLungeTrigger = new TriggerPredicate();
        StartReloadTrigger = new TriggerPredicate();
        StopReloadTrigger = new TriggerPredicate();
 

        bool isAiming = RotateInput.magnitude > _moveLockThreshold && BoltInChamber;
        
        At(moveState, aimState, new FuncStatePredicate(() => isAiming));
        At(aimState, moveState, new FuncStatePredicate(() => !isAiming));
        
        At(moveState, reloadState, StartReloadTrigger);
        At(aimState, reloadState, StartReloadTrigger);
        
        At(reloadState, moveState, StopReloadTrigger);
        
        At(aimState, shootState, ShootTrigger);
        // Todo doublecheck if this is correct
        At(shootState, aimState, new FuncStatePredicate(() => true));
        
        At(moveState, lungeState, LungeTrigger);
        At(aimState, lungeState, LungeTrigger);
        
        At(lungeState, moveState, FinishLungeTrigger);
        
        StateMachine.SetState(moveState);
    }
    
    void At(IState from, IState to, IStatePredicate condition) =>
        StateMachine.AddTransition(from, to, condition);
    
    void Any(IState to, IStatePredicate condition) =>
        StateMachine.AddAnyTransition(to, condition);
    

    void Update()
    {
        StateMachine.Update();
        Debug.Log(StateMachine.CurrentState);
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
        _rb.linearVelocity = MovementVelocity + KnockbackVelocity;
    }
    

    public void Rotation()
    {
        // LookDirection set by State
        
        float angle = Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg - 90f;
        _rb.MoveRotation(Quaternion.Euler(0f, 0f, angle));
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
        Debug.Log("start lunge to " + boltType);

        _characterAnimator.SetTrigger(IsLungingTriggerAnim);
        
        
        // Set state machine trigger
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
        if (CurrentBoltsHeld.Values.All(e => !e)) return;
        
        StartReloadTrigger.Trigger();
    }
    

    public BoltType GetBoltTypeToShoot()
    {
        foreach (var kv in CurrentBoltsHeld.Where(kv => kv.Value))
        {
            return kv.Key;
        }

        return BoltType.NONE;
    }

    public void PickupBolt(BoltController bolt)
    {
        CurrentBoltsHeld[bolt.BoltType] = true;

        Destroy(bolt.gameObject);
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
