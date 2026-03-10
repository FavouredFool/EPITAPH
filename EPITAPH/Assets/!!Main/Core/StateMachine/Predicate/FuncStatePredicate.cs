using System;
using UnityEngine;

public class FuncStatePredicate : IStatePredicate
{
    readonly Func<bool> _func;

    public FuncStatePredicate(Func<bool> func)
    {
        _func = func;
    }
    
    public bool Evaluate()
    {
        return _func.Invoke();
    }
}
