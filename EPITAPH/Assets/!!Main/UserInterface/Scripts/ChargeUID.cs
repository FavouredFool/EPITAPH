using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUID : MonoBehaviour
{
    [SerializeField] Image _ChargeBar;
    [SerializeField] TMP_Text _ChargeText;

    [SerializeField] RectTransform _CenterRect;

    [SerializeField]AnimationCurve _progressEase;

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_RefreshUI_Charge>(RefreshUID);
        SignalBus.Subscribe<Signal_RefreshUI_ChargeProgress>(RefreshUID);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_RefreshUI_Charge>(RefreshUID);
        SignalBus.Unsubscribe<Signal_RefreshUI_ChargeProgress>(RefreshUID);
    }

    public void RefreshUID(Signal_RefreshUI_Charge signal)
    {
        int value = (int)signal.variables.Charge;
        _ChargeText.text= value.ToString();
    }
    public void RefreshUID(Signal_RefreshUI_ChargeProgress signal)
    {
        float value = signal.variables.ChargeProgress;

        _ChargeBar.fillAmount=_progressEase.Evaluate(value);
    }
}
