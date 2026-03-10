using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public GameObject _selectionMarker;

    protected virtual void OnEnable()
    {
        _selectionMarker.SetActive(false);
    }
}
