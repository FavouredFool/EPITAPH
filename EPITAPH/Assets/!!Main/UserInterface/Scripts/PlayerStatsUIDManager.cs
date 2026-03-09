using UnityEngine;

public class PlayerStatsUIDManager : MonoBehaviour
{
    [SerializeField] HealthUID _healthUID;
    [SerializeField] AmmoUID _ammoUID;
    [SerializeField] ChargeUID _chargeUID;

    //TODO: Replace this entire script with something better.

    void Start()
    {
        RefreshUID();
    }

    [ContextMenu("Refresh")]
    public void RefreshUID()
    {
        _healthUID.RefreshUID();
        _ammoUID.RefreshUID();
        _chargeUID.RefreshUID();
    }
}
