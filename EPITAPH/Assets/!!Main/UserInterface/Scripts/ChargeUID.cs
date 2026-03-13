using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUID : MonoBehaviour
{
    [SerializeField] Image[] _chargeBars;
    [SerializeField] RectTransform[] _chargeMarkers;
     [SerializeField] RectTransform _chargeBarTip;
    [SerializeField] GameObject[] _chargePrompts, _chargeTooltipsLower,_chargeTooltipsUpper;

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
        RefreshBars(signal.variables);

        DOTween.Kill(this, true);
        Sequence seq= DOTween.Sequence(this);

        for (int i = 0; i < _chargeMarkers.Length; i++)
            seq.Insert(0,_chargeMarkers[i].DOScale(i==signal.variables.Charge - 1 ? Vector3.one : Vector3.one * 0.35f, 0.5f).SetEase(Ease.OutBack));
        
        if (signal.variables.Charge == 3)
            seq.Insert(0, _chargeBarTip.DOPunchScale(Vector3.one * 0.15f, 0.5f).SetEase(Ease.OutCirc));
    }
    public void RefreshUID(Signal_RefreshUI_ChargeProgress signal)
    {
        RefreshBars(signal.variables);
    }

    public void RefreshBars(PlayerVariables variables)
    {
        for (int i = 0; i < _chargeBars.Length; i++)
        {
            Image bar = _chargeBars[i];
            if (i < variables.Charge)
                bar.fillAmount = 1;
            else if (i == variables.Charge)
                bar.fillAmount = variables.ChargeProgress;
            else
                bar.fillAmount = 0;
        }

        for (int i = 0; i < _chargeTooltipsLower.Length; i++)
        {
            _chargeTooltipsLower[i].SetActive(i==variables.Charge);
            _chargeTooltipsUpper[i].SetActive(i==variables.Charge);
            _chargePrompts[i].SetActive(i==variables.Charge);
        }
    }
}
