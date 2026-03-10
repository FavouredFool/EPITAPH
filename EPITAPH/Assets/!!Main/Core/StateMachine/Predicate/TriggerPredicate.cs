using UnityEngine;

public class TriggerPredicate : IStatePredicate
{
    bool _triggered;

    public void Trigger()
    {
        _triggered = true;
    }
    
    public bool Evaluate()
    {
        if (!_triggered) return false;
        _triggered = false;
        return true;
    }
}
