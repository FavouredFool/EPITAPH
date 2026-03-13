using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "EPITAPH Audio/PlayerAudioData", order = 1)]
public class PlayerAudioData : ScriptableObject
{
    [Header("Crossbow")]
    [SerializeField] public EventReference chargeEvent;
    [SerializeField] public EventReference lockedEvent;
    [SerializeField] public EventReference releaseEvent;
    [SerializeField] public EventReference boltPickup;
    [SerializeField] public EventReference parry;
    [Header("Hits")]
    [SerializeField] public EventReference hitMeat;
    [SerializeField] public EventReference hitWall;
    [Header("Movement")]
    [SerializeField] public EventReference lunge;   
    [SerializeField] public EventReference hitRecovery;
    [SerializeField] public EventReference bite;

    public PlayerAudio Setup(GameObject go)
    {
        PlayerAudio playerAudio = go.AddComponent<PlayerAudio>();
        playerAudio.data = this;
        playerAudio.Setup();
        return playerAudio;

    }
}
