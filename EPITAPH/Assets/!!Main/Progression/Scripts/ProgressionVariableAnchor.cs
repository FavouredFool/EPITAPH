using UnityEngine;

public class ProgressionVariableAnchor : MonoBehaviour
{
    public static ProgressionVariableAnchor ME {get; private set;}

    [SerializeField] ProgressionVariables progressionVariables;
    public static ProgressionVariables ProgressionVariables => ME.progressionVariables;

    void Awake()
    {
        if(ME != null)
        {
            Destroy(this);
            return;
        }

        ME = this;
        DontDestroyOnLoad(ME);

        progressionVariables = Instantiate(progressionVariables);
    }
}