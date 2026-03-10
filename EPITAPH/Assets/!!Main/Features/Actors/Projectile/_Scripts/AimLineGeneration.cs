using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AimLineGeneration : MonoBehaviour
{
    [SerializeField] PlayerController _playerController;
    [SerializeField, Range(4, 16)] int _resolution;
    [SerializeField, Range(1, 10)] float _startDistance;
    [SerializeField, Range(1, 10)] float _endDistance;
    [SerializeField, Range(1, 100)] float _angleDecaySpeed = 3;

    LineRenderer _lineRenderer;
    
    float _offsetAngle = 0;
    float _actualAngle = 0;
    
    bool _enabled;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2 + _resolution;
    }
    
    void LateUpdate()
    {
        UpdateOffsetAngle();
        
        if (_enabled && !_playerController.ReadyToShoot)
        {
            _enabled = false;
        }
        
        if (!_enabled && _playerController.ReadyToShoot)
        {
            _actualAngle = Vector2.SignedAngle(Vector2.up, _playerController.AimAssistedLookDirection);
            _actualAngle = _offsetAngle;
            _enabled = true;
        }
        
        RegenerateLine();

        _lineRenderer.enabled = _playerController.ReadyToShoot;
    }

    void UpdateOffsetAngle()
    {
        // Should turn this into a windup that i can then resolve with the UI
        _actualAngle = Vector2.SignedAngle(Vector2.up, _playerController.transform.up);
        
        if (!_playerController.ReadyToShoot)
        {
            _offsetAngle = _actualAngle;
        }
        else
        {
            _offsetAngle = Mathf.MoveTowardsAngle(_offsetAngle, _actualAngle, _angleDecaySpeed * Time.deltaTime * 100);
        }
    }

    void RegenerateLine()
    {
        Vector3[] points = new Vector3[2 + _resolution];

        Quaternion actualRotation = Quaternion.Euler(0, 0, _actualAngle);
        
        points[0] = _playerController.transform.position + actualRotation * Vector3.up * _startDistance;
        points[1] = _playerController.transform.position + actualRotation * Vector3.up * _endDistance;
        
        for (int i = 0; i < _resolution; i++)
        {
            Quaternion offsetRotation = Quaternion.Euler(0, 0, _offsetAngle);
            Quaternion partRotation = Quaternion.Slerp(actualRotation, offsetRotation, (i + 1f) / _resolution);
            
            points[i+2] = _playerController.transform.position + partRotation * Vector3.up * _endDistance;
        }

        _lineRenderer.SetPositions(points);
    }
}
