using Unity.VisualScripting;
using UnityEngine;

public class YarnTrigger_Collider : YarnTriggerBase
{
    public void OnTriggerEnter2D()
    {
        if (_hasPlayed) return;
        
        SpinYarn();
    }
}
