using UnityEngine;

public class BeatReceiverExample : MonoBehaviour,AudioEventSubscriber<BeatChanged> 
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

    public void OnEventHappened(BeatChanged e)
    {
        Debug.Log($"{Time.time} Beat: {e.beat}\n Bar: {e.bar}");
    }
}
