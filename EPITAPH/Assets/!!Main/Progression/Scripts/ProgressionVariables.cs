using UnityEngine;

[CreateAssetMenu(fileName = "ProgressionVariables", menuName = "Scriptable Objects/ProgressionVariables")]
public class ProgressionVariables : ScriptableObject
{
    [SerializeField] int _level;

    [SerializeField] string[] _levelSceneNames;


    public int Level
    {
        get => _level;
        set
        {
            _level = Mathf.Clamp(value, 0, _levelSceneNames.Length);
        }
    }
    public string LevelName => _levelSceneNames[Level];
}
