using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeatHueShifter : BeatResponderBase
{
    [SerializeField] Graphic _graphic;

    public Color[] hues;


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

            Color col = hues[e.beat % hues.Length];
  
            seq.Insert(0,_graphic.DOColor(col,0.2f).SetEase(Ease.OutCirc));
    }

}