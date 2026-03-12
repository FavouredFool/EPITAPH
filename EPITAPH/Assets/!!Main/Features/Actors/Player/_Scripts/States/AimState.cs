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
        
        _ctx.PlayerController.SetCameraFollow(true);
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsAimingBoolAnim, true);
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsMovingBoolAnim, true);
        
        _ctx.PlayerController.UpdateActiveBolt(true);
        _ctx.InputActions.Player.UseBolt.performed += UseActiveBoltInput;
     
    }

    public override void Update()
    {
        _ctx.PlayerController.ReadInput();
        _ctx.PlayerController.UpdateActiveBolt(false);

        Vector2 localDir = _ctx.PlayerController.transform.InverseTransformDirection(_ctx.PlayerController.MovementInput);
        //Debug.Log(localDir);
        _ctx.PlayerController.CharacterAnimator.SetFloat(PlayerController.AimDirXFloatAnim, localDir.x);
        _ctx.PlayerController.CharacterAnimator.SetFloat(PlayerController.AimDirYFloatAnim, localDir.y);
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

        _ctx.InputActions.Player.UseBolt.performed -= UseActiveBoltInput;
        
        _ctx.PlayerController.SetCameraFollow(false);
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsAimingBoolAnim, false);
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsMovingBoolAnim, false);
        
        _ctx.PlayerController.UpdateActiveBolt(true);

      
        
    }

    public void UseActiveBoltInput(InputAction.CallbackContext ctx)
    {
        _ctx.PlayerController.UseActiveBolt();
    }
}