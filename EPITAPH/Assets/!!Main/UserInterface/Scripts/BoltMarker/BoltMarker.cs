using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class BoltMarker : MonoBehaviour
{
    public BoltType type;
    [SerializeField] GameObject _basicVisuals, _dashVisuals, _feedVisuals;
    [SerializeField] Image _typeImage;
    [SerializeField] List<Sprite> _typePrompts;


    public void SetSleep()
    {
        transform.localScale=Vector3.zero;

        _basicVisuals.SetActive(false);
        _dashVisuals.SetActive(false);
        _feedVisuals.SetActive(false);

        gameObject.SetActive(false);
    }

    public void TweenAppear(Transform parent, BoltType type, bool dash, bool feed)
    {
        gameObject.SetActive(true);
        transform.SetParent(parent);
        transform.localPosition=Vector3.zero;

        this.type=type;
        _dashVisuals.SetActive(dash);
        _feedVisuals.SetActive(feed);
        _basicVisuals.SetActive(!dash&&!feed);

        _typeImage.sprite=_typePrompts[(int)type];

        DOTween.Kill(gameObject);
        Sequence seq = DOTween.Sequence(gameObject);
        seq.Insert(0, transform.DOScale(1,1f).SetEase(Ease.OutBack));
    }

    public void TweenTrigger()
    {
        DOTween.Kill(gameObject);
        Sequence seq = DOTween.Sequence(gameObject);
        seq.Insert(0, transform.DOScale(0,1f).SetEase(Ease.InBack));
        seq.OnComplete(()=>SetSleep());
    }

    void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
}
