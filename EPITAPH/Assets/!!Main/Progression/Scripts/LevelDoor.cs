using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelDoor : MonoBehaviour
{
    private bool AllEnemiesDefeated => deadEnemyCount>=enemyCount;
    private int enemyCount;
    private int deadEnemyCount;

    public GameObject counterObject, pinkGlow,purpleGlow;
    public TMP_Text counterText;

    void Awake()
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
        enemyCount = enemies.Length;
        deadEnemyCount = 0;
        RefreshUI();
    }

    private void OnEnable()
    {
        SignalBus.Subscribe<Signal_EnemyDeath>(OnEnemyDeath);
    }
    private void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_EnemyDeath>(OnEnemyDeath);        
    }

    public void OnEnemyDeath(Signal_EnemyDeath signal)
    {
        deadEnemyCount++;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if(!AllEnemiesDefeated)
            counterText.text= deadEnemyCount+"/"+enemyCount;
        else
            counterText.text="Next Chamber";

        counterObject.SetActive(AllEnemiesDefeated);
        pinkGlow.SetActive(AllEnemiesDefeated);
        purpleGlow.SetActive(!AllEnemiesDefeated);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player == null) return;

        if (AllEnemiesDefeated)
            ProgressionManager.LoadNextLevel();

        counterObject.SetActive(true);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.GetComponentInParent<PlayerController>()) return;

        counterObject.SetActive(AllEnemiesDefeated);
    }

}
