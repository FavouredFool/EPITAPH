using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenUID : MonoBehaviour
{
    public RectTransform _rect;
    public Button _button;
    public void Revive()
    {
        SceneManager.LoadScene(ProgressionVariableAnchor.ProgressionVariables.LevelName);
    }
    public void Quit()
    {
        SceneManager.LoadScene("_MainMenu");
    }

    void Awake()
    {
        _rect.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_PlayerDeath>(Show);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_PlayerDeath>(Show);
    }

    public void Show(Signal_PlayerDeath signal)
    {
        _rect.gameObject.SetActive(true);

        _button.Select();
    }
}
