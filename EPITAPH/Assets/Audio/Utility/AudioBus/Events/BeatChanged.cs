using System.Threading;
using UnityEngine;

public class BeatChanged : AudioEvent
{
    public BeatChanged(int bar,int beat) 
    { 
        this.bar = bar;
        this.beat = beat;
    }

    public int bar;
    public int beat;
}
