using Unity.VisualScripting;
using UnityEngine;

public class YarnProgressor_Collider : MonoBehaviour
{
    bool _hasPlayed=false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasPlayed) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player == null) return;

        _hasPlayed = true;
        SignalBus.Fire(new Signal_DialogueForceContinue());
    }
}
