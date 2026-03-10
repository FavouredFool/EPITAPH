using UnityEngine;

public class EarlyBeatChanged : AudioEvent
{
    public EarlyBeatChanged( int beat)
    {
        this.beat = beat;
    }

    public int beat;
}