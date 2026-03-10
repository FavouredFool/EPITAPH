using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public PlayerAudioData data;
    

    private void Awake()
    {
        data.Setup(gameObject);
         
    }

    public void PlayStep(float step)
    {
        Debug.Log((int)step);
        PlayerAudio.PlayStepLock((int)step);
        
    }

    public void charging(bool on)
    {
        if (on)
        {
            StartChage();
        }
        else
        {
            StopCharging();
        }
    }

    public void StartChage()
    {
        PlayerAudio.StartCharging();
    }

    public void StopCharging()
    {
        PlayerAudio.StopCharging();
    }

    public void SetCharge(float value)
    {
        PlayerAudio.SetCharge(value);
    }

    public void TestSetting(float val)
    {
        AudioManager.MasterVolume = val;
    }
}
