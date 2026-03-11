using System.Linq;
using UnityEngine;

public class OrPredicate : IStatePredicate
{
    readonly IStatePredicate[] _predicates;
    
    public OrPredicate(params IStatePredicate[] predicates)
    {
        _predicates = predicates;
    }
    
    public bool Evaluate()
    {
        return _predicates.Any(e => e.Evaluate());
    }
}
