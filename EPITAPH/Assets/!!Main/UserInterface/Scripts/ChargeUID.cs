using UnityEngine;
using UnityEngine.UI;

public class ChargeUID : MonoBehaviour
{
    [SerializeField] PlayerVariables _playerVariables;
    [SerializeField] Image _ChargeUiBar;

    public void RefreshUID()
    {
        float value= _playerVariables.Charge/_playerVariables.ChargeMax;

        _ChargeUiBar.fillAmount=value;
    }
}
