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
        }
    }
    public float Charge
    {
        get=> _charge;
        set
        {
            _charge=Mathf.Clamp(value,0,_chargeMax);
        }
    }
    public int Ammo
    {
        get=> _ammo;
        set
        {
            _ammo=Mathf.Clamp(value,0,_ammoMax);
        }
    }
    
    public int HealthMax=>_healthMax;
    public float ChargeMax=>_chargeMax;
    public int AmmoMax=>_ammoMax;

    public void Heal() => Heal(_healthMax);
    public void Heal(int value)
    {
        Health+=value;
    }

}
