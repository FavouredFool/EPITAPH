using UnityEngine;

public class NullEnemyState : EnemyBaseState
{
    public NullEnemyState(EnemyStateContext ctx) : base(ctx)
    {
        
    }
    

    public override void OnEnter()
    {
        _ctx.EnemyController.Rb.linearVelocity = Vector2.zero;
    }

}
