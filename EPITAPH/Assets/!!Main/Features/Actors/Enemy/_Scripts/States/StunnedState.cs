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
            _ctx.EnemyController.Animator.SetTrigger("ExitStun");

        }
        base.FixedUpdate();
    }

    public override void OnEnter()
    {
        time = 0;
        _ctx.EnemyController.Animator.SetTrigger("EnterStun");
        _ctx.EnemyController.GetParried.Play();
        _ctx.EnemyController.StunnedReset();
        base.OnEnter();
    }

    public override void OnExit()
    {

        base.OnExit();

    }

    public override void Update()
    {
        base.Update();
    }
}