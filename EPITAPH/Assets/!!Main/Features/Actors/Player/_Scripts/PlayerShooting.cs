using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerShooting : MonoBehaviour
{
    [SerializeField, Range(0, 10)] float _knockbackStrength = 2;
    [SerializeField] Transform _instantiationParent;
    [SerializeField] Rigidbody2D _projectileBlueprint;
    
    InputActions _inputActions;
    PlayerController _playerController;
    
    void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        _playerController = GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        _inputActions.Player.Shoot.performed += ShootBoltInput;
    }
    
    void OnDisable()
    {
        _inputActions.Player.Shoot.performed -= ShootBoltInput;
    }

    void ShootBoltInput(InputAction.CallbackContext ctx)
    {
        ShootBolt();
    }
    
    void ShootBolt()
    {
        Instantiate(_projectileBlueprint, transform.position, transform.rotation, _instantiationParent);
        
        _playerController.Knockback(-transform.up * _knockbackStrength);
    }
}
