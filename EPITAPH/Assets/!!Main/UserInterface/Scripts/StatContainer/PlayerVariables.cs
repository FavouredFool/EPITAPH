using UnityEngine;

[CreateAssetMenu(fileName = "PlayerVariables", menuName = "Scriptable Objects/PlayerVariables")]
public class PlayerVariables : ScriptableObject
{
    [Header("Health")]
    [SerializeField] int _health;
    [SerializeField] int _healthMax;

[Header("Ammo")]
    [SerializeField] int _ammo;
    [SerializeField] int _ammoMax;

[Header("Charge")]
    [SerializeField] float _charge;
    [SerializeField] float _chargeMax;

    public int Health
    {
        get=> _health;
        set
        {
            _health=Mathf.Clamp(value,0,_healthMax);
            SignalBus.Fire(new Signal_RefreshUI_Health(this));
        }
    }
    public float Charge
    {
        get=> _charge;
        set
        {
            _charge=Mathf.Clamp(value,0,_chargeMax);
            SignalBus.Fire(new Signal_RefreshUI_Charge(this));
        }
    }
    public int Ammo
    {
        get=> _ammo;
        set
        {
            _ammo=Mathf.Clamp(value,0,_ammoMax);
            SignalBus.Fire(new Signal_RefreshUI_Ammo(this));
        }
    }
    
    public int HealthMax=>_healthMax;
    public float ChargeMax=>_chargeMax;
    public int AmmoMax=>_ammoMax;

    public void HealMax() => Heal(HealthMax);
    public void Heal(int value)
    {
        Health= Mathf.Clamp(Health+value,0,HealthMax);
    }

    //Debug
    [ContextMenu("Damage health Test")]
    public void DamageTest()
    {
        Health -= 1;
    }
    [ContextMenu("Heal Health Test")] public void HealTest() => HealMax();

    [ContextMenu("Shoot Ammo Test")]
    public void ShootTest()
    {
        Ammo -= 1;
    }
    [ContextMenu("Retrieve Ammo Test")]
    public void RetrieveTest()
    {
        Ammo += 1;
    }
}
