using UnityEngine;

public interface IStateTransition
{
    IState To { get; }
    IStatePredicate Condition { get; }
}
