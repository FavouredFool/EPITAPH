using UnityEngine;

public class LungeState : VampireBaseState
{
    BoltController _lungeBolt;
    float _startDistance;

    float _startLungeTime;
    
    public LungeState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _lungeBolt = _ctx.PlayerController.CurrentLungeBolt;
        
        // find actual bolt from type
        
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsLungingBoolAnim, true);
        _ctx.PlayerController.MovementVelocity = Vector2.zero;

        _startDistance = (_lungeBolt.Rb2D.position - _ctx.PlayerController.Rb.position).magnitude;
        _startLungeTime = Time.time;

        // AUDIO
        PlayerAudio.PlayLunge();

        // TODO add a max lunge time for a breakout when something goes horribly wrong
    }

    public override void Update()
    {
        
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
        
        Debug.Log(_ctx.PlayerController.MovementVelocity.magnitude);


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
        
        // relative to distance, maybe meh
        //float distanceT = Mathf.Clamp01(diff.magnitude / _startDistance);
        //
        //float speed = _ctx.PlayerController.LungeSpeed *
        //              (0.1f + (1f - Mathf.Exp(-_ctx.PlayerController.LungePower * distanceT)));
        //
        //_ctx.PlayerController.MovementVelocity = dir * speed;
        
        _ctx.PlayerController.CalculateVelocity();
        
        if (_ctx.PlayerController.Rb.linearVelocity.sqrMagnitude > 0.05)
        {
            _ctx.PlayerController.LookDirection = _ctx.PlayerController.Rb.linearVelocity.normalized;
        }
        
        _ctx.PlayerController.Rotation();
    }

    public override void OnExit()
    {
        _ctx.PlayerController.CharacterAnimator.SetBool(PlayerController.IsLungingBoolAnim, false);
        _ctx.PlayerController.MovementVelocity = Vector2.zero;
    }
}