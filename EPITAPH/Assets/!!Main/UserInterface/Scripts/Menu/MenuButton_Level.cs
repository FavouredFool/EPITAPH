using UnityEngine;
using TMPro;
using DG.Tweening;

public class MenuButton_Level : MenuButton
{
    [SerializeField] TMP_Text _levelText;
    [SerializeField] string _variableName;

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshVariable();
    }
    public void RefreshVariable()
    {
        int level= PlayerPrefs.GetInt(_variableName,0);
        _levelText.text=level.ToString();

        Debug.Log(_variableName+" is "+level);
        
        DOTween.Kill(this,true);
        _levelText.transform.DOPunchScale(Vector3.one*0.1f,0.25f,1).SetEase(Ease.OutSine);
    }
}
