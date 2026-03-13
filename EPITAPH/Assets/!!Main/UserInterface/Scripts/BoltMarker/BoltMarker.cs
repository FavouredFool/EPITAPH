using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class BoltMarker : MonoBehaviour
{
    RectTransform rect;
    [SerializeField]Canvas canvas;
    public Transform Parent {get; set;}
    [SerializeField] GameObject _basicVisuals, _dashVisuals, _feedVisuals;

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

    void Update()
    {
        if (Parent != null)
        {
        var ViewportPos = Camera.main.WorldToViewportPoint(Parent.transform.position);
        rect.anchoredPosition = new Vector2(Screen.width * ViewportPos.x, Screen.height * ViewportPos.y);
//              Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, Parent.transform.position);

// // convert the screen position to the local anchored position

// Vector2 anchoredPosition = transform.InverseTransformPoint(screenPoint); 
// rect.position=anchoredPosition;
// return;

//             // var screen = Camera.main.WorldToScreenPoint(Parent.transform.position);
//             // screen.z = (canvas.transform.position -);
//             // RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, , Camera.main, out Vector2 canvasPos);
//             // rect.position = canvasPos;
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
        
        DOTween.Kill(gameObject);
        Sequence seq = DOTween.Sequence(gameObject);
        seq.Insert(0, transform.DOScale(1,0.3f).SetEase(Ease.OutBack));
    }

    public void TweenTrigger()
    {
        DOTween.Kill(gameObject);
        Sequence seq = DOTween.Sequence(gameObject);
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
}
