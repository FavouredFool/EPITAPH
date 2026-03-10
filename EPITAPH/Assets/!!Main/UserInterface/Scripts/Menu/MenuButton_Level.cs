using UnityEngine;
using TMPro;
using DG.Tweening;

public class MenuButton_Level : MenuButton
{
    [SerializeField] TMP_Text _levelText;

    public void RefreshLevel(int level)
    {
        _levelText.text=level.ToString();
        
        DOTween.Kill(this,true);
        _levelText.transform.DOPunchScale(Vector3.one*0.1f,0.25f,1).SetEase(Ease.OutSine);
    }
}
