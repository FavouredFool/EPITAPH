using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuUIDManager : MonoBehaviour
{
    [SerializeField] GameObject
        _menuObject;
    
    [SerializeField] RectTransform
    _bodyRect,_glowRect;

    [SerializeField] MenuScreen
        _mainMenu,
        _optionMenu,
        _audioOptionMenu;

    MenuScreen _currentMenu=null;

    public bool IsOpen { get; set; }

    void Awake()
    {
        _currentMenu=null;
        IsOpen=false;

        _bodyRect.anchoredPosition = new Vector2(-4585, 0);
        _glowRect.anchoredPosition = new Vector2(-6000, 0);

        _menuObject.SetActive(false);
    }

    public void OpenMenu(MenuScreen nextMenu)
    {
        if(nextMenu==null)
        {
            Toggle(false);
            return;
        }
        if(_currentMenu == nextMenu) return;

        _currentMenu?.Toggle(false);
        _currentMenu = nextMenu;
        _currentMenu.Toggle(true);

        Toggle(true);
    }

    public void Toggle(bool on)
    {
        if (IsOpen == on) return;

        IsOpen = on;
        _menuObject.SetActive(IsOpen);

        DOTween.Kill(this,true);
        Sequence seq= DOTween.Sequence(this);

        if (IsOpen)
        {
            if(_currentMenu == null)
                OpenMenu(_mainMenu);
            
            seq.Insert(0,_bodyRect.DOAnchorPosX(-585,1).SetEase(Ease.OutCirc));
            seq.Insert(0,_glowRect.DOAnchorPosX(-2000,1.25f).SetEase(Ease.OutCirc));
        }
        else
        {
            _currentMenu.Toggle(false);
            _currentMenu = null;

            seq.Insert(0,_bodyRect.DOAnchorPosX(-4585,0.75f).SetEase(Ease.InSine));
            seq.Insert(0,_glowRect.DOAnchorPosX(-6000,0.75f).SetEase(Ease.InSine));
        }
    }

    public void StartGame()
    {
        Debug.Log("START GAME");
    }
    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
    }
    public void QuitApplication()
    {
        #if UNITY_EDITOR
                Debug.Log("QUIT PLAYMODE");
        UnityEditor.EditorApplication.isPlaying = false;
        #else
                Debug.Log("QUIT APPLICATION");
        Application.Quit();
        #endif
    }

    public void IncrimentVolume(string variableName = "VolumeMaster")
    {
        int volume= PlayerPrefs.GetInt(variableName,0);

        volume++;
        if(volume>=5) volume = 0;

        PlayerPrefs.SetInt(variableName, volume);
        SignalBus.Fire(new Signal_RefreshVolume(variableName));
    }
}
