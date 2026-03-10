using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerAudio : MonoBehaviour
{
    public PlayerAudioData data;

    EventInstance chargeInstance;
    EventInstance lockedInstance;

    public static PlayerAudio instance;
    

    public void Setup()
    {
        instance = this;
        // Weapon stuff
        chargeInstance = RuntimeManager.CreateInstance(data.chargeEvent);
        chargeInstance.setParameterByName("Charge", 0);
        RuntimeManager.AttachInstanceToGameObject(chargeInstance,gameObject.transform);

        lockedInstance = RuntimeManager.CreateInstance(data.lockedEvent);
        lockedInstance.setParameterByName("ChargeStep", 1);
        RuntimeManager.AttachInstanceToGameObject(lockedInstance,gameObject.transform);
        
    }



    //

    // Method to start the charging sound.
    // Resets Charge state.
    // Starts only a new sound if none is playing
    public static void StartCharging()
    {
        instance.chargeInstance.setParameterByName("Charge", 0.0f);
        PLAYBACK_STATE state;
        instance.chargeInstance.getPlaybackState(out state);
        if(state != PLAYBACK_STATE.PLAYING)
        {
            instance.chargeInstance.start();
        }
    }

    // Stops the Charging sound
    // Will allow for fadeout.
    public static void StopCharging()
    {
        instance.chargeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    // Sets the Charge (Within a Step)
    public static void SetCharge(float charge)
    {
        instance.chargeInstance.setParameterByName("Charge", charge);
    }


    public static void PlayStepLock(int step)
    {
        PLAYBACK_STATE state;
        instance.lockedInstance.getPlaybackState(out state);
        if (state == PLAYBACK_STATE.PLAYING) 
        {
            instance.lockedInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        instance.lockedInstance.start();
        instance.lockedInstance.setParameterByName("ChargeStep", step);

    }

    public static void PlayMeatHit(Vector3 position)
    {
        RuntimeManager.PlayOneShot(instance.data.hitMeat, position);
    }

    public static void PlayWallHit(Vector3 position)
    {
        RuntimeManager.PlayOneShot(instance.data.hitWall, position);
    }
}
