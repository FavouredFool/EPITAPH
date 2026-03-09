using UnityEngine;
using UnityEngine.UI;

public class HealthMarker : MonoBehaviour
{
    [SerializeField] Image _background,_filled;

    public void RefreshGUI(bool filled, bool active = true)
    {
        gameObject.SetActive(active);
        if(!active) return;

        _filled.gameObject.SetActive(filled);
        _background.color = filled? CustomColor.White: CustomColor.OldBlood;
    }
}
