using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1, 20)] float _speed;
    [SerializeField, Range(0, 0.95f)] float _moveLockThreshold = 0.3f;
    
    InputActions _inputActions;
    Rigidbody2D _rb;

    Vector2 _movementInput;
    Vector2 _rotateInput;
    
    void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _movementInput = _inputActions.Player.Movement.ReadValue<Vector2>();
        _rotateInput = _inputActions.Player.Look.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Translation();
        Rotation();
    }

    void Translation()
    {
        if (_rotateInput.sqrMagnitude > _moveLockThreshold)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }
        
        _rb.linearVelocity = _movementInput * _speed;
    }

    void Rotation()
    {
        Vector2 dir = transform.up;
        
        if (_rotateInput.sqrMagnitude > _moveLockThreshold)
        {
            dir = _rotateInput.normalized;
        }
        else if (_rb.linearVelocity.sqrMagnitude > 0.01)
        {
            dir = _rb.linearVelocity.normalized;
        }
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
