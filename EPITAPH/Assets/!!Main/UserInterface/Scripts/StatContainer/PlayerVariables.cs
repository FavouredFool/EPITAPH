using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "PlayerVariables", menuName = "Scriptable Objects/PlayerVariables")]
public class PlayerVariables : ScriptableObject
{
    [Header("Health")]
    [SerializeField] int _health;
    [SerializeField] int _healthMax;

[Header("Ammo")]
    public Dictionary<BoltType, BoltController> CurrentBoltsHeld { get; set; }
    //[SerializeField] int _ammo;
    //[SerializeField] int _ammoMax;

[Header("Charge")]
    [SerializeField] int _charge;
    [SerializeField] int _chargeMax;
        [SerializeField] float _chargeProgress;

        [Header("Style")]
    [SerializeField] int _style;
    [SerializeField] int _styleMax;
    [SerializeField] float _styleResetTime;
    float _lastStyleChangeTime;

    public int Health
    {
        get=> _health;
        set
        {
            if (value == 0)
            {
                SceneManager.LoadScene("Level1");
            }
            
            _health=Mathf.Clamp(value,0,_healthMax);
            SignalBus.Fire(new Signal_RefreshUI_Health(this));
        }
    }
    public int Charge
    {
        get=> _charge;
        set
        {
            _charge=Mathf.Clamp(value,0,_chargeMax);
            SignalBus.Fire(new Signal_RefreshUI_Charge(this));
        }
    }
    public float ChargeProgress
    {
        get=> _chargeProgress;
        set
        {
            _chargeProgress=Mathf.Clamp01(value);
            SignalBus.Fire(new Signal_RefreshUI_ChargeProgress(this));
        }
    }
    public int Style
    {
        get => _style;
        set
        {
            _style = Mathf.Clamp(value, 0, _styleMax);
            Debug.Log("STYLE IS NOW lvl "+_style);
            _lastStyleChangeTime = Time.time;
            SignalBus.Fire(new Signal_RefreshUI_Style(this));
        }
    }

    public void AddAmmo(BoltType type)
    {
        CurrentBoltsHeld[type] = null;
        SignalBus.Fire(new Signal_RefreshUI_Ammo(this));
    }

    public void LoseAmmo(BoltController bolt)
    {
        CurrentBoltsHeld[bolt.BoltType] = bolt;
        SignalBus.Fire(new Signal_RefreshUI_Ammo(this));
    }
    
    public int CurrentAmmoCount
    {
        get
        {
            int countAmmo = 0;

            foreach (var bolts in CurrentBoltsHeld.Values)
            {
                if (bolts == null)
                {
                    countAmmo += 1;
                }
            }
            
            return countAmmo;
        }
    }
    public int HealthMax=>_healthMax;
    public int ChargeMax=>_chargeMax;

    public float StyleMax=>_styleMax;
    public float StyleResetTime=>_styleResetTime;
    public float StyleResetI=>(Time.time-_lastStyleChangeTime)/_styleResetTime;

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
}
