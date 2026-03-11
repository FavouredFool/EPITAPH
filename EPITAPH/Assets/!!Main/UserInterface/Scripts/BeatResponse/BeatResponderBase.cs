using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BeatResponderBase : MonoBehaviour, AudioEventSubscriber<EarlyBeatChanged>, AudioEventSubscriber<BeatChanged>
{
    public Vector2 modulo= Vector2.one;
    public bool play=true;

    protected virtual void Awake()
    {
        modulo.x-=1;
        play=true;
    }
    void OnEnable()
    {
        AudioBus.Subscribe<EarlyBeatChanged>(this);
        AudioBus.Subscribe<BeatChanged>(this);
    }

    void OnDisable()
    {
                DOTween.Kill(this);
        AudioBus.Unsubscribe<EarlyBeatChanged>(this);
        AudioBus.Unsubscribe<BeatChanged>(this);
    }

    public void OnEventHappened(EarlyBeatChanged e)
    {
        if(!play) return;
      
        if ((e.beat % modulo.y) == modulo.x) BeatTrigger(e);

    }
    public virtual void OnOnBeatEventHappened(BeatChanged e)
    {
      
    }
    public virtual void BeatTrigger(EarlyBeatChanged e)
    {
        
    }

    public void Toggle(bool on)
    {
        Halt();
        play = on;
    }
    public void Halt()
    {
        DOTween.Kill(this, true);
    }

    void AudioEventSubscriber<BeatChanged>.OnEventHappened(BeatChanged e)
    {
        OnOnBeatEventHappened(e);
    }

}