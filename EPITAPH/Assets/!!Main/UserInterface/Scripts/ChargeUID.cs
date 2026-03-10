using UnityEngine;
using UnityEngine.UI;

public class ChargeUID : MonoBehaviour
{
    [SerializeField] Image[] _ChargeBars;

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_RefreshUI_Charge>(RefreshUID);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_RefreshUI_Charge>(RefreshUID);
    }

    public void RefreshUID(Signal_RefreshUI_Charge signal)
    {
        float value= signal.variables.Charge;

        for(int i =0; i<_ChargeBars.Length; i++)
        {
            float fill= Mathf.Clamp01(value-i);
            _ChargeBars[i].fillAmount=fill;
        }
    }
}
