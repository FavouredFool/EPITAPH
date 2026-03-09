using UnityEngine;

// Interface that defines the Subscriber to Events done by the player
public interface AudioEventSubscriber<T> where T:AudioEvent
{
    void OnEventHappened(T e);
}
