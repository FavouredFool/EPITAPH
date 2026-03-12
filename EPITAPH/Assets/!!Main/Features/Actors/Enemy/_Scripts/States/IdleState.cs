using System.Numerics;

public class IdleState : EnemyBaseState
{
    public IdleState(EnemyStateContext ctx) : base(ctx)
    {
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {

        base.OnExit();
    }

    public override void Update()
    {

        if (_ctx.EnemyController.IsTargetInRangeForChaseBegin())
        {
            _ctx.EnemyController.EnterChase.Trigger();

        }
        base.Update();
    }
}