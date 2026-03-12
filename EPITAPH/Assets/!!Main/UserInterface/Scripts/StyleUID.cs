using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StyleUID : MonoBehaviour
{
    [SerializeField] TMP_Text _textShadow, _textBackground, _textFill;
    [SerializeField] Image _timerFill;

    [SerializeField]string[] coolWords;

    public void SetText(string text)
    {
        _textShadow.text = text;
        _textBackground.text = text;
        _textFill.text = text;
    }

    void Update()
    {
        if (PlayerVariableAnchor.PlayerVariables.Style > 0)
            _timerFill.fillAmount = 1f-PlayerVariableAnchor.PlayerVariables.StyleResetI;
    }

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_RefreshUI_Style>(RefreshUID);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_RefreshUI_Style>(RefreshUID);
    }

    public void RefreshUID(Signal_RefreshUI_Style signal)
    {
        Debug.Log("STYLE UI");
        SetText(coolWords[signal.variables.Style]);
        Color color = _timerFill.color;
        _timerFill.color = Color.white;
        _timerFill.DOColor(color, 0.5f).SetEase(Ease.InOutSine);
    }
}
