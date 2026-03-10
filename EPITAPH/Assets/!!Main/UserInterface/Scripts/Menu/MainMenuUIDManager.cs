using UnityEngine;

public class MainMenuUIDManager : MonoBehaviour
{
    public MenuUIDManager _menu;

    void Start()
    {
        _menu.Toggle(true);
    }
}
