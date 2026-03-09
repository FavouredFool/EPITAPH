using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public PlayerAudioData data;
    PlayerAudio pa;

    private void Awake()
    {
        data.Setup(gameObject);
        pa = GetComponent<PlayerAudio>();
    }

    public void PlayStep(float step)
    {
        Debug.Log((int)step);
        pa.PlayStepLock((int)step);
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
        pa.StartCharging();
    }

    public void StopCharging()
    {
        pa.StopCharging();
    }

    public void SetCharge(float value)
    {
        pa.SetCharge(value);
    }

    public void TestSetting(float val)
    {
        AudioManager.MasterVolume = val;
    }
}
