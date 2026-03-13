using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class UserInterfaceController : MonoBehaviour
{
    InputActions _inputActions;

    [SerializeField]YarnManager _yarnManager;

    [SerializeField]MenuUIDManager _menuManager;
    [SerializeField]DialogueUIDManager _dialogueManager;

    public bool IsAnyOpen => _dialogueManager.IsOpen || _menuManager.IsOpen;

    void OnEnable()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        _inputActions.Menus.Pause.performed += EnterPause;
        _inputActions.Menus.DialogueContinue.performed += _yarnManager.InputDefaultContinueDialogue;

        SignalBus.Subscribe<Signal_MenuUIToggled>(MenuUIToggled);
        SignalBus.Subscribe<Signal_DialogueUIToggled>(DialogueUIToggled);
    }
    void OnDisable()
    {
        _inputActions.Menus.Pause.performed -= EnterPause;
        _inputActions.Menus.DialogueContinue.performed -= _yarnManager.InputDefaultContinueDialogue;

        SignalBus.Unsubscribe<Signal_MenuUIToggled>(MenuUIToggled);
        SignalBus.Unsubscribe<Signal_DialogueUIToggled>(DialogueUIToggled);
    }

    public void EnterPause(InputAction.CallbackContext ctx)
    {
        if (!_menuManager.IsOpen)
            _menuManager.Toggle(true);

        //The menu must close itself.
    }

    public void MenuUIToggled(Signal_MenuUIToggled signal)
    {
        RefreshTimescale();
    }
    public void DialogueUIToggled(Signal_DialogueUIToggled signal)
    {
        RefreshTimescale();
    }

    public void RefreshTimescale()
    {
        Time.timeScale = _menuManager.IsOpen ? 0 : 1;
    }

    [YarnCommand("ToggleFreeze")]public static void TogglePauseDialogue(bool on)
    {
    }
}