using UnityEngine;

public class StateTransition : IStateTransition
{
    public IState To { get; }
    public IStatePredicate Condition { get; }

    public StateTransition(IState to, IStatePredicate condition)
    {
        To = to;
        Condition = condition;
    }
}
