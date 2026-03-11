using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "EPITAPH Audio/EnemyData", order = 1)]
public class EnemyAudioData : ScriptableObject
{
    [Header("Attacks")]
    [SerializeField] public EventReference AttackReference;

    public EnemyAudio Setup(GameObject go)
    {
        EnemyAudio enemyAudio = go.AddComponent<EnemyAudio>();
        enemyAudio.data = this;
        enemyAudio.Setup();
        return enemyAudio;
    }
}
