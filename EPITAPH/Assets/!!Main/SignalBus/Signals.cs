using UnityEngine;

public struct Signal_RefreshUI_Health
{
    public PlayerVariables variables;
    public Signal_RefreshUI_Health(PlayerVariables variables)
    {
        this.variables = variables;
    }
}

public struct Signal_RefreshUI_Ammo
{
    public PlayerVariables variables;
    public Signal_RefreshUI_Ammo(PlayerVariables variables)
    {
        this.variables = variables;
    }
}

public struct Signal_RefreshUI_Charge
{
    public PlayerVariables variables;
    public Signal_RefreshUI_Charge(PlayerVariables variables)
    {
        this.variables = variables;
    }
}
public struct Signal_RefreshUI_ChargeProgress
{
    public PlayerVariables variables;
    public Signal_RefreshUI_ChargeProgress(PlayerVariables variables)
    {
        this.variables = variables;
    }
}


public struct Signal_ShowBoltMarker
{
    public Transform parent;
    public BoltType type;
    public bool dash, feed;
    public Signal_ShowBoltMarker(Transform parent, BoltType type, bool dash, bool feed)
    {
        this.parent = parent;
        this.type = type;
        this.dash = dash;
        this.feed = feed;
    }
}
public struct Signal_TriggerBoltMarker
{
    public BoltType type;
    public Signal_TriggerBoltMarker(BoltType type)
    {
        this.type = type;
    }
}
public struct Hit_Enemy
{
    public Vector3 EnemyPosition;
    public Hit_Enemy(Vector3 position)
    {
        EnemyPosition = position;
    }
}
public struct Hit_Player
{
    public Transform PlayerTransform;
    public Hit_Player(Transform PlayerTransform)
    {
        this.PlayerTransform = PlayerTransform;
    }
}

public struct Signal_RefreshVolume
{
    public string variableName;
    public Signal_RefreshVolume(string varName)
    {
        variableName=varName;
        Debug.Log(varName+" is "+PlayerPrefs.GetInt(varName));
    }
}

public struct Signal_DialogueToggled
{
    public bool on;
    public Signal_DialogueToggled(bool on)
    {
        this.on= on;
    }
}

public struct Signal_MenuUIToggled
{
    public bool on;
    public Signal_MenuUIToggled(bool on)
    {
        this.on= on;
    }
}
public struct Signal_DialogueUIToggled
{
    public bool on;
    public Signal_DialogueUIToggled(bool on)
    {
        this.on= on;
    }
}

public struct Signal_DialogueDefaultContinueToggled
{
    public bool on;
    public Signal_DialogueDefaultContinueToggled(bool on)
    {
        this.on= on;
    }
}
public struct Signal_DialogueForceContinue
{
}

public struct Signal_StartDialogue
{
    public string nodeName;

    public Signal_StartDialogue(string nodeName)
    {
        this.nodeName=nodeName;
    }
}

public struct Signal_EnemyDeath
{
    public EnemyController enemy;

    public Signal_EnemyDeath(EnemyController enemy)
    {
        this.enemy=enemy;
    }
}