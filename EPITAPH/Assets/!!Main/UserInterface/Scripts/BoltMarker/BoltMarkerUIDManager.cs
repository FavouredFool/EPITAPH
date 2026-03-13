using System.Collections.Generic;
using UnityEngine;

public class BoltMarkerUIDManager : MonoBehaviour
{
    [SerializeField] BoltMarker _markerPrefab;

    [SerializeField] List<BoltMarker> _markers;

    void OnEnable()
    {
        SignalBus.Subscribe<Signal_ShowBoltMarker>(ShowMarker);
        SignalBus.Subscribe<Signal_TriggerBoltMarker>(TriggerMarker);
    }
    void OnDisable()
    {
        SignalBus.Unsubscribe<Signal_ShowBoltMarker>(ShowMarker);
        SignalBus.Unsubscribe<Signal_TriggerBoltMarker>(TriggerMarker);
    }

    void Start()
    {
        //SignalBus.Fire(new Signal_ShowBoltMarker(transform,BoltType.DOWN,true, true));
    }
    [ContextMenu("Trigger Test")] void TriggerTest()
    {
        SignalBus.Fire(new Signal_TriggerBoltMarker(BoltType.DOWN));
    }

    public BoltMarker GetNewMarker()
    {
        BoltMarker result = Instantiate(_markerPrefab);
        result.SetSleep();
        _markers.Add(result);

        return result;
    }
    public BoltMarker GetExistingMarker(BoltType type)
    {
        foreach(BoltMarker marker in _markers)
        {
            if(marker.type == type)
                return marker;
        }
        return null;
    }

//SHOW
    public void ShowMarker(Signal_ShowBoltMarker signal) => ShowMarker(signal.parent, signal.type,signal.dash,signal.feed);
    public void ShowMarker(Transform parent, BoltType type, bool dash, bool feed)
    {
        BoltMarker marker= GetNewMarker();
        marker.TweenAppear(parent, type, dash, feed);
    }

//TRIGGER
    public void TriggerMarker(Signal_TriggerBoltMarker signal) => TriggerMarker(signal.type);
    public void TriggerMarker(BoltType type)
    {
        BoltMarker marker = GetExistingMarker(type);

        if(marker!=null)
            TriggerMarker(marker);
    }
    public void TriggerMarker(BoltMarker marker)
    {
        _markers.Remove(marker);
        marker.TweenTrigger();
    }
}
