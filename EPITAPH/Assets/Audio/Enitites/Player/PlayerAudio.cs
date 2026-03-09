using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerAudio : MonoBehaviour
{
    public PlayerAudioData data;

    EventInstance chargeInstance;
    EventInstance lockedInstance;

    

    public void Setup()
    {
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
    public void StartCharging()
    {
        chargeInstance.setParameterByName("Charge", 0.0f);
        PLAYBACK_STATE state;
        chargeInstance.getPlaybackState(out state);
        if(state != PLAYBACK_STATE.PLAYING)
        {
            chargeInstance.start();
        }
    }

    // Stops the Charging sound
    // Will allow for fadeout.
    public void StopCharging()
    {
        chargeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    // Sets the Charge (Within a Step)
    public void SetCharge(float charge)
    {
        chargeInstance.setParameterByName("Charge", charge);
    }


    public void PlayStepLock(int step)
    {
        PLAYBACK_STATE state;
        lockedInstance.getPlaybackState(out state);
        if (state == PLAYBACK_STATE.PLAYING) 
        {
            lockedInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        lockedInstance.start();
        lockedInstance.setParameterByName("ChargeStep", step);

    }

}
