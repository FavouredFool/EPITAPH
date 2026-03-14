using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuUIDManager : MonoBehaviour
{
    [SerializeField] GameObject
        _menuObject;
    
    [SerializeField] RectTransform
    _bodyRect,_glowRect;

    [SerializeField] MenuScreen
        _mainMenu;

        [SerializeField] MenuScreen[] _allMenus;

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
        if (_currentMenu == nextMenu) return;

        _currentMenu = nextMenu;
        foreach (MenuScreen screen in _allMenus)
        {
            screen.Toggle(screen == _currentMenu);
        }

        Toggle(true);
    }

    public void Toggle() => Toggle(!IsOpen);
    public void Toggle(bool on)
    {
        if (IsOpen == on) return;

        IsOpen = on;
        _menuObject.SetActive(IsOpen);

        DOTween.Kill(this);
        Sequence seq= DOTween.Sequence(this);
        seq.SetUpdate(true);

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

        SignalBus.Fire(new Signal_MenuUIToggled(IsOpen));
    }

    public void StartGame()
    {
        Debug.Log("START GAME");
        SceneManager.LoadScene("0_Tutorial");
    }
    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        SceneManager.LoadScene("_MainMenu");
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

    public void LoadLevel(string levelName)
    {
        Debug.Log("LOAD SCENE "+levelName);
        SceneManager.LoadScene(levelName);
    }

    public void IncrimentVolume(string variableName = "VolumeMaster")
    {
        int volume= PlayerPrefs.GetInt(variableName,0);
        
        volume++;
        if(volume>=5) volume = 0;

        PlayerPrefs.SetInt(variableName, volume);
        AudioManager.SetVolumeLevel(variableName, volume);
    }
}
