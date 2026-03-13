using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeatPuncher : BeatResponderBase
{
    [SerializeField] Transform rect;

    public float strength=0.05f;
    Vector3 originalScale;

    protected override void Awake()
    {
        base.Awake();
originalScale=rect.localScale;
    }

    public override void BeatTrigger(EarlyBeatChanged e)
    {
            DOTween.Kill(this,true);
            Sequence seq = DOTween.Sequence(this);
            seq.SetUpdate(true);

        seq.OnPlay(() =>
        {
            rect.localScale=originalScale;
        });
  
            seq.Insert(0,rect.DOPunchScale(Vector3.one * strength,0.2f,1).SetEase(Ease.OutSine));
    }

}