using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class EnvObjAudio : MonoBehaviour
{
    public EnvObjAudioData data;

    EventInstance interactionInstance;

    
    

    public void Setup()
    {

        // Weapon stuff
        interactionInstance = RuntimeManager.CreateInstance(data.interactionEvent);
        

        

    }

    public void PlayInteractionEvent()
    {
        RuntimeManager.AttachInstanceToGameObject(interactionInstance, gameObject, gameObject.GetComponent<Rigidbody2D>());
        interactionInstance.start();
    }
}
