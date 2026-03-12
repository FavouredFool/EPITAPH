using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BeatResponderGroup))]
public class HealthMarker : MonoBehaviour, AudioEventSubscriber<EarlyBeatChanged>
{
    [SerializeField] Image _background, _fill;

    [SerializeField] RectTransform _b_rect, _f_rect;

    bool lastState=false, initiated;
    BeatResponderGroup _beatGroup;


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
            _beatGroup.Toggle(filled);
            Sequence seq = DOTween.Sequence(this);
            seq.SetUpdate(true);
            if (filled)
            {
                float time= 0.5f;

                seq.Insert(0,_b_rect.DOPunchScale(Vector3.one * 0.1f,time,1));
                seq.Insert(0,_f_rect.DOAnchorPosY(15,time).SetEase(Ease.OutBack));
                seq.Insert(0,_fill.DOColor(CustomColor.HotBlood,time).SetEase(Ease.OutCirc));
            }
            else
            {
                float time= 1f;
                
                seq.OnPlay(() =>
                {
                    _fill.color=CustomColor.BadBlood;
                });

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
        if (lastState)
        {
            DOTween.Kill(this, true);            
            Sequence seq = DOTween.Sequence(this);
            seq.SetUpdate(true);
  
            seq.Insert(0,_b_rect.DOPunchScale(Vector3.one * 0.05f,0.2f,1).SetEase(Ease.OutSine));
        }
    }

}
