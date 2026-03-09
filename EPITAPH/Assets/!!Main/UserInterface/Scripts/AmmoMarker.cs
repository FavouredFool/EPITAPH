using UnityEngine;
using UnityEngine.UI;

public class AmmoMarker : MonoBehaviour
{
    [SerializeField] Image _background;

    public void RefreshGUI(bool filled, bool active = true)
    {
        gameObject.SetActive(active);
        if(!active) return;

        Color col;

        if(filled)
            col=CustomColor.HotBlood;
        else
            col= CustomColor.OldBlood;
        
        _background.color = filled? CustomColor.HotBlood: CustomColor.OldBlood;
    }
}
