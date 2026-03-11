using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    public ProgressionVariables ProgressionVariables=>ProgressionVariableAnchor.ProgressionVariables;
    public PlayerVariables PlayerVariables=>PlayerVariableAnchor.PlayerVariables;

    public Scene currentLevel;

    void Start()
    {
        Scene loadedLevel = SceneManager.GetSceneByName(ProgressionVariables.LevelName);
        if (!loadedLevel.isLoaded)
            LoadLevel();
    }
    public void ProgressLevel()
    {

        SceneManager.UnloadSceneAsync(ProgressionVariables.LevelName);
        ProgressionVariables.Level++;
        LoadLevel();
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(ProgressionVariables.LevelName, LoadSceneMode.Additive);
    }
}
