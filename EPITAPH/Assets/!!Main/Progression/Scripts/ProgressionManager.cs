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

}
