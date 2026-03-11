using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{

    public EnemyAudioData data;

    EventInstance attackInstance;

    private void Awake()
    {
        Setup();
    }
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
        RuntimeManager.AttachInstanceToGameObject(attackInstance, this.gameObject);

        attackInstance.start();


    }
}
