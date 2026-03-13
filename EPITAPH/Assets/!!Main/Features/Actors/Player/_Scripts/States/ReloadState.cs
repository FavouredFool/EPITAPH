using UnityEngine;
using UnityEngine.InputSystem;

public class ReloadState : VampireBaseState
{
    float _reloadStart = float.PositiveInfinity;
    public ReloadState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _ctx.InputActions.Player.Shoot.performed += _ctx.PlayerController.ShootBoltInput;
        _ctx.InputActions.Player.UseBolt.performed += UseActiveBoltInput;
        _ctx.InputActions.Player.Parry.performed += _ctx.PlayerController.ParryInput;
        
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsReloadingBoolAnim, true);
        _ctx.PlayerController.CrossbowAnimator.SetBool(PlayerController.IsReloadBoolAnim, true);
        
        _ctx.PlayerController.UpdateActiveBolt(true);
        _ctx.PlayerController.UpdateActiveBolt(true);
        
        _reloadStart = Time.time;
        
        PlayerAudio.StartCharging();

        PlayerCameraController.DoFOV(30,3,DG.Tweening.Ease.OutSine);
    }

    public override void Update()
    {
        _ctx.PlayerController.ReadInput();
        _ctx.PlayerController.UpdateActiveBolt(false);
        
        UpdateReload();
        
        _ctx.PlayerController.SetCameraFollow(_ctx.PlayerController.RotateInput.sqrMagnitude > 0.05f);
    }
    
    public override void FixedUpdate()
    {
        _ctx.PlayerController.MovementVelocity = _ctx.PlayerController.MovementInput * _ctx.PlayerController.Speed * 0;
        _ctx.PlayerController.CalculateVelocity();

        if (_ctx.PlayerController.RotateInput.sqrMagnitude > 0.05)
        {
            _ctx.PlayerController.LookDirection = _ctx.PlayerController.AimAssistedLookDirection;
            _ctx.PlayerController.Rotation();
        }
        
    }

    public override void OnExit()
    {
        FinishReload();
        
        _ctx.PlayerController.SetCameraFollow(false);
        
        _ctx.PlayerController.UpdateActiveBolt(true);
        _ctx.PlayerController.SetCameraFollow(false);
        
        _ctx.InputActions.Player.Shoot.performed -= _ctx.PlayerController.ShootBoltInput;
        _ctx.InputActions.Player.UseBolt.performed -= UseActiveBoltInput;
        _ctx.InputActions.Player.Parry.performed -= _ctx.PlayerController.ParryInput;
        
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsReloadingBoolAnim, false);
        _ctx.PlayerController.CrossbowAnimator.SetBool(PlayerController.IsReloadBoolAnim, false);
    }

    public void UpdateReload()
    {
        if (PlayerVariableAnchor.PlayerVariables.Charge == 3)
        {
            FinishReload();
            return;
        }
        
        if (PlayerVariableAnchor.PlayerVariables.ChargeProgress >= 1)
        {
            PlayerVariableAnchor.PlayerVariables.ChargeProgress = 0;
            PlayerVariableAnchor.PlayerVariables.Charge += 1;
            PlayerAudio.PlayStepLock(PlayerVariableAnchor.PlayerVariables.Charge);
            _reloadStart = Time.time;
        }
        
        float reloadTime;

        if (PlayerVariableAnchor.PlayerVariables.Charge == 0)
        {
            reloadTime = _ctx.PlayerController.ReloadTimeCharge1;
        }
        else if (PlayerVariableAnchor.PlayerVariables.Charge == 1)
        {
            reloadTime = _ctx.PlayerController.ReloadTimeCharge2;
        }
        else if (PlayerVariableAnchor.PlayerVariables.Charge == 2)
        {
            reloadTime = _ctx.PlayerController.ReloadTimeCharge3;
        }
        else
        {
            return;
        }
        
        float chargeT = (Time.time - _reloadStart) / reloadTime;
        PlayerAudio.SetCharge(chargeT);
        PlayerVariableAnchor.PlayerVariables.ChargeProgress = chargeT;
    }

    public void FinishReload()
    {
        PlayerAudio.StopCharging();

        if (PlayerVariableAnchor.PlayerVariables.Charge != 3)
        {
            PlayerVariableAnchor.PlayerVariables.ChargeProgress = 0;
        }
        else
        {
            PlayerVariableAnchor.PlayerVariables.ChargeProgress = 1;
        }
    }
    
    public void UseActiveBoltInput(InputAction.CallbackContext ctx)
    {
        _ctx.PlayerController.UseActiveBolt();
    }
}