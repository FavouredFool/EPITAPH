using UnityEngine;

public class HitRecoveryState : VampireBaseState
{
    float _startTime;
    
    public HitRecoveryState(VampireStateContext ctx) : base(ctx)
    {
    }

    public override void OnEnter()
    {
        PlayerVariableAnchor.PlayerVariables.Health -= 1;

        if (PlayerVariableAnchor.PlayerVariables.Health <= 0)
        {
            Debug.Log("GAME OVER");
            return;
        }
        
        _startTime = Time.time;

        _ctx.PlayerController.Visual3DMesh.SetActive(false);
        _ctx.PlayerController.MainCollider.enabled = false;
        _ctx.PlayerController.VFXObject.SetActive(true);
        
        _ctx.PlayerController.Knockback(_ctx.PlayerController.LastHitDir * _ctx.PlayerController.KnockbackStrength);
    }

    public override void Update()
    {
        _ctx.PlayerController.ReadInput();
        
        if ((Time.time - _startTime) > _ctx.PlayerController.BatTime)
        {
            _ctx.PlayerController.FinishBatTrigger.Trigger();
        }
    }

    public override void FixedUpdate()
    {
        _ctx.PlayerController.MovementVelocity = _ctx.PlayerController.MovementInput * (_ctx.PlayerController.Speed * _ctx.PlayerController.HitSpeedMultiplier);
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
        _ctx.PlayerController.Visual3DMesh.SetActive(true);
        _ctx.PlayerController.MainCollider.enabled = true;
        _ctx.PlayerController.VFXObject.SetActive(false);
    }
}
