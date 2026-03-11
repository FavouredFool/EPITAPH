using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BeatColorChanger : BeatResponderBase
{
    [SerializeField] Light2D LightToChange;

    [SerializeField] System.Collections.Generic.List<Color> Colors;

    int ColorIndex=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnOnBeatEventHappened(BeatChanged e)
    {
        LightToChange.color = Colors[ColorIndex];
        ColorIndex++;
        ColorIndex = ColorIndex % Colors.Count;
    }
}
