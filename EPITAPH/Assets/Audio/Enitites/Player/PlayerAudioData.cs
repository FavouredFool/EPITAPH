using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "EPITAPH Audio/PlayerAudioData", order = 1)]
public class PlayerAudioData : ScriptableObject
{
    [Header("Weapon")]
    [SerializeField] public EventReference chargeEvent;
    [SerializeField] public EventReference lockedEvent;
    [SerializeField] public EventReference hitMeat;
    [SerializeField] public EventReference hitWall;

    public PlayerAudio Setup(GameObject go)
    {
        PlayerAudio playerAudio = go.AddComponent<PlayerAudio>();
        playerAudio.data = this;
        playerAudio.Setup();
        return playerAudio;

    }
}
