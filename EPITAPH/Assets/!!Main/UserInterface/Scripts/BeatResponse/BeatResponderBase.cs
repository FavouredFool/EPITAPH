using UnityEngine;
using UnityEngine.UI;

public class BeatResponderBase : MonoBehaviour, AudioEventSubscriber<BeatChanged>
{
    public Vector2 modulo= Vector2.one;

    protected virtual void Awake()
    {
        modulo.x= modulo.x % modulo.y;
        AudioBus.Subscribe(this);
    }

    protected virtual void OnDisable()
    {
        AudioBus.Unsubscribe(this);
    }

    public void OnEventHappened(BeatChanged e)
    {
        if ((e.beat % modulo.y) == modulo.x) BeatTrigger(e);
    }
    public virtual void BeatTrigger(BeatChanged e)
    {
        
    }

}