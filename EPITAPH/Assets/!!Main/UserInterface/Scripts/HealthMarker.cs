using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class HealthMarker : MonoBehaviour, AudioEventSubscriber<BeatChanged>
{
    [SerializeField] Image _background, _fill;

    [SerializeField] RectTransform _b_rect, _f_rect;

    [SerializeField] float _tweenDuration=2;
    bool lastState=false, initiated;

    [ContextMenu("Hit")]public void Hit() => RefreshGUI(false);
    [ContextMenu("Heal")]public void Heal() => RefreshGUI(true);
    public void RefreshGUI(bool filled, bool active = true)
    {
        gameObject.SetActive(active);
        if (!active)
            return;

        if (filled != lastState || !initiated)
        {
            initiated=true;
            DOTween.Kill(this,true);
            Sequence seq = DOTween.Sequence(this);
            seq.SetUpdate(true);
            if (filled)
            {
                float time= _tweenDuration;

                seq.Insert(0,_b_rect.DOPunchScale(Vector3.one * 0.1f,time,1));
                seq.Insert(0,_f_rect.DOAnchorPosY(15,time).SetEase(Ease.OutBack));
                seq.Insert(0,_fill.DOColor(CustomColor.HotBlood,time).SetEase(Ease.OutCirc));
            }
            else
            {
                float time= _tweenDuration;
                
                _fill.color=CustomColor.BadBlood;

                seq.Insert(0,_b_rect.DOPunchScale(Vector3.one * 0.1f,time,10));
                seq.Insert(0,_f_rect.DOAnchorPosY(0,time).SetEase(Ease.InBack));
                seq.Insert(0,_fill.DOColor(CustomColor.OldBlood,time).SetEase(Ease.InBack));
            }
        }

        lastState=filled;
    }

    void Awake()
    {
        AudioBus.Subscribe(this);
    }

    private void OnDisable()
    {
        lastState=false;
        DOTween.Kill(this);
        AudioBus.Unsubscribe(this);
    }

    public void OnEventHappened(BeatChanged e)
    {
        if (lastState)
        {
            DOTween.Kill(this,true);
            Sequence seq = DOTween.Sequence(this);
            seq.SetUpdate(true);
  
            seq.Insert(0,_b_rect.DOPunchScale(Vector3.one * 0.05f,0.2f,1).SetEase(Ease.OutSine));
        }
    }

}
