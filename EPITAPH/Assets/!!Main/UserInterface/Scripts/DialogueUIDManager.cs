using UnityEngine;
using Yarn.Unity;
using DG.Tweening;
using Yarn;

public class DialogueUIDManager : MonoBehaviour
{
    [SerializeField] RectTransform _dialogueWindowsRect,_cornerRect;

    public bool IsOpen { get; set; }

    void Awake()
    {
        _dialogueWindowsRect.anchoredPosition = new Vector2(1000, 1124);
    }

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_DialogueToggled>(Toggle);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_DialogueToggled>(Toggle);
    }

    public void Toggle(Signal_DialogueToggled signal) => Toggle(signal.on);
    public void Toggle(bool on)
    {
        if (IsOpen == on) return;
        IsOpen = on;

Debug.Log("Dialogue "+on);

        DOTween.Kill(this);
        Sequence seq = DOTween.Sequence(this);
        seq.SetUpdate(true);

        if (IsOpen)
        {
            Debug.Log("GO");
            seq.Insert(0, _dialogueWindowsRect.DOAnchorPosX(-2000,2f).SetEase(Ease.OutCirc));
        }
        else
        {
                        Debug.Log("NO");

            seq.Insert(0, _dialogueWindowsRect.DOAnchorPosX(1000,1f).SetEase(Ease.InSine));
        }

        SignalBus.Fire(new Signal_DialogueUIToggled(IsOpen));
    }
}