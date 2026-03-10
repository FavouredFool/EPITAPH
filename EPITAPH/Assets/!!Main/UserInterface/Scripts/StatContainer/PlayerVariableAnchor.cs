using UnityEngine;

public class PlayerVariableAnchor : MonoBehaviour
{
    public static PlayerVariableAnchor ME {get; private set;}

    [SerializeField] PlayerVariables playerVariables;
    public static PlayerVariables PlayerVariables => ME.playerVariables;

    void Awake()
    {
        if(ME != null)
        {
            Destroy(this);
            return;
        }

        ME = this;
        DontDestroyOnLoad(ME);

        playerVariables = Instantiate(playerVariables);
    }

    void Start()
    {
            SignalBus.Fire(new Signal_RefreshUI_Health(playerVariables));
            SignalBus.Fire(new Signal_RefreshUI_Ammo(playerVariables));
            SignalBus.Fire(new Signal_RefreshUI_Charge(playerVariables));

    }


}
