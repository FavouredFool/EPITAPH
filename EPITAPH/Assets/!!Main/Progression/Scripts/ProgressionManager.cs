using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    void Awake()
    {
        ProgressionVariableAnchor.ProgressionVariables.SetLevel(SceneManager.GetActiveScene().name);
        Scene baseScene = SceneManager.GetSceneByName("_GameBase");
        if (!baseScene.isLoaded)
        SceneManager.LoadScene("_GameBase", LoadSceneMode.Additive);
    }
    public static void LoadNextLevel()
    {
        ProgressionVariables progressionVariables = ProgressionVariableAnchor.ProgressionVariables;

        SceneManager.UnloadSceneAsync(progressionVariables.LevelName);
        progressionVariables.Level++;
        string levelname = progressionVariables.LevelName;
        SceneManager.LoadScene(levelname,levelname=="_End"? LoadSceneMode.Single: LoadSceneMode.Additive);
    }

}
