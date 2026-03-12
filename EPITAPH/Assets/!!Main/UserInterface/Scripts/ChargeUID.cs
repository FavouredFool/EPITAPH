using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUID : MonoBehaviour
{
    [SerializeField] Image _ChargeBar;
    [SerializeField] TMP_Text _ChargeText;

    [SerializeField] RectTransform _CenterRect;
    [SerializeField] Image _ChargedBoltIndicator;

    [SerializeField]AnimationCurve _progressEase;
    [SerializeField] Color[]_chargeColors;

    Color ChargeColor(int charge, float progress)
    {
        Color col = Color.Lerp(_chargeColors[charge], _chargeColors[Mathf.Clamp(charge + 1, 0, _chargeColors.Length - 1)], progress);
        return col;
    }

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
        Color col= ChargeColor(value, signal.variables.ChargeProgress);

        DOTween.Kill(this,true);
        Sequence seq = DOTween.Sequence(this);

        seq.Insert(0, _CenterRect.DOPunchScale(Vector3.one * 0.25f, 0.5f, 1).SetEase(Ease.OutCirc));

        _ChargedBoltIndicator.gameObject.SetActive(value!=0);
        _ChargedBoltIndicator.color= col;

    }
    public void RefreshUID(Signal_RefreshUI_ChargeProgress signal)
    {
        float value = signal.variables.ChargeProgress;
        int charge = (int)signal.variables.Charge;
        Color col= ChargeColor(charge, value);

        _ChargeBar.fillAmount = _progressEase.Evaluate(value);
        _ChargeBar.color = col;
        _ChargeText.color = col;
    }
}
