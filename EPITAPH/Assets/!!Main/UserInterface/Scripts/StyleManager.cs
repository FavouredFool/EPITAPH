using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StyleManager : MonoBehaviour
{
    void Update()
    {
        if (PlayerVariableAnchor.PlayerVariables.Style > 0 && PlayerVariableAnchor.PlayerVariables.StyleResetI >= 1)
            PlayerVariableAnchor.PlayerVariables.Style = 0;
    }

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_EnemyDeath>(GainStyle);
        SignalBus.Subscribe<Signal_PlayerDamage>(ResetStyle);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_EnemyDeath>(GainStyle);
        SignalBus.Unsubscribe<Signal_PlayerDamage>(ResetStyle);
    }

    public void GainStyle(Signal_EnemyDeath signal)
    {
        PlayerVariableAnchor.PlayerVariables.Style++;
    }
    public void ResetStyle(Signal_PlayerDamage signal)
    {
        PlayerVariableAnchor.PlayerVariables.Style = 0;
    }
}
