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
        Debug.Log("enter ravage");
        
        _startTime = Time.time;
        
        _ctx.PlayerController.CharacterAnimator.SetTrigger(PlayerController.RavageTriggerAnim);

        _hasEaten = false;

        Explode();
    }
    
    public override void Update()
    {
        if (!_hasEaten &&(Time.time - _startTime) > _ctx.PlayerController.RavageTime / 2)
        {
            PlayerVariableAnchor.PlayerVariables.Health += 1;
            _hasEaten = true;
        }
        
        if ((Time.time - _startTime) > _ctx.PlayerController.RavageTime)
        {
            _ctx.PlayerController.FinishRavageTrigger.Trigger();
        }
    }

    void Explode()
    {
        
    }

    public override void OnExit()
    {
        Debug.Log("exit ravage");
    }
}
