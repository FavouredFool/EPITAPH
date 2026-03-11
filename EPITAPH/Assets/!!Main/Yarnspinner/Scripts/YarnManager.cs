using DG.Tweening;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;

public class YarnManager : MonoBehaviour
{
    [SerializeField] DialogueRunner _dialogueRunner;

    public static bool IsDefaultDialogueContinueActive { get; set; }

    public bool IsResponsive { get; set; }


    void OnEnable()
    {
        SignalBus.Subscribe<Signal_DialogueForceContinue>(ForceContinueDialogue);
        SignalBus.Subscribe<Signal_StartDialogue>(SpinYarn);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_DialogueForceContinue>(ForceContinueDialogue);
        SignalBus.Unsubscribe<Signal_StartDialogue>(SpinYarn);
    }

    public void SpinYarn(Signal_StartDialogue signal) => SpinYarn(signal.nodeName);
    public async void SpinYarn(string nodeName)
    {
        if (_dialogueRunner.IsDialogueRunning)
        {
            Debug.LogWarning("Dialogue started while it was already running!");
            return;
        }
        ToggleDefaultDialogueContinue(true);
        SignalBus.Fire(new Signal_DialogueToggled(nodeName, true));
        await _dialogueRunner.StartDialogue(nodeName);
        SignalBus.Fire(new Signal_DialogueToggled(nodeName, false));
    }

    [YarnCommand("SetDefaultContinue")] public static void ToggleDefaultDialogueContinue(bool on)
    {
        IsDefaultDialogueContinueActive = on;
        SignalBus.Fire(new Signal_DialogueDefaultContinueToggled(IsDefaultDialogueContinueActive));
    }
    public void ContinueDialogue()
    {
        if(_dialogueRunner.IsDialogueRunning)
            _dialogueRunner.RequestNextLine();
    }

    public void ForceContinueDialogue(Signal_DialogueForceContinue signal) => ContinueDialogue();
    public void InputDefaultContinueDialogue(InputAction.CallbackContext ctx)
    {
        if(IsDefaultDialogueContinueActive && IsResponsive)
            ContinueDialogue();
    }
}
