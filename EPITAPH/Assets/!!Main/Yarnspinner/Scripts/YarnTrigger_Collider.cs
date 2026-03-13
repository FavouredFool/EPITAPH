using Unity.VisualScripting;
using UnityEngine;

public class YarnTrigger_Collider : YarnTriggerBase
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasPlayed) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player == null) return;

        SpinYarn();
    }
}
