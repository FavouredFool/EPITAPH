using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    [SerializeField] List<Button> _buttons;

    public void Toggle(bool on)
    {
        gameObject.SetActive(on);

        if (on)
        {
            _buttons[0].Select();
        }
    }
}
