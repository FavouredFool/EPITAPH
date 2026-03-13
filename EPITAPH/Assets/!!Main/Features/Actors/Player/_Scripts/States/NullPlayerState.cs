using UnityEngine;

public class NullPlayerState : VampireBaseState
{
    public NullPlayerState(VampireStateContext ctx) : base(ctx)
    {
    }

    public override void OnEnter()
    {
        _ctx.PlayerController.Rb.linearVelocity = Vector2.zero;
    }
}
