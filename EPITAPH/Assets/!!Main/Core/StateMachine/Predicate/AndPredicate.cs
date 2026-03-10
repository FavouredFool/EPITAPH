using System.Linq;
using UnityEngine;

public class AndPredicate : IStatePredicate
{
    readonly IStatePredicate[] _predicates;
    
    // WARNING: Be careful with using multiple different Trigger Predicates. They evalulate and reset themselves every frame, therefore they dont cache their state. This will never trigger if all predicates aren't true on the exact same frame.
    public AndPredicate(params IStatePredicate[] predicates)
    {
        _predicates = predicates;
    }
    
    public bool Evaluate()
    {
        return _predicates.All(e => e.Evaluate());
    }
}
