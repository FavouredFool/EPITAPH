using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BoltMarkerUIDManager : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] BoltMarker _markerPrefab;
    
    Dictionary<BoltType, BoltMarker> _markersDict;


    void Awake()
    {
        _markersDict = new Dictionary<BoltType, BoltMarker>
        {
            [BoltType.DOWN] = null,
            [BoltType.LEFT] = null,
            [BoltType.UP] = null,
        };
    }

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

    public BoltMarker GetNewMarker(BoltType type)
    {
        BoltMarker result = Instantiate(_markerPrefab, transform);
        result.Canvas = _canvas;
        result.SetSleep();

        SetMarker(type, result);

        return result;
    }

    void SetMarker(BoltType type, BoltMarker marker)
    {
        if (_markersDict.TryGetValue(type, out BoltMarker oldMarker))
        {
            if (marker != null)
            {
                Destroy(oldMarker);
            }
        }

        _markersDict[type] = marker;
    }
    

//SHOW
    public void ShowMarker(Signal_ShowBoltMarker signal) => ShowMarker(signal.parent,signal.dash,signal.feed, signal.type);
    public void ShowMarker(Transform parent, bool dash, bool feed, BoltType type)
    {
        BoltMarker marker= GetNewMarker(type);
        marker.TweenAppear(parent, dash, feed);
    }

//TRIGGER
    public void TriggerMarker(Signal_TriggerBoltMarker signal) => TriggerMarker(signal.parent, signal.kill, signal.type);
    public void TriggerMarker(Transform parent, bool kill, BoltType type)
    {
        if (parent == null)
        {
            Assert.IsTrue(false);
            return;
        }
        
        BoltMarker marker = _markersDict[type];

        if (marker == null)
        {
            return;
        }

        _markersDict[type] = null;
        
        if (kill)
        {
            Destroy(marker.gameObject);
            return;
        }

        marker.TweenTrigger();
    }

    [ContextMenu("Log Dict")]
    public void LogDict()
    {
        foreach (KeyValuePair<BoltType, BoltMarker> pair in _markersDict)
        {
            Debug.Log(pair);
        }
    }
}
