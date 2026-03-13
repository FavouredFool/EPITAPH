using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BoltMarkerUIDManager : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
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

    public BoltMarker GetNewMarker()
    {
        BoltMarker result = Instantiate(_markerPrefab, transform);
        result.Canvas = _canvas;
        result.SetSleep();
        _markers.Add(result);

        return result;
    }
    public BoltMarker GetExistingMarker(Transform parent)
    {
        foreach(BoltMarker marker in _markers)
        {
            if (marker.Parent == parent)
            {
                return marker;
            }
                
        }
        return null;
    }

//SHOW
    public void ShowMarker(Signal_ShowBoltMarker signal) => ShowMarker(signal.parent,signal.dash,signal.feed);
    public void ShowMarker(Transform parent, bool dash, bool feed)
    {
        Debug.Log("show");
        BoltMarker marker= GetNewMarker();
        marker.TweenAppear(parent, dash, feed);
    }

//TRIGGER
    public void TriggerMarker(Signal_TriggerBoltMarker signal) => TriggerMarker(signal.parent, signal.kill);
    public void TriggerMarker(Transform parent, bool kill)
    {
        if (parent == null)
        {
            Assert.IsTrue(false);
            return;
        }
        
        BoltMarker marker = GetExistingMarker(parent);

        if (marker == null)
        {
            return;
        }
        
        _markers.Remove(marker);
        
        if (kill)
        {
            Destroy(marker.gameObject);
            return;
        }

        marker.TweenTrigger();
    }
}
