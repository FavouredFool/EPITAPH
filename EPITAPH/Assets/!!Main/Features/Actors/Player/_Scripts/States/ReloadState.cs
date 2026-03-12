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
        _ctx.InputActions.Player.Reload.canceled += ReloadInputStop;
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsReloadingBoolAnim, true);
        _ctx.PlayerController.CrossbowAnimator.SetBool(PlayerController.IsReloadBoolAnim, true);
        _reloadStart = Time.time;
        
        PlayerAudio.StartCharging();
        if (_ctx.PlayerController.ParryCooldown >= 1)
        {
            _ctx.PlayerController.IsParrying = true;
            _ctx.PlayerController.currentParryTime = 0;
            _ctx.PlayerController.ParryEffect.Play();
        }
    }

    public override void Update()
    {
        _ctx.PlayerController.ReadInput();
        if (_ctx.PlayerController.ParryCooldown >= 1)
        {
            if (_ctx.PlayerController.currentParryTime < _ctx.PlayerController.MaxParryTime)
            {
                _ctx.PlayerController.IsParrying = true;
                _ctx.PlayerController.currentParryTime += Time.deltaTime;
            }
            else
            {
                _ctx.PlayerController.IsParrying = false;
            }
        }
        UpdateReload();
    }
    
    public override void FixedUpdate()
    {
        _ctx.PlayerController.MovementVelocity = Vector2.zero;
        _ctx.PlayerController.CalculateVelocity();
        
        _ctx.PlayerController.Rotation();
    }

    public override void OnExit()
    {
        if (_ctx.PlayerController.ParryCooldown >= 1)
        {
            _ctx.PlayerController.IsParrying = false;
            _ctx.PlayerController.currentParryTime = 0;
            _ctx.PlayerController.ParryEffect.Stop();
            _ctx.PlayerController.ParryCooldown = 0;
        }
        _ctx.InputActions.Player.Reload.canceled -= ReloadInputStop;
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsReloadingBoolAnim, false);
        _ctx.PlayerController.CrossbowAnimator.SetBool(PlayerController.IsReloadBoolAnim, false);
    }
    
    public void ReloadInputStop(InputAction.CallbackContext ctx)
    {
        FinishReload();
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
        // TODO only listen to input to stop the charge (or the highest has been reached)
        
        _ctx.PlayerController.StopReloadTrigger.Trigger();
        
        PlayerAudio.StopCharging();

        if (PlayerVariableAnchor.PlayerVariables.Charge != 3)
        {
            PlayerVariableAnchor.PlayerVariables.ChargeProgress = 0;
        }
        else
        {
            PlayerVariableAnchor.PlayerVariables.ChargeProgress = 1;
        }

        PlayerAudio.PlayStepLock(3); // TODO: ADD Steps
        
    }
}