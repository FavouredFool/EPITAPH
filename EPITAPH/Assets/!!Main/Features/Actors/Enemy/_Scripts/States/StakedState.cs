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
        _ctx.EnemyController.StakedBlood.Play();
        _ctx.EnemyController.Rb.simulated = false;
        CameraShake.Instance.TriggerShake(Random.insideUnitSphere,1);

        _ctx.EnemyController.CurrentlyStickingBolt.IsStakeBolt = true;
        _ctx.EnemyController.CurrentlyStickingBolt.StickToNothing();
        _ctx.EnemyController.CurrentlyStickingBolt.EnableBoltMarker(true);
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
