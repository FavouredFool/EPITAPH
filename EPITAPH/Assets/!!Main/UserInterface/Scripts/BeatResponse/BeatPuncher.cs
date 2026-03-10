using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeatPuncher : BeatResponderBase
{
    [SerializeField] Transform rect;

    public float strength=0.05f;


    protected override void OnDisable()
    {
        DOTween.Kill(this);
        base.OnDisable();
    }

    public override void BeatTrigger(BeatChanged e)
    {
            DOTween.Kill(this,true);
            Sequence seq = DOTween.Sequence(this);
            seq.SetUpdate(true);
  
            seq.Insert(0,rect.DOPunchScale(Vector3.one * strength,0.2f,1).SetEase(Ease.OutSine));
    }

}