using UnityEngine;

public class HealthUID : MonoBehaviour
{
    [SerializeField] PlayerVariables _playerVariables;
    [SerializeField] HealthMarker[] _healthMarkers;

    public void RefreshUID()
    {
        float value= _playerVariables.Health;
        float max= _playerVariables.HealthMax;

        for(int i=0; i<_healthMarkers.Length; i++)
        {
            HealthMarker marker= _healthMarkers[i];
            marker.RefreshGUI(i<value, i<max);
        }
    }
}
