using UnityEngine;

public class ParryState : VampireBaseState
{
    public ParryState(VampireStateContext ctx) : base(ctx)
    {
    }

    public override void OnEnter()
    {
        _ctx.PlayerController.LastParry = Time.time;
        _ctx.PlayerController.SuccessfulParryHappened = false;
        _ctx.PlayerController.ParryEffect.Play();
        _ctx.PlayerController.Rb.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        if (Time.time - _ctx.PlayerController.LastParry > _ctx.PlayerController.ParryActiveTime)
        {
            // Exit Parry
            _ctx.PlayerController.ParryExitTrigger.Trigger();
        }
    }

    public override void FixedUpdate()
    {
        _ctx.PlayerController.SuccessfulParryHappened = false;
    }

    public override void OnExit()
    {
        _ctx.PlayerController.ParryEffect.Stop();
    }
}
