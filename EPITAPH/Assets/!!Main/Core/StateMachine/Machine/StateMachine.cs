using System;
using System.Collections.Generic;
using System.Linq;

public class StateMachine
{
    StateNode _current;
    Dictionary<(Type, string), StateNode> _nodes = new();
    HashSet<IStateTransition> _anyTransitions = new();

    public IState CurrentState => _current?.State;

    public void Update()
    {
        IStateTransition transition = GetTransition();
        if (transition != null)
        {
            ChangeState(transition.To);
        }

        _current.State?.Update();
    }

    public void FixedUpdate()
    {
        _current.State?.FixedUpdate();
    }

    public void OnDestroy()
    {
        CurrentState.OnExit();
    }

    public void SetState(IState state)
    {
        _current = _nodes[(state.GetType(), state.GetKey())];
        _current.State?.OnEnter();
    }

    void ChangeState(IState state)
    {
        IState previousState = _current.State;
        IState nextState = _nodes[(state.GetType(), state.GetKey())].State;

        previousState?.OnExit();
        nextState?.OnEnter();

        _current = _nodes[(state.GetType(), state.GetKey())];
    }

    IStateTransition GetTransition()
    {
        foreach (IStateTransition transition in _anyTransitions.Where(transition => transition.Condition.Evaluate()))
        {
            // While it is an any transition, it feels foolish to implicitly jump from the state into itself, triggering the OnEnter and OnExit per Frame.
            if (_current.State == transition.To) continue;
            
            return transition;
        }
        
        IStateTransition foundTransition = _current.Transitions.FirstOrDefault(transition => transition.Condition.Evaluate());
        return foundTransition;
    }

    public void AddTransition(IState from, IState to, IStatePredicate condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(IState to, IStatePredicate condition)
    {
        _anyTransitions.Add(new StateTransition(GetOrAddNode(to).State, condition));
    }

    StateNode GetOrAddNode(IState state)
    {
        StateNode node = _nodes.GetValueOrDefault((state.GetType(), state.GetKey()));

        if (node == null)
        {
            node = new StateNode(state);
            _nodes.Add((state.GetType(), state.GetKey()), node);
        }

        return node;
    }
    
    class StateNode
    {
        public IState State { get; }
        public HashSet<IStateTransition> Transitions { get; }

        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<IStateTransition>();
        }

        public void AddTransition(IState to, IStatePredicate condition)
        {
            Transitions.Add(new StateTransition(to, condition));
        }
    }
}
