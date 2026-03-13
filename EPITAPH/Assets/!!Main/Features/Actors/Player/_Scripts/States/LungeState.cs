using UnityEngine;

public class LungeState : VampireBaseState
{
    BoltController _lungeBolt;

    float _startLungeTime;
    
    public LungeState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _lungeBolt = _ctx.PlayerController.CurrentLungeBolt;
        
        // find actual bolt from type
        
        _ctx.PlayerController.CharacterAnimator.SetTrigger(PlayerController.EnterLungeTriggerAnim);
        _ctx.PlayerController.MovementVelocity = Vector2.zero;

        _ctx.PlayerController.MainCollider.enabled = false;
        _ctx.PlayerController.LungeCollider.enabled = true;
        _ctx.PlayerController.WallCollider.enabled = false;
        
        _startLungeTime = Time.time;
        _ctx.PlayerController.BatDash.Play();
        // AUDIO
        PlayerAudio.PlayLunge();

        PlayerCameraController.DoFOV(30,1,DG.Tweening.Ease.OutCirc);
    }

    public override void Update()
    {
        if ((Time.time - _startLungeTime) > _ctx.PlayerController.FailsaveExitTime)
        {
            _ctx.PlayerController.LungeToRavageTrigger.Trigger();
            _ctx.PlayerController.CharacterAnimator.SetTrigger(PlayerController.EnterIdle);
        }
    }
    
    public override void FixedUpdate()
    {
        if (_lungeBolt == null) return;

        if ((Time.time - _startLungeTime) < _ctx.PlayerController.InitalDelay) return;
        
        Vector2 diff = (_lungeBolt.Rb2D.position - _ctx.PlayerController.Rb.position);
        Vector2 dir = diff.normalized;
        
        // acceleration
        _ctx.PlayerController.MovementVelocity += dir * (_ctx.PlayerController.LungeAcceleration * Time.deltaTime);

        _ctx.PlayerController.MovementVelocity = Vector2.ClampMagnitude(
            _ctx.PlayerController.MovementVelocity,
            _ctx.PlayerController.LungeSpeed
        );
        

        // linear speed
        
        //_ctx.PlayerController.MovementVelocity = dir * _ctx.PlayerController.LungeSpeed * Time.deltaTime * 1000;
        //Debug.Log(_ctx.PlayerController.MovementVelocity);
        
        // pow
        //float maxSpeed = _ctx.PlayerController.LungeSpeed;
        //float acceleration = _ctx.PlayerController.LungeAcceleration;
        //
        //float t = _ctx.PlayerController.MovementVelocity.magnitude / maxSpeed;
        //
        //float accelMultiplier = Mathf.Pow(t, _ctx.PlayerController.LungePower) + 0.1f;
        //
        //_ctx.PlayerController.MovementVelocity +=
        //    direction * (acceleration * accelMultiplier * Time.fixedDeltaTime);
        //
        //_ctx.PlayerController.MovementVelocity = Vector2.ClampMagnitude(
        //    _ctx.PlayerController.MovementVelocity,
        //    _ctx.PlayerController.LungeSpeed
        //);
        
        
        _ctx.PlayerController.CalculateVelocity();
        
        if (_ctx.PlayerController.Rb.linearVelocity.sqrMagnitude > 0.05)
        {
            _ctx.PlayerController.LookDirection = _ctx.PlayerController.Rb.linearVelocity.normalized;
        }
        
        _ctx.PlayerController.Rotation();
    }

    public override void OnExit()
    {
        _ctx.PlayerController.MovementVelocity = Vector2.zero;
        
        _ctx.PlayerController.MainCollider.enabled = true;
        _ctx.PlayerController.LungeCollider.enabled = false;
        _ctx.PlayerController.WallCollider.enabled = true;
        CameraShake.Instance.TriggerShake(_ctx.PlayerController.transform.forward, 0.5f);

        PlayerCameraController.DoFOV(35,1f,DG.Tweening.Ease.InOutSine);

        //if (PlayerVariableAnchor.PlayerVariables.Charge < 1)
        //{
        //    PlayerVariableAnchor.PlayerVariables.Charge = 1;
        //}
    }
}