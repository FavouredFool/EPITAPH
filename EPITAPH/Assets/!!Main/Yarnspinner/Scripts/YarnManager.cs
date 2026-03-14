using DG.Tweening;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;

public class YarnManager : MonoBehaviour
{
    [SerializeField] DialogueRunner _dialogueRunner;

    public static bool IsDefaultDialogueContinueActive { get; set; }

    void OnEnable()
    {
        _dialogueRunner.onDialogueComplete.AddListener(OnEndDialogue);
        SignalBus.Subscribe<Signal_DialogueForceContinue>(ForceContinueDialogue);
        SignalBus.Subscribe<Signal_StartDialogue>(SpinYarn);
    }
    void OnDisable()
    {
        _dialogueRunner.onDialogueComplete.RemoveListener(OnEndDialogue);
        SignalBus.Unsubscribe<Signal_DialogueForceContinue>(ForceContinueDialogue);
        SignalBus.Unsubscribe<Signal_StartDialogue>(SpinYarn);
    }

    public void SpinYarn(Signal_StartDialogue signal) => SpinYarn(signal.nodeName);
    public async void SpinYarn(string nodeName)
    {
        if (_dialogueRunner.IsDialogueRunning)
        {
            await _dialogueRunner.Stop();
            Debug.LogWarning("Dialogue started while it was already running!");
            
        }
        ToggleDefaultDialogueContinue(true);
        SignalBus.Fire(new Signal_DialogueToggled( true));
        await _dialogueRunner.StartDialogue(nodeName);
    }
    public void OnEndDialogue()
    {
        SignalBus.Fire(new Signal_ToggleFreeze(false));
        SignalBus.Fire(new Signal_DialogueToggled(false));
    }

    [YarnCommand("SetDefaultContinue")] public static void ToggleDefaultDialogueContinue(bool on)
    {
        IsDefaultDialogueContinueActive = on;
        SignalBus.Fire(new Signal_DialogueDefaultContinueToggled(IsDefaultDialogueContinueActive));
    }
    [YarnCommand("DoNothing")] public static void DoNothing(){}
    public void ContinueDialogue()
    {
        if(_dialogueRunner.IsDialogueRunning)
            _dialogueRunner.RequestNextLine();
    }

    public void ForceContinueDialogue(Signal_DialogueForceContinue signal) => ContinueDialogue();
    public void InputDefaultContinueDialogue(InputAction.CallbackContext ctx)
    {
        if(IsDefaultDialogueContinueActive)
            ContinueDialogue();
    }
}
