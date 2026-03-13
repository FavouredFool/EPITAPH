using UnityEngine;

public class HitAndKnockbackedState : EnemyBaseState
{
    static readonly int EnterKnockbackedTriggerAnim = Animator.StringToHash("EnterKnockback");
    static readonly int ExitKnockbackTriggerAnim = Animator.StringToHash("ExitKnockback");
    
    public HitAndKnockbackedState(EnemyStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _ctx.EnemyController.Animator.SetTrigger(EnterKnockbackedTriggerAnim);
        _ctx.EnemyController.Knockback(_ctx.EnemyController.LatestHitVelocity);
        _ctx.EnemyController.BloodTrail.Play();
        _ctx.EnemyController.HitByBolt.Play();
        PlayerAudio.PlayMeatHit(_ctx.EnemyController.transform.position);
       
    }

    public override void Update()
    {
        if (_ctx.EnemyController.KnockbackVelocity.magnitude < _ctx.EnemyController.KnockbackMagnitudeThreshold)
        {
            _ctx.EnemyController.LatestHitVelocity = _ctx.EnemyController.KnockbackMagnitudeThreshold * _ctx.EnemyController.LatestHitVelocity.normalized;
            _ctx.EnemyController.NormalDeathTrigger.Trigger();
            
            // there's no way to normally recover from the knockback
            //_ctx.EnemyController.CurrentHp -= 1;
            //
            //if (_ctx.EnemyController.CurrentHp <= 0)
            //{
            //    // final push of corpse
            //    
            //}
            //else
            //{
            //    _ctx.EnemyController.Animator.SetTrigger(ExitKnockbackTriggerAnim);
            //    _ctx.EnemyController.ExitKnockback.Trigger();
            //}
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
        _ctx.EnemyController.BloodTrail.Stop();
        _ctx.EnemyController.Rb.linearVelocity = Vector2.zero;
    }
}