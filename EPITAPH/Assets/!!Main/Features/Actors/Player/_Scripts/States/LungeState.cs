using UnityEngine;

public class LungeState : VampireBaseState
{
    public LungeState(VampireStateContext ctx) : base(ctx)
    {

    }

    public override void OnEnter()
    {
        //Debug.Log("start lunge to " + boltType);
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