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
    
    
    InputActions _inputActions;
    Rigidbody2D _rb;

    Vector2 MovementInput { get; set; }
    Vector2 RotateInput  { get; set; }

    float _reloadStart = float.PositiveInfinity;

    bool _isReloading = false;
    bool _isLunging = false;

    bool _boltInChamber = true;
    Dictionary<BoltType, bool> _currentBoltsHeld;
    
    // Crossbow
    static readonly int ShootCrossbowTrigger = Animator.StringToHash("Shoot");
    static readonly int StartReloadTrigger = Animator.StringToHash("StartReload");
    static readonly int InterruptReloadTrigger = Animator.StringToHash("InterruptReload");
    static readonly int FinishReloadTrigger = Animator.StringToHash("FinishReload");
    
    // Player
    static readonly int IsMovingBool = Animator.StringToHash("IsMoving");
    static readonly int IsAimingBool = Animator.StringToHash("IsAiming");
    static readonly int IsReloadingBool = Animator.StringToHash("IsReloading");
    static readonly int IsLungingTrigger = Animator.StringToHash("IsLunging");
    static readonly int ShotCharacterTrigger = Animator.StringToHash("Shot");
    

    public Vector2 AimAssistedLookDirection
    {
        get
        {
            Vector2 rawAimDir = RotateInput.normalized;

            if (!IsAiming) return rawAimDir;
            
            float angle = Mathf.Atan2(rawAimDir.y, rawAimDir.x) * Mathf.Rad2Deg - 90;
            float assistedAngle = _aimAssist.GetAssistedAngle(angle, transform.position);
            
            return Quaternion.Euler(0, 0, assistedAngle) * Vector2.up;
        }
    }

    Vector2 _movementVelocity;
    Vector2 _knockbackVelocity;
    
    public bool IsAiming => RotateInput.magnitude > _moveLockThreshold && _boltInChamber;
    // arbitrary threshold
    public bool IsMoving => _rb.linearVelocity.magnitude > 0.05f;
    
    void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        _rb = GetComponent<Rigidbody2D>();

        _currentBoltsHeld = new Dictionary<BoltType, bool>
        {
            [BoltType.DOWN] = true,
            [BoltType.LEFT] = true,
            [BoltType.UP] = true,
            [BoltType.RIGHT] = true
        };

        PADScriptableObject.Setup(this.gameObject);
        
        Assert.IsNotNull(_characterAnimator);
        Assert.IsNotNull(_crossbowAnimator);
    }

    void Start()
    {
        
    }


    void OnEnable()
    {
        _inputActions.Player.Shoot.performed += ShootBoltInput;
        _inputActions.Player.Reload.performed += ReloadInputStart;
        _inputActions.Player.Reload.canceled += ReloadInputStop;
        
        _inputActions.Player.LungeDown.performed += LungeDownInput;
        _inputActions.Player.LungeLeft.performed += LungeLeftInput;
        _inputActions.Player.LungeUp.performed += LungeUpInput;
        _inputActions.Player.LungeRight.performed += LungeRightInput;
    }
    
    void OnDisable()
    {
        _inputActions.Player.Shoot.performed -= ShootBoltInput;
        _inputActions.Player.Reload.performed -= ReloadInputStart;
        _inputActions.Player.Reload.canceled -= ReloadInputStop;
        
        _inputActions.Player.LungeDown.performed -= LungeDownInput;
        _inputActions.Player.LungeLeft.performed -= LungeLeftInput;
        _inputActions.Player.LungeUp.performed -= LungeUpInput;
        _inputActions.Player.LungeRight.performed -= LungeRightInput;
    }

    void Update()
    {
        MovementInput = _inputActions.Player.Movement.ReadValue<Vector2>();
        RotateInput = _inputActions.Player.Look.ReadValue<Vector2>();

        UpdateReload();
        CameraPos();

        UpdateAnimationParams();
    }

    void FixedUpdate()
    {
        Translation();
        Rotation();
    }

    void UpdateAnimationParams()
    {
        _characterAnimator.SetBool(IsAimingBool, IsAiming);
        _characterAnimator.SetBool(IsMovingBool, IsMoving);
        _characterAnimator.SetBool(IsReloadingBool, _isReloading);
    }

    void UpdateReload()
    {
        if (!_isReloading) return;
        
        if (Time.time - _reloadStart > _reloadTime)
        {
            FinishReload();
        }
    }
    
    void CameraPos()
    {
        if (IsAiming)
        {
            _cameraFollow.localPosition = Vector3.up * _cameraAimOffset;
        }
        else
        {
            _cameraFollow.localPosition = Vector3.zero;
        }
    }

    void Translation()
    {
        if (_isReloading)
        {
            _movementVelocity = Vector2.zero;
            //_movementVelocity = MovementInput * _speed / _speedReloadReduction;
        }
        else
        {
            if (IsAiming)
            {
                _movementVelocity = MovementInput * _speed / _speedAimReduction;
            }
            else
            {
                _movementVelocity = MovementInput * _speed;
            }
        }

        
        // knockback
        _knockbackVelocity *= Mathf.Exp(-_knockbackDecay * Time.deltaTime);
        
        // Combine
        _rb.linearVelocity = _movementVelocity + _knockbackVelocity;
    }

    void Rotation()
    {
        Vector2 dir = transform.up;
        
        if (IsAiming)
        {
            dir = AimAssistedLookDirection;
        }
        else if (MovementInput.sqrMagnitude > 0.01)
        {
            dir = MovementInput.normalized;
        }
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        _rb.MoveRotation(Quaternion.Euler(0f, 0f, angle));
    }
    
    public void Knockback(Vector2 velocity)
    {
        _knockbackVelocity += velocity;
    }
    
    void ShootBoltInput(InputAction.CallbackContext ctx)
    {
        ShootBolt();
    }

    void ReloadInputStart(InputAction.CallbackContext ctx)
    {
        StartReload();
    }

    void ReloadInputStop(InputAction.CallbackContext ctx)
    {
        InterruptReload();
    }

    void LungeRightInput(InputAction.CallbackContext ctx)
    {
        LungeToBolt(BoltType.RIGHT);
    }
    
    void LungeDownInput(InputAction.CallbackContext ctx)
    {
        LungeToBolt(BoltType.DOWN);
    }
    
    void LungeLeftInput(InputAction.CallbackContext ctx)
    {
        LungeToBolt(BoltType.LEFT);
    }
    
    void LungeUpInput(InputAction.CallbackContext ctx)
    {
        LungeToBolt(BoltType.UP);
    }

    void LungeToBolt(BoltType boltType)
    {
        if (_isReloading) return;
        
        Debug.Log("start lunge to " + boltType);

        _isLunging = true;
        _characterAnimator.SetTrigger(IsLungingTrigger);
    }

    void StartReload()
    {
        if (_boltInChamber) return;
        // no bolts left
        if (_currentBoltsHeld.Values.All(e => !e)) return;
        
        _reloadStart = Time.time;
        _isReloading = true;
        _crossbowAnimator.SetTrigger(StartReloadTrigger);
    }

    void FinishReload()
    {
        _boltInChamber = true;
        _isReloading = false;
        _crossbowAnimator.SetTrigger(FinishReloadTrigger);
    }

    void InterruptReload()
    {
        if (!_isReloading) return;
        
        _reloadStart = float.PositiveInfinity;
        _isReloading = false;
        _crossbowAnimator.SetTrigger(InterruptReloadTrigger);
    }
    
    void ShootBolt()
    {
        if (!IsAiming) return;
        
        BoltType type = GetBoltTypeToShoot();

        if (type == BoltType.NONE) return;

        _currentBoltsHeld[type] = false;
        _boltInChamber = false;
        
        BoltController bolt = Instantiate(_projectileBlueprint, transform.position + transform.forward * _spawnDist, transform.rotation, _instantiationParent);
        bolt.BoltType = type;
        bolt.BloodpointPlayer = _bloodlineConnection;
        
        Knockback(-transform.up * _knockbackStrength);

        // Play Audio
        PlayerAudio.PlayReleaseCrossbow();
        
        // Animation
        Debug.Log("SHOT");
        _crossbowAnimator.SetTrigger(ShootCrossbowTrigger);
        _characterAnimator.SetTrigger(ShotCharacterTrigger);
    }

    BoltType GetBoltTypeToShoot()
    {
        foreach (var kv in _currentBoltsHeld.Where(kv => kv.Value))
        {
            return kv.Key;
        }

        return BoltType.NONE;
    }

    void PickupBolt(BoltController bolt)
    {
        _currentBoltsHeld[bolt.BoltType] = true;

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
