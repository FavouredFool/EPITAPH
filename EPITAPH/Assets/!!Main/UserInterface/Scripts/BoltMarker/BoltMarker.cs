using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class BoltMarker : MonoBehaviour
{
    RectTransform rect;
    public Transform Parent {get; set;}
    [SerializeField] GameObject _basicVisuals, _dashVisuals, _feedVisuals;

    public Canvas Canvas { get; set; }
    
    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetSleep()
    {
        transform.localScale=Vector3.zero;

        _basicVisuals.SetActive(false);
        _dashVisuals.SetActive(false);
        _feedVisuals.SetActive(false);

        gameObject.SetActive(false);

    }

    RectTransform _rectTransform;

    void Start()
    {
        _rectTransform = Canvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Parent != null)
        {
            var viewportPos = Camera.main.WorldToViewportPoint(Parent.transform.position);
            
            rect.anchoredPosition = new Vector2(_rectTransform.rect.width * viewportPos.x - _rectTransform.rect.width / 2, _rectTransform.rect.height * viewportPos.y - _rectTransform.rect.height / 2);
            
        }
    }

    public void TweenAppear(Transform parent, bool dash, bool feed)
    {
        gameObject.SetActive(true);
        this.Parent=parent;
        transform.localPosition = Vector3.zero;

        _dashVisuals.SetActive(dash);
        _feedVisuals.SetActive(feed);
        _basicVisuals.SetActive(!dash&&!feed);
        
        DOTween.Kill(gameObject, true);
        Sequence seq = DOTween.Sequence(gameObject);
        seq.SetUpdate(true);
        seq.Insert(0, transform.DOScale(1,0.3f).SetEase(Ease.OutBack));
    }

    public void TweenTrigger()
    {
        DOTween.Kill(gameObject,true);
        Sequence seq = DOTween.Sequence(gameObject);
                seq.SetUpdate(true);
        seq.Insert(0, transform.DOScale(0,0.3f).SetEase(Ease.InBack));
        seq.OnComplete(()=>        Destroy(gameObject));
    }

    void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
    void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }

    void OnDrawGizmos()
    {
        
    }
}
