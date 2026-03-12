using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(BeatResponderGroup))]
public class AmmoMarker : MonoBehaviour, AudioEventSubscriber<EarlyBeatChanged>
{
    [SerializeField] Image _background;
    [SerializeField] RectTransform _b_rect;

    bool lastState=false, initiated;
    int siblingIndex, siblingCount;
    BeatResponderGroup _beatGroup;

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
                seq.Insert(0,_b_rect.DOAnchorPosY(0,0.66f).SetEase(Ease.OutSine));
                seq.Insert(0,_background.DOColor(CustomColor.White,0.66f).SetEase(Ease.OutCirc));
            }
            else
            {                
                seq.Insert(0,_b_rect.DOAnchorPosY(-110,0.66f).SetEase(Ease.OutBounce));
                seq.Insert(0,_background.DOColor(CustomColor.OldBlood,0.66f).SetEase(Ease.OutCirc));
            }
        }

        lastState=filled;
    }

     void Awake()
    {
        siblingIndex= transform.GetSiblingIndex();
        siblingCount= transform.parent.childCount;

        AudioBus.Subscribe(this);

        _beatGroup = GetComponent<BeatResponderGroup>();
    }

    private void OnDisable()
    {
        lastState=false;
        DOTween.Kill(this);
        AudioBus.Unsubscribe(this);
    }

    public void OnEventHappened(EarlyBeatChanged e)
    {
        if ((e.beat%siblingCount)==siblingCount-1-siblingIndex|| !initiated)
        {
            initiated = true;

            DOTween.Kill(this,true);
                        _beatGroup.Halt();

            Sequence seq = DOTween.Sequence(this);
            seq.SetUpdate(true);

            float pos= lastState? 0 : -110;
  
            seq.Insert(0,_b_rect.DOAnchorPosY(pos+15,0.2f).SetEase(Ease.OutBack));
            seq.Append(_b_rect.DOAnchorPosY(pos,0.3f)).SetEase(Ease.OutCirc);

        }
    }
}
