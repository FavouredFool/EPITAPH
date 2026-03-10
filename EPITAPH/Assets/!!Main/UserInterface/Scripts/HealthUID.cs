using UnityEngine;

public class HealthUID : MonoBehaviour
{
    [SerializeField] HealthMarker[] _healthMarkers;

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_RefreshUI_Health>(RefreshUID);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_RefreshUI_Health>(RefreshUID);
    }

    public void RefreshUID(Signal_RefreshUI_Health signal)    
    {
        float value= signal.variables.Health;
        float max= signal.variables.HealthMax;

        for(int i=0; i<_healthMarkers.Length; i++)
        {
            HealthMarker marker= _healthMarkers[i];
            marker.RefreshGUI(i<value, i<max);
        }
    }
}
