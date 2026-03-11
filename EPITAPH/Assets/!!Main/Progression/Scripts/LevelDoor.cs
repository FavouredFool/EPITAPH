using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    [SerializeField] ProgressionManager progressionManager;
    private bool AllEnemiesDefeated = true;
    private int EnemyCount;
    private int currentDeadEnemies;

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&AllEnemiesDefeated)
        {
            ProgressionManager.LoadNextLevel();
        }
    }
    private void OnTriggerEnter2D(Collider other)
    {
        if (other.CompareTag("Player") && AllEnemiesDefeated)
        {
            ProgressionManager.LoadNextLevel();
        }
    }

}
