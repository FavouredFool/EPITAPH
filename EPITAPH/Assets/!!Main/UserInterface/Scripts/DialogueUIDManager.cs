using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using DG.Tweening;
using Yarn;

public class DialogueUIDManager : MonoBehaviour
{
    [SerializeField] RectTransform _dialogueWindowsRect,_cornerRect;

    [SerializeField] Image _FlowTimerFill;
    [SerializeField] CanvasGroup _FlowTimerGroup;
    public static int _draculaFlowWait=6;

    public bool IsOpen { get; set; }

    public static float LastDraculaFlowTime;
    public static bool IsDraculaFlowNext=>IsDraculaFlowing && Time.time-LastDraculaFlowTime>=_draculaFlowWait;
    public static bool IsDraculaFlowing { get; set; }

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

    void Update()
    {
        _FlowTimerGroup.alpha = IsDraculaFlowing ? 1 : 0;
        if (IsDraculaFlowing)
        {
            _FlowTimerFill.fillAmount = (Time.time - LastDraculaFlowTime) / _draculaFlowWait;

            if (IsDraculaFlowNext)
            {
                SignalBus.Fire(new Signal_DialogueForceContinue());
            }
        }
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

    [YarnCommand("ToggleDraculaFlow")]public static void ToggleDraculaFLow(bool on = true)
    {
        IsDraculaFlowing=true;
    }
}