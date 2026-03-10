using System;
using UnityEngine;

public class EventStatePredicate : IStatePredicate
{
    bool _eventInvoked;
    readonly Action<Action> _unsubscribeAction;
    
    public EventStatePredicate(Action<Action> subscribe, Action<Action> unsubscribe)
    {
        subscribe(EventInvoked);
        _unsubscribeAction = unsubscribe;
    }

    void EventInvoked()
    {
        _eventInvoked = true;
    }

    // currently unsubscribe is never called because the class is constructed in-line. I am not sure if this could ever lead to problems. If so, you have to unsubscribe all these Predicates when the statemachine dies.
    // (When the state is killed you also unsubscribe and kill its condition)
    public void Unsubscribe()
    {
        _unsubscribeAction(EventInvoked);
    }
    
    public bool Evaluate()
    {
        if (!_eventInvoked) return false;
        
        _eventInvoked = false;
        return true;
    }
}