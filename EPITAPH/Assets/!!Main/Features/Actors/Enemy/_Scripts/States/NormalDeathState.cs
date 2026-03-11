using UnityEngine;

public class NormalDeathState : EnemyBaseState
{
    static readonly int NormalDeathTriggerAnim = Animator.StringToHash("NormalDeath");
    
    public NormalDeathState(EnemyStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _ctx.EnemyController.Animator.SetBool(NormalDeathTriggerAnim, true);
        _ctx.EnemyController.Die();
        _ctx.EnemyController.Rb.simulated = false;
    }

    public override void Update()
    {

    }
    
    public override void FixedUpdate()
    {

    }

    public override void OnExit()
    {

    }
}