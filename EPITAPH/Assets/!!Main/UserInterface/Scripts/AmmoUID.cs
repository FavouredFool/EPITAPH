using Unity.VisualScripting;
using UnityEngine;

public class AmmoUID : MonoBehaviour
{
    [SerializeField] AmmoMarker[] _ammoMarkers;

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_RefreshUI_Ammo>(RefreshUID);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_RefreshUI_Ammo>(RefreshUID);
    }

    public void RefreshUID(Signal_RefreshUI_Ammo signal)
    {
        float value= signal.variables.CurrentAmmoCount;
        float max= signal.variables.CurrentBoltsHeld.Count;

        for(int i=0; i<_ammoMarkers.Length; i++)
        {
            AmmoMarker marker= _ammoMarkers[i];
            marker.RefreshGUI(i<value, i<max);
        }
    }
}
