using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeatResponderGroup : MonoBehaviour
{
    List<BeatResponderBase> responders = new();

    void OnEnable()
    {
        responders = GetComponentsInChildren<BeatResponderBase>().ToList();
    }

    public void Halt()
    {
        foreach (BeatResponderBase responder in responders)
            responder.Halt();
    }

    public void Toggle(bool on)
    {
        foreach (BeatResponderBase responder in responders)
            responder.Toggle(on);
    }

}
