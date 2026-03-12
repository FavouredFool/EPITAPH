using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask _hitLayer;
    [SerializeField] LayerMask _boltLayer;
    
    [Header("Hit")]
    [field: SerializeField, Range(1, 6)] public float HitSpeedMultiplier { get; set; } = 2.5f;
    [field: SerializeField, Range(0.1f, 10)] public float BatTime { get; set; } = 2f;
    [field: SerializeField] public GameObject Visual3DMesh { get; set; }
    [field: SerializeField] public GameObject BatVFXObject { get; set; }
    [field: SerializeField] public Collider2D MainCollider { get; set; }
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
    [SerializeField] BoltController _projectileBlueprint;
    [SerializeField, Range(0, 4)] float _spawnDist;
    [SerializeField] Transform _bloodlineConnection;
    [field: SerializeField, Range(0.1f, 10)] public float ReloadTimeCharge1 { get; private set; }
    [field: SerializeField, Range(0.1f, 10)] public float ReloadTimeCharge2 { get; private set; }
    [field: SerializeField, Range(0.1f, 10)] public float ReloadTimeCharge3 { get; private set; }
    
    [Header("Cam")]
    [SerializeField] Transform _cameraFollow;
    [SerializeField, Range(0, 10)] float _cameraAimOffset;

    [Header("Audio")]
    [SerializeField] PlayerAudioData PADScriptableObject;

    [Header("Animation")]
    [SerializeField] Animator _characterAnimator;
    [SerializeField] Animator _crossbowAnimator;

    [Header("Bolt")]
    [SerializeField, Range(0, 180)] float _maxActivateAngle = 40;
    
    [field: Header("Lunge")]
    [field:SerializeField, Range(1, 100)] public float LungeSpeed { get; private set; }
    [field:SerializeField, Range(1, 200)] public float LungeAcceleration { get; private set; }
    [field: SerializeField, Range(1, 6)] public float LungePower { get; private set; } = 3;
    [field: SerializeField, Range(0, 1)] public float InitalDelay { get; private set; } = 0.1f;
    [field: SerializeField, Range(0, 30)] public float FailsaveExitTime { get; private set; } = 10f;
    [field: SerializeField] public Collider2D LungeCollider { get; private set; }
    [field: SerializeField, Range(0, 200)] public float LungeKnockbackStrength { get; private set; } = 100;
    
    [field: Header("Ravage")]
    [field: SerializeField, Range(0, 10)] public float ExplosionRadius { get; private set; } = 3;
    [field: SerializeField] public GameObject ExplosionVFXObject { get; set; }
    [field: SerializeField, Range(0, 10)] public float RavageTime { get; private set; } = 1;
    [field: SerializeField, Range(0, 500)] public float ExplosionKnockbackStrength { get; private set; } = 200;
    
    public float Speed => _speed;
    public float SpeedAimReduction => _speedAimReduction;
    
    public Animator CharacterAnimator => _characterAnimator;
    public Animator CrossbowAnimator => _crossbowAnimator;
    public float SpawnDist => _spawnDist;
    public Transform BloodlineConnection => _bloodlineConnection;

    public BoltController ProjectileBlueprint => _projectileBlueprint;
    
    InputActions _inputActions;
    public Rigidbody2D Rb { get; set; }

    // Input
    public Vector2 MovementInput { get; set; }
    public Vector2 RotateInput  { get; set; }

    // Dir
    public Vector2 LookDirection { get; set; }
    
    public Vector2 LastHitDir { get; set; }
    
    bool IsAiming => RotateInput.magnitude > _moveLockThreshold;
    public bool IsParrying = false;
    public float MaxParryTime = 1;
    public float currentParryTime = 0;
    public bool BoltInChamber => PlayerVariableAnchor.PlayerVariables.Charge >= 1;
    
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
    public TriggerPredicate FinishRavageTrigger { get; private set; }
    
    // Hashes
    // Crossbow
    public static readonly int ShootCrossbowTriggerAnim = Animator.StringToHash("Shoot");
    public static readonly int IsReloadBoolAnim = Animator.StringToHash("IsReload");
    
    // Player
    public static readonly int IsMovingBoolAnim = Animator.StringToHash("IsMoving");
    public static readonly int IsAimingBoolAnim = Animator.StringToHash("IsAiming");
    public static readonly int IsReloadingBoolAnim = Animator.StringToHash("IsReloading");
    public static readonly int EnterLungeTriggerAnim = Animator.StringToHash("EnterLunge");
    public static readonly int EnterIdle = Animator.StringToHash("EnterIdle");
    public static readonly int ShotCharacterTriggerAnim = Animator.StringToHash("Shot");
    public static readonly int RavageTriggerAnim = Animator.StringToHash("Ravage");
    public static readonly int AimDirXFloatAnim = Animator.StringToHash("AimDirX");
    public static readonly int AimDirYFloatAnim = Animator.StringToHash("AimDirY");

    // Both
    public static readonly int ChargeIntAnim = Animator.StringToHash("Charge");
    
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

        PlayerVariableAnchor.PlayerVariables.CurrentBoltsHeld = new Dictionary<BoltType, BoltController>
        {
            [BoltType.DOWN] = null,
            [BoltType.LEFT] = null,
            [BoltType.UP] = null,
            [BoltType.RIGHT] = null
        };

        PADScriptableObject.Setup(this.gameObject);
        
        Assert.IsNotNull(_characterAnimator);
        Assert.IsNotNull(_crossbowAnimator);

        InitStateMachine();
    }

    void Start()
    {
        PlayerVariableAnchor.PlayerVariables.Health = PlayerVariableAnchor.PlayerVariables.HealthMax;
        PlayerVariableAnchor.PlayerVariables.Charge = 1;
        PlayerVariableAnchor.PlayerVariables.ChargeProgress = 0;
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
        RavageState ravageState = new(ctx);

        ShootTrigger = new TriggerPredicate();
        LungeTrigger = new TriggerPredicate();
        FinishLungeTrigger = new TriggerPredicate();
        StartReloadTrigger = new TriggerPredicate();
        StopReloadTrigger = new TriggerPredicate();
        GetHitTrigger = new TriggerPredicate();
        FinishBatTrigger = new TriggerPredicate();
        FinishRavageTrigger = new TriggerPredicate();
        
        At(moveState, aimState, new FuncStatePredicate(() => IsAiming));
        At(aimState, moveState, new FuncStatePredicate(() => !IsAiming));
        
        At(moveState, reloadState, StartReloadTrigger);
        At(aimState, reloadState, StartReloadTrigger);
        
        At(reloadState, moveState, StopReloadTrigger);
        
        At(aimState, shootState, ShootTrigger);
        At(shootState, aimState, new FuncStatePredicate(() => true));
        
        At(moveState, lungeState, LungeTrigger);
        At(aimState, lungeState, LungeTrigger);
        
        //At(lungeState, moveState, Rava);
        At(lungeState, ravageState, FinishLungeTrigger);
        At(ravageState, moveState, FinishRavageTrigger);
        
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
        
        CrossbowAnimator.SetInteger(ChargeIntAnim, PlayerVariableAnchor.PlayerVariables.Charge);
        CharacterAnimator.SetInteger(ChargeIntAnim, PlayerVariableAnchor.PlayerVariables.Charge);
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
    
    public void LungeToBolt(BoltType boltType)
    {
        if (PlayerVariableAnchor.PlayerVariables.CurrentBoltsHeld[boltType] == null) return;
        
        BoltController boltToLunge = PlayerVariableAnchor.PlayerVariables.CurrentBoltsHeld[boltType];

        if (!boltToLunge.IsLineOfSight) return;
 
        CurrentLungeBolt = boltToLunge;
        LungeTrigger.Trigger();
    }
    
    public void ShootBolt()
    {
        BoltType type = GetBoltTypeToShoot();
        if (type == BoltType.NONE) return;
        if (!BoltInChamber) return;

        ShootTrigger.Trigger();
    }
    
    public void StartReload()
    {
        if (PlayerVariableAnchor.PlayerVariables.Charge == 3) return;
        if (PlayerVariableAnchor.PlayerVariables.CurrentAmmoCount == 0) return;
        
        StartReloadTrigger.Trigger();
    }
    

    public BoltType GetBoltTypeToShoot()
    {
        foreach (var kv in PlayerVariableAnchor.PlayerVariables.CurrentBoltsHeld.Where(kv => kv.Value == null))
        {
            return kv.Key;
        }

        return BoltType.NONE;
    }

    public void PickupBolt(BoltController bolt)
    {
        PlayerVariableAnchor.PlayerVariables.AddAmmo(bolt.BoltType);
        
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

    public void UpdateActiveBolt(bool toggleAllOff)
    {
        BoltController[] bolts = PlayerVariableAnchor.PlayerVariables.CurrentBoltsHeld.Select(e => e.Value).Where(e => e != null).ToArray();

        BoltController bestBolt = null;
        float bestAngle = float.PositiveInfinity;


        Vector2 aimDir = RotateInput.normalized;
        
        foreach (BoltController bolt in bolts)
        {
            Vector2 boltDir = (bolt.Rb2D.position - Rb.position).normalized;

            float angle = Vector2.Angle(aimDir, boltDir);

            if (angle > _maxActivateAngle)
            {
                continue;
            }
            
            if (angle < bestAngle)
            {
                bestAngle = angle;
                bestBolt = bolt;
            }
        }
        
        foreach (BoltController boltToTurnOff in bolts)
        {
            boltToTurnOff.IsSelected = boltToTurnOff == bestBolt && !toggleAllOff;
        }
    }

    public void UseActiveBolt()
    {
        BoltController activeBolt = PlayerVariableAnchor.PlayerVariables.CurrentBoltsHeld.FirstOrDefault(e => e.Value != null && e.Value.IsActivatable).Value;

        if (activeBolt == null) return;
        
        CurrentLungeBolt = activeBolt;
        LungeTrigger.Trigger();
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
            // I hate guarding like this, but its easy and stuff like this is hard to delegate into the states.
            // Maybe this will make problems or more states have to get added later.
            if (StateMachine.CurrentState is not LungeState)
            {
                Hit();
            }
        }
        
        if (other.GetComponentInParent<EnemyController>() is { } enemy)
        {
            // same comment as above
            if (StateMachine.CurrentState is LungeState)
            {
                // i opted away from a normal push in favour of an instant kill thing for the lunge
                
                Vector2 dir = (enemy.Rb.position - Rb.position).normalized;
                //enemy.Hit(dir * _lungeKnockbackStrength);

                enemy.LatestHitVelocity = dir * LungeKnockbackStrength;
                enemy.NormalDeathTrigger.Trigger();
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
