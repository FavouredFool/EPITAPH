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
            _level = Mathf.Clamp(value, 0, _levelSceneNames.Count);
        }
    }
    public string LevelName => _levelSceneNames[Level];

    public void SetLevel(string name)
    {
        _level = _levelSceneNames.IndexOf(name);
    }
    
}
