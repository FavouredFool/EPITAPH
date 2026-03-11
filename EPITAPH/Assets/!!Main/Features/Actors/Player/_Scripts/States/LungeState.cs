using UnityEngine;

public class LungeState : VampireBaseState
{
    BoltController _lungeBolt;
    
    public LungeState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        _lungeBolt = _ctx.PlayerController.CurrentLungeBolt;
        
        // find actual bolt from type
        
        Debug.Log("start lunge to " + _lungeBolt);
        _ctx.PlayerController.CharacterAnimator.SetTrigger(PlayerController.IsLungingTriggerAnim);
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