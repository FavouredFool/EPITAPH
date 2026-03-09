using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AmmoMarker : MonoBehaviour, AudioEventSubscriber<BeatChanged>
{
    [SerializeField] Image _background;
    [SerializeField] RectTransform _b_rect;

    [SerializeField] float _tweenDuration=2.5f;
    bool lastState=false, initiated;
    int siblingIndex, siblingCount;

    [ContextMenu("Hit")]public void Hit() => RefreshGUI(false);
    [ContextMenu("Heal")]public void Heal() => RefreshGUI(true);
    public void RefreshGUI(bool filled, bool active = true)
    {
        gameObject.SetActive(active);
        if (!active)
        {
            lastState=false;
            DOTween.Kill(this);
            return;
        }

        if (filled != lastState || !initiated)
        {
            initiated=true;

            DOTween.Kill(this,true);
            Sequence seq = DOTween.Sequence(this);
            seq.SetUpdate(true);
            if (filled)
            {
                float time= _tweenDuration;

                seq.Insert(0,_b_rect.DOAnchorPosY(0,time).SetEase(Ease.OutSine));
                seq.Insert(0,_background.DOColor(CustomColor.White,time).SetEase(Ease.OutCirc));
            }
            else
            {
                float time= _tweenDuration;
                
                seq.Insert(0,_b_rect.DOAnchorPosY(-110,time).SetEase(Ease.OutBounce));
                seq.Insert(0,_background.DOColor(CustomColor.OldBlood,time).SetEase(Ease.OutCirc));
            }
        }

        lastState=filled;
    }

     void Awake()
    {
        siblingIndex= transform.GetSiblingIndex();
        siblingCount= transform.parent.childCount;

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
        if ((e.beat%siblingCount)==siblingCount-1-siblingIndex)
        {
            DOTween.Kill(this,true);
            Sequence seq = DOTween.Sequence(this);
            seq.SetUpdate(true);

            float pos= lastState? 0 : -110;
  
            seq.Insert(0,_b_rect.DOAnchorPosY(pos+15,0.1f).SetEase(Ease.OutSine));
            seq.Append(_b_rect.DOAnchorPosY(pos,0.3f)).SetEase(Ease.InOutSine);
        }
    }
}
