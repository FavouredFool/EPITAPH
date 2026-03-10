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
