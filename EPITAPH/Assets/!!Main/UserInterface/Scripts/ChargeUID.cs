using UnityEngine;
using UnityEngine.UI;

public class ChargeUID : MonoBehaviour
{
    [SerializeField] PlayerVariables _playerVariables;
    [SerializeField] Image[] _ChargeBars;

    public void RefreshUID()
    {
        float value= _playerVariables.Charge;

        for(int i =0; i<_ChargeBars.Length; i++)
        {
            float fill= Mathf.Clamp01(value-i);
            _ChargeBars[i].fillAmount=fill;
        }
        
    }
}
