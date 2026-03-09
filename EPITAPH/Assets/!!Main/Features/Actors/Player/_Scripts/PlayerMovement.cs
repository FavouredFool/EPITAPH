using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1, 20)] float _speed;
    
    InputActions _inputActions;
    Rigidbody2D _rb;

    Vector2 _movementInput;
    
    void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _movementInput = _inputActions.Player.Movement.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = _movementInput * _speed;
    }
}
