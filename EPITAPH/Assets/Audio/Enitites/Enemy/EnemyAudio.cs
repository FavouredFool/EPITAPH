using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{

    public EnemyAudioData data;

    EventInstance attackInstance;

    public void Setup()
    {
        attackInstance = RuntimeManager.CreateInstance(data.AttackReference);
        RuntimeManager.AttachInstanceToGameObject(attackInstance, this.gameObject);

    }

    public void PlayAttack()
    {
        PLAYBACK_STATE state;
        attackInstance.getPlaybackState(out state);
        if (state == PLAYBACK_STATE.PLAYING)
        {
            attackInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        attackInstance.start();


    }
}
