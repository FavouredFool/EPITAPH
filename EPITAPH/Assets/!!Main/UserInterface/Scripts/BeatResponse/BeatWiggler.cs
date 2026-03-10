using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeatWiggler : BeatResponderBase
{
    [SerializeField] Transform rect;

    public float strength=5f;

    Quaternion baseRotation;

    protected override void Awake()
    {
        baseRotation=transform.rotation;
    }

    public override void BeatTrigger(EarlyBeatChanged e)
    {
        Sequence seq = DOTween.Sequence(this);
        seq.SetUpdate(true);

        seq.OnPlay(() =>
        {
            rect.rotation=baseRotation;
        });

        seq.Insert(0,rect.DOPunchRotation(Vector3.one * strength,0.2f,1).SetEase(Ease.OutSine));
    }

}