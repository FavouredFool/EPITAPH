using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeatHueShifter : BeatResponderBase
{
    [SerializeField] Graphic _graphic;

    public Color[] hues;
    public bool punchy;

    public override void BeatTrigger(EarlyBeatChanged e)
    {
        Sequence seq = DOTween.Sequence(this);
        seq.SetUpdate(true);

        if (punchy)
        {
            seq.Insert(0, _graphic.DOColor(hues[1], 0.2f).SetEase(Ease.OutCirc));
            seq.Insert(0, _graphic.DOColor(hues[0], 0.3f).SetEase(Ease.InOutSine));
        }
        else
        {
            Color col = hues[e.beat % hues.Length];
            seq.Insert(0, _graphic.DOColor(col, 0.2f).SetEase(Ease.OutCirc));
        }
    }

}