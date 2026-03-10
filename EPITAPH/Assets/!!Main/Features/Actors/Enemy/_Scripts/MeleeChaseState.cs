using UnityEngine;

public class MeleeChaseState : EnemyState
{


    public override void UpdateTick()
    {
        base.UpdateTick();
        /*
        _agent.speed = _speed;
        _agent.nextPosition = _rb.position;
        _agent.destination = _target.position;
        */
    }
}
