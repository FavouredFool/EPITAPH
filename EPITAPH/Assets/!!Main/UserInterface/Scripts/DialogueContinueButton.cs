using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using DG.Tweening;

public class DialogueContinueButton : MonoBehaviour
{
    public CanvasGroup group;
    void OnEnable()
    {
        SignalBus.Subscribe<Signal_DialogueDefaultContinueToggled>(RefreshGroup);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_DialogueDefaultContinueToggled>(RefreshGroup);
    }

    public void RefreshGroup(Signal_DialogueDefaultContinueToggled signal) => RefreshGroup();
    public void RefreshGroup()
    {
        group.alpha= YarnManager.IsDefaultDialogueContinueActive? 1:0;
    }
}