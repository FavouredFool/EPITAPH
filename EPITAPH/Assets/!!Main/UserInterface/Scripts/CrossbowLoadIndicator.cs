using UnityEngine.UI;
using UnityEngine;

public class CrossbowLoadIndicator : MonoBehaviour
{
    public GameObject indicator;

    public Color[] colors;

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_RefreshUI_Charge>(RefreshCharge);
    }    
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_RefreshUI_Charge>(RefreshCharge);
    }
    public void RefreshCharge(Signal_RefreshUI_Charge signal)
    {
        indicator.SetActive(signal.variables.Charge>0);  
    }
}
