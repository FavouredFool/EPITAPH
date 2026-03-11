using UnityEngine;

public class EverythingState : EnemyBaseState
{
    public EverythingState(EnemyStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {

    }

    public override void Update()
    {
        _ctx.EnemyController.EverythingUpdate();
    }
    
    public override void FixedUpdate()
    {
        _ctx.EnemyController.EverythingFixedUpdate();
    }

    public override void OnExit()
    {

    }
}