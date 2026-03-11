using UnityEngine;

public class BeatReceiverExample : MonoBehaviour,AudioEventSubscriber<EarlyBeatChanged> 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        AudioBus.Subscribe(this);

    }

    // Update is called once per frame
    private void OnDestroy()
    {
        AudioBus.Unsubscribe(this);
    }

    public void OnEventHappened(EarlyBeatChanged e)
    {
    }
}
