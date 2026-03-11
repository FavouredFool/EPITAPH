using UnityEngine;
using UnityEngine.InputSystem;

public class AimState : VampireBaseState
{
    public AimState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _ctx.InputActions.Player.Shoot.performed += _ctx.PlayerController.ShootBoltInput;
        _ctx.InputActions.Player.Reload.performed += _ctx.PlayerController.ReloadInputStart;
        
        _ctx.InputActions.Player.LungeDown.performed += _ctx.PlayerController.LungeDownInput;
        _ctx.InputActions.Player.LungeLeft.performed += _ctx.PlayerController.LungeLeftInput;
        _ctx.InputActions.Player.LungeUp.performed += _ctx.PlayerController.LungeUpInput;
        _ctx.InputActions.Player.LungeRight.performed += _ctx.PlayerController.LungeRightInput;

        _ctx.PlayerController.SetCameraFollow(true);
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsAimingBoolAnim, true);
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsMovingBoolAnim, true);
    }

    public override void Update()
    {
        _ctx.PlayerController.ReadInput();
    }
    
    public override void FixedUpdate()
    {
        _ctx.PlayerController.MovementVelocity = _ctx.PlayerController.MovementInput * _ctx.PlayerController.Speed / _ctx.PlayerController.SpeedAimReduction;
        _ctx.PlayerController.CalculateVelocity();

        _ctx.PlayerController.LookDirection = _ctx.PlayerController.AimAssistedLookDirection;
        
        _ctx.PlayerController.Rotation();
    }

    public override void OnExit()
    {
        _ctx.InputActions.Player.Shoot.performed -= _ctx.PlayerController.ShootBoltInput;
        _ctx.InputActions.Player.Reload.performed -= _ctx.PlayerController.ReloadInputStart;
        
        _ctx.InputActions.Player.LungeDown.performed -= _ctx.PlayerController.LungeDownInput;
        _ctx.InputActions.Player.LungeLeft.performed -= _ctx.PlayerController.LungeLeftInput;
        _ctx.InputActions.Player.LungeUp.performed -= _ctx.PlayerController.LungeUpInput;
        _ctx.InputActions.Player.LungeRight.performed -= _ctx.PlayerController.LungeRightInput;
        
        _ctx.PlayerController.SetCameraFollow(false);
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsAimingBoolAnim, false);
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsMovingBoolAnim, false);
    }
}