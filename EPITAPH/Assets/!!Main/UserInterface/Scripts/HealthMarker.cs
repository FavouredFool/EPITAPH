using UnityEngine;
using UnityEngine.UI;

public class HealthMarker : MonoBehaviour
{
    [SerializeField] Image _background;

    public void RefreshGUI(bool filled, bool active = true)
    {
        gameObject.SetActive(active);
        if(!active) return;

        _background.color = filled? CustomColor.HotBlood: CustomColor.OldBlood;
    }
}
