using UnityEngine;

public class MoveState : VampireBaseState
{
    public MoveState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _ctx.InputActions.Player.Reload.performed += _ctx.PlayerController.ReloadInputStart;
        
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsMovingBoolAnim, true);
    }

    public override void Update()
    {
        _ctx.PlayerController.ReadInput();
    }
    
    public override void FixedUpdate()
    {
        _ctx.PlayerController.MovementVelocity = _ctx.PlayerController.MovementInput * _ctx.PlayerController.Speed;
        _ctx.PlayerController.CalculateVelocity();
        
        Vector2 inputDir = _ctx.PlayerController.MovementInput.normalized;

        if (inputDir.sqrMagnitude > 0.05)
        {
            _ctx.PlayerController.LookDirection = inputDir;
        }
        
        _ctx.PlayerController.Rotation();
    }

    public override void OnExit()
    {
        _ctx.InputActions.Player.Reload.performed -= _ctx.PlayerController.ReloadInputStart;
        
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsMovingBoolAnim, false);
    }
}
