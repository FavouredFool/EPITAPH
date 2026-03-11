using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class YarnTriggerBase:MonoBehaviour
{
    [SerializeField] protected DialogueReference _nodeName;

    protected bool _hasPlayed=false;

    public void SpinYarn()
    {
        _hasPlayed=true;
        SignalBus.Fire(new Signal_StartDialogue(_nodeName));
    }
}
