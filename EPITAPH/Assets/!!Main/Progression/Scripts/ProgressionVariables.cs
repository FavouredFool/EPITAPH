using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProgressionVariables", menuName = "Scriptable Objects/ProgressionVariables")]
public class ProgressionVariables : ScriptableObject
{
    [SerializeField] int _level;

    [SerializeField] List<string> _levelSceneNames;


    public int Level
    {
        get => _level;
        set
        {
            _level = Mathf.Clamp(value, 0, _levelSceneNames.Count-1);
        }
    }
    public string LevelName => Level>=_levelSceneNames.Count?"_End": _levelSceneNames[Level];

    public void SetLevel(string name)
    {
        Level = _levelSceneNames.IndexOf(name);
    }
    
}
