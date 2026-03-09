using Unity.VisualScripting;
using UnityEngine;

public class AmmoUID : MonoBehaviour
{
     [SerializeField] PlayerVariables _playerVariables;
    [SerializeField] AmmoMarker[] _ammoMarkers;

    public void RefreshUID()
    {
        float value= _playerVariables.Ammo;
        float max= _playerVariables.AmmoMax;

        for(int i=0; i<_ammoMarkers.Length; i++)
        {
            AmmoMarker marker= _ammoMarkers[i];
            marker.RefreshGUI(i<value, i<max);
        }
    }
}
