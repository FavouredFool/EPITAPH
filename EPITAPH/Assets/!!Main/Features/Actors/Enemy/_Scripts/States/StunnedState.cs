using System.Diagnostics;

public class StunnedState : EnemyBaseState
{
    public StunnedState(EnemyStateContext ctx) : base(ctx)
    {
    }


    float time = 0;
   
    public override void FixedUpdate()
    {
        time += UnityEngine.Time.fixedDeltaTime;
        if (time >= 5)
        {
            _ctx.EnemyController.ExitStun.Trigger();
        }
        base.FixedUpdate();
    }

    public override void OnEnter()
    {
        time = 0;
        _ctx.EnemyController.Animator.SetTrigger("EnterStun");
        base.OnEnter();
    }

    public override void OnExit()
    {
        _ctx.EnemyController.Animator.SetTrigger("ExitStun");

        base.OnExit();

    }

    public override void Update()
    {
        base.Update();
    }
}