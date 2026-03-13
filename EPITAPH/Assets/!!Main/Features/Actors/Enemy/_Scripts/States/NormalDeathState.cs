using UnityEngine;

public class NormalDeathState : EnemyBaseState
{
    static readonly int NormalDeathTriggerAnim = Animator.StringToHash("NormalDeath");
    static readonly int ReviveTriggerAnim = Animator.StringToHash("Revive");

    float _reviveTime;
    float _startTime;

    bool _finishedKnockback;
    
    public NormalDeathState(EnemyStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _ctx.EnemyController.Animator.SetBool(NormalDeathTriggerAnim, true);
        
        _ctx.EnemyController.Knockback(_ctx.EnemyController.LatestHitVelocity);

        _reviveTime = Random.Range(_ctx.EnemyController.ReviveRange.x, _ctx.EnemyController.ReviveRange.y);
        _startTime = Time.time;

        _finishedKnockback = false;
    }

    public override void Update()
    {
        if (!_finishedKnockback && _ctx.EnemyController.KnockbackVelocity.sqrMagnitude < 0.05)
        {
            _finishedKnockback = true;

            if (_ctx.EnemyController.CurrentlyStickingBolt != null)
            {
                _ctx.EnemyController.CurrentlyStickingBolt.StickToNothing();
                _ctx.EnemyController.CurrentlyStickingBolt.EnableBoltMarker(false);
                _ctx.EnemyController.CurrentlyStickingBolt = null;
            }
            
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
        _ctx.EnemyController.Rb.simulated = true;
    }
}