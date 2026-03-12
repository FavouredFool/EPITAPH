using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvObj", menuName = "EPITAPH Audio/EnvObjAudioData", order = 1)]
public class EnvObjAudioData : ScriptableObject
{
    [Header("Interaction")]
    [SerializeField] public EventReference interactionEvent;


    public EnvObjAudio Setup(GameObject go)
    {
        EnvObjAudio envObjAudio = go.AddComponent<EnvObjAudio>();
        envObjAudio.data = this;
        envObjAudio.Setup();
        return envObjAudio;

    }
}
