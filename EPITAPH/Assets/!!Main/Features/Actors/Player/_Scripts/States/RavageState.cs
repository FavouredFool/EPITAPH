using System.Linq;
using UnityEngine;
public class RavageState : VampireBaseState
{
    float _startTime;
    bool _hasEaten;
    
    public RavageState(VampireStateContext ctx) : base(ctx)
    {
    }

    public override void OnEnter()
    {
        _startTime = Time.time;
        
        _ctx.PlayerController.CharacterAnimator.SetTrigger(PlayerController.RavageTriggerAnim);

        _hasEaten = false;

        Explode();
        _ctx.PlayerController.ExplosionVFXObject.Play();
        PlayerAudio.PlayBite();
    }
    
    public override void Update()
    {
        if (!_hasEaten &&(Time.time - _startTime) > _ctx.PlayerController.RavageTime / 2)
        {
            PlayerVariableAnchor.PlayerVariables.Health = PlayerVariableAnchor.PlayerVariables.HealthMax;
            _hasEaten = true;
            
            _ctx.PlayerController.SetChargeMin(Mathf.Clamp(PlayerVariableAnchor.PlayerVariables.Charge + 1, 0, 3));
        }
        
        if ((Time.time - _startTime) > _ctx.PlayerController.RavageTime)
        {
            _ctx.PlayerController.FinishRavageTrigger.Trigger();
        }
    }

    public override void FixedUpdate()
    {
        _ctx.PlayerController.MovementVelocity = Vector2.zero;
        _ctx.PlayerController.CalculateVelocity();
    }

    void Explode()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(_ctx.PlayerController.Rb.position, _ctx.PlayerController.ExplosionRadius);
        
        foreach (EnemyController enemy in enemiesHit.Select(e => e.GetComponentInParent<EnemyController>()).Where(e => e !=null))
        {
            Vector2 dir = (enemy.Rb.position - _ctx.PlayerController.Rb.position).normalized;
            enemy.LatestHitVelocity = dir * _ctx.PlayerController.ExplosionKnockbackStrength;
            enemy.NormalDeathTrigger.Trigger();
        }
    }

    public override void OnExit()
    {
      //  _ctx.PlayerController.ExplosionVFXObject.gameObject.SetActive(false);
    }
}
