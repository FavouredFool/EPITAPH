using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class ProgressionManager : MonoBehaviour
{
    void Awake()
    {
        ProgressionVariableAnchor.ProgressionVariables.SetLevel(SceneManager.GetActiveScene().name);
        Scene baseScene = SceneManager.GetSceneByName("_GameBase");
        if (!baseScene.isLoaded)
            SceneManager.LoadScene("_GameBase", LoadSceneMode.Additive);
    }

    [YarnFunction("LevelName")]
    public static string GetLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
