using Unity.VisualScripting;
using UnityEngine;

public class AmmoUID : MonoBehaviour
{
     [SerializeField] PlayerVariables _playerVariables;
    [SerializeField] AmmoMarker[] _ammoMarkers;

    [SerializeField] int _spacing;

    public void RefreshUID()
    {
        float value= _playerVariables.Ammo;
        float max= _playerVariables.AmmoMax;

        for(int i=0; i<_ammoMarkers.Length; i++)
        {
            AmmoMarker marker= _ammoMarkers[i];
            marker.RefreshGUI(i<value, i<max);

            marker.transform.localEulerAngles= new Vector3(0,0,_spacing*-i);
        }
    }
}
