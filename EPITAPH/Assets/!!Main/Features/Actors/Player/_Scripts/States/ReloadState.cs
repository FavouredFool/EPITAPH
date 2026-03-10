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
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsReloadingBool, true);
        
        _reloadStart = Time.time;
        _ctx.PlayerController.CrossboxAnimator.SetTrigger(PlayerController.StartReloadTriggerAnim);
        PlayerAudio.StartCharging();

    }

    public override void Update()
    {
        _ctx.PlayerController.ReadInput();
        
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
        _ctx.InputActions.Player.Reload.canceled -= ReloadInputStop;
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsReloadingBool, false);
    }
    
    public void ReloadInputStop(InputAction.CallbackContext ctx)
    {
        InterruptReload();
    }
    
    public void UpdateReload()
    {
        PlayerAudio.SetCharge((Time.time - _reloadStart) / _ctx.PlayerController.ReloadTime);

        if (Time.time - _reloadStart >_ctx.PlayerController.ReloadTime)
        {
            FinishReload();
        }
    }

    public void FinishReload()
    {
        _ctx.PlayerController.StopReloadTrigger.Trigger();
        
        _ctx.PlayerController.BoltInChamber = true;
        PlayerAudio.StopCharging();

        PlayerAudio.PlayStepLock(3); // TODO: ADD Steps
        _ctx.PlayerController.CrossboxAnimator.SetTrigger(PlayerController.FinishReloadTriggerAnim);
    }

    public void InterruptReload()
    {
        _ctx.PlayerController.StopReloadTrigger.Trigger();
        
        _reloadStart = float.PositiveInfinity;

        PlayerAudio.StopCharging();

        _ctx.PlayerController.CrossboxAnimator.SetTrigger(PlayerController.InterruptReloadTriggerAnim);
    }
}