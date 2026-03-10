using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Range(1, 20)] float _speed;
    [SerializeField, Range(1, 20)] float _speedAimReduction;
    [SerializeField, Range(0, 0.95f)] float _moveLockThreshold = 0.3f;
    [SerializeField, Range(1, 20)] float _knockbackDecay;
    [SerializeField] AimAssistV3 _aimAssist;
    
    [Header("Shooting")]
    [SerializeField, Range(0, 10)] float _knockbackStrength = 2;
    [SerializeField] Transform _instantiationParent;
    [SerializeField] BoltController _projectileBlueprint;
    [SerializeField, Range(0, 4)] float _spawnDist;
    
    [Header("Cam")]
    [SerializeField] Transform _cameraFollow;
    [SerializeField, Range(0, 10)] float _cameraAimOffset;
    
    
    InputActions _inputActions;
    Rigidbody2D _rb;

    Vector2 MovementInput { get; set; }
    Vector2 RotateInput  { get; set; }

    Dictionary<BoltType, bool> _currentBoltsHeld;

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

    Vector2 _movementVelocity;
    Vector2 _knockbackVelocity;
    
    public bool ReadyToShoot => RotateInput.magnitude > _moveLockThreshold;
    
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
    }

    void Start()
    {
        
    }


    void OnEnable()
    {
        _inputActions.Player.Shoot.performed += ShootBoltInput;
    }
    
    void OnDisable()
    {
        _inputActions.Player.Shoot.performed -= ShootBoltInput;
    }

    void Update()
    {
        MovementInput = _inputActions.Player.Movement.ReadValue<Vector2>();
        RotateInput = _inputActions.Player.Look.ReadValue<Vector2>();
        
        CameraPos();
    }

    void FixedUpdate()
    {
        Translation();
        Rotation();
    }

    void CameraPos()
    {
        if (ReadyToShoot)
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
        // movement
        if (ReadyToShoot)
        {
            //_movementVelocity = Vector2.zero;
            _movementVelocity = MovementInput * _speed / _speedAimReduction;
        }
        else
        {
            _movementVelocity = MovementInput * _speed;
        }
        
        // knockback
        _knockbackVelocity *= Mathf.Exp(-_knockbackDecay * Time.deltaTime);
        
        // Combine
        _rb.linearVelocity = _movementVelocity + _knockbackVelocity;
    }

    void Rotation()
    {
        Vector2 dir = transform.up;
        
        if (ReadyToShoot)
        {
            dir = AimAssistedLookDirection;
        }
        else if (_rb.linearVelocity.sqrMagnitude > 0.01)
        {
            dir = _rb.linearVelocity.normalized;
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
        if (!ReadyToShoot) return;
        
        ShootBolt();
    }
    
    void ShootBolt()
    {
        BoltType type = GetBoltTypeToShoot();

        if (type == BoltType.NONE) return;

        _currentBoltsHeld[type] = false;
        
        BoltController bolt = Instantiate(_projectileBlueprint, transform.position + transform.forward * _spawnDist, transform.rotation, _instantiationParent);
        bolt.BoltType = type;
        
        Knockback(-transform.up * _knockbackStrength);
    }

    BoltType GetBoltTypeToShoot()
    {
        foreach (var kv in _currentBoltsHeld.Where(kv => kv.Value))
        {
            return kv.Key;
        }

        return BoltType.NONE;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position + RotateInput.normalized);
        //
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position + AimAssistedLookDirection);
    }
}
