using UnityEngine;
using Yarn.Unity;
using DG.Tweening;

public class DialogueUIDManager : MonoBehaviour
{
    [SerializeField] RectTransform _dialogueWindowsRect,_cornerRect;

    public bool IsOpen { get; set; }

    void Awake()
    {
        _dialogueWindowsRect.anchoredPosition = new Vector2(2500, 0);
        _cornerRect.anchoredPosition = new Vector2(3000, -1125);
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

        //DOTween.Kill(this);
        Sequence seq = DOTween.Sequence(this);
        seq.SetUpdate(true);

        if (IsOpen)
        {
            Debug.Log("GO");
            seq.Insert(0, _dialogueWindowsRect.DOAnchorPosX(0,2f).SetEase(Ease.OutCirc));
            seq.Insert(0, _cornerRect.DOAnchorPosX(2000,1f).SetEase(Ease.OutCirc));
        }
        else
        {
                        Debug.Log("NO");

            seq.Insert(0, _dialogueWindowsRect.DOAnchorPosX(2500,1f).SetEase(Ease.InSine));
            seq.Insert(0, _cornerRect.DOAnchorPosX(3000,0.5f).SetEase(Ease.InSine));
        }

        SignalBus.Fire(new Signal_DialogueUIToggled(IsOpen));
    }
}