using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] Transform _instantiationParent;
    [SerializeField] Rigidbody2D _projectileBlueprint;
    
    InputActions _inputActions;
    
    void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();
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
    }
}
