using UnityEngine;

public class NormalDeathState : EnemyBaseState
{
    static readonly int NormalDeathTriggerAnim = Animator.StringToHash("NormalDeath");
    static readonly int ReviveTriggerAnim = Animator.StringToHash("Revive");

    float _reviveTime;
    float _startTime;
    
    public NormalDeathState(EnemyStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _ctx.EnemyController.Animator.SetBool(NormalDeathTriggerAnim, true);
        
        _ctx.EnemyController.Knockback(_ctx.EnemyController.LatestHitVelocity);

        _reviveTime = Random.Range(_ctx.EnemyController.ReviveRange.x, _ctx.EnemyController.ReviveRange.y);
        _startTime = Time.time;
    }

    public override void Update()
    {
        if (_ctx.EnemyController.KnockbackVelocity.sqrMagnitude < 0.05)
        {
            _ctx.EnemyController.Rb.simulated = false;
        }

        if (Time.time - _startTime > _reviveTime)
        {
            // revive
            _ctx.EnemyController.ReviveTrigger.Trigger();
        }
    }
    
    public override void FixedUpdate()
    {
        _ctx.EnemyController.KnockbackVelocity *= Mathf.Exp(-_ctx.EnemyController.KnockbackDecay * Time.fixedDeltaTime);
        _ctx.EnemyController.Rb.linearVelocity = _ctx.EnemyController.KnockbackVelocity;

        Vector2 dir = -_ctx.EnemyController.Rb.linearVelocity.normalized;
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        _ctx.EnemyController.Rb.MoveRotation(Quaternion.Euler(0f, 0f, angle));
    }

    public override void OnExit()
    {
        _ctx.EnemyController.Animator.SetTrigger(ReviveTriggerAnim);
    }
}