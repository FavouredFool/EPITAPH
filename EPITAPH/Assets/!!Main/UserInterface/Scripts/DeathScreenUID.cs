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
        _rect.anchoredPosition = new Vector2(6500, 0);
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
        DOTween.Kill(this, true);
        Sequence seq = DOTween.Sequence(this).SetUpdate(true);

        seq.Insert(0, _rect.DOAnchorPosX(0, 2).SetEase(Ease.OutCirc));
        seq.SetDelay(1);

        _button.Select();
    }
}
