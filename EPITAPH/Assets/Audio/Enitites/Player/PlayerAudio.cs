using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class PlayerAudio : MonoBehaviour
{
    public PlayerAudioData data;

    EventInstance chargeInstance;
    EventInstance lockedInstance;
    EventInstance releasedInstance;

    public static PlayerAudio instance;
    

    public void Setup()
    {
        instance = this;
        // Weapon stuff
        chargeInstance = RuntimeManager.CreateInstance(data.chargeEvent);
        chargeInstance.setParameterByName("Charge", 0);
        RuntimeManager.AttachInstanceToGameObject(chargeInstance,gameObject,gameObject.GetComponent<Rigidbody2D>());

        releasedInstance = RuntimeManager.CreateInstance(data.releaseEvent);
        RuntimeManager.AttachInstanceToGameObject(releasedInstance, gameObject);


        lockedInstance = RuntimeManager.CreateInstance(data.lockedEvent);
        lockedInstance.setParameterByName("ChargeStep", 1);
        RuntimeManager.AttachInstanceToGameObject(lockedInstance, gameObject);
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
        RuntimeManager.AttachInstanceToGameObject(instance.chargeInstance, instance.gameObject, instance.gameObject.GetComponent<Rigidbody2D>());

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
        float dist;
        FMOD.RESULT wtf = instance.chargeInstance.getParameterByName("Elevation", out dist);
        Debug.Log(wtf);
        Debug.Log($"Charging:\n x:{dist}");
        Debug.Log(instance.gameObject.transform.position);
    }


    public static void PlayStepLock(int step)
    {
        PLAYBACK_STATE state;
        instance.lockedInstance.getPlaybackState(out state);
        if (state == PLAYBACK_STATE.PLAYING) 
        {
            instance.lockedInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        RuntimeManager.AttachInstanceToGameObject(instance.lockedInstance, instance.gameObject, instance.gameObject.GetComponent<Rigidbody2D>());

        instance.lockedInstance.start();
        instance.lockedInstance.setParameterByName("ChargeStep", step);

    }
    public static void PlayReleaseCrossbow()
    {
        PLAYBACK_STATE state;
        instance.releasedInstance.getPlaybackState(out state);
        if (state == PLAYBACK_STATE.PLAYING)
        {
            instance.releasedInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        RuntimeManager.AttachInstanceToGameObject(instance.releasedInstance, instance.gameObject, instance.gameObject.GetComponent<Rigidbody2D>());

        instance.releasedInstance.start();
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
