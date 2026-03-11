using UnityEngine;

public class StakedState : EnemyBaseState
{
    static readonly int StakedTriggerAnim = Animator.StringToHash("Staked");
    
    public StakedState(EnemyStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _ctx.EnemyController.Animator.SetBool(StakedTriggerAnim, true);
        _ctx.EnemyController.Die();
        PlayerAudio.PlayWallHit(_ctx.EnemyController.transform.position);
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