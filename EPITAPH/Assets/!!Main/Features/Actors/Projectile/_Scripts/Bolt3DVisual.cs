using System;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bolt3DVisual : MonoBehaviour
{
    [SerializeField, UnityEngine.Range(-0.1f, 0.5f)] float _staticHeightThreshold;

    Rigidbody _rb;
    Rigidbody2D _rb2D;
    bool _isStatic = false;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb2D = GetComponentInParent<Rigidbody2D>();
        Assert.IsNotNull(_rb2D);
    }

    void FixedUpdate()
    {
        if (_isStatic) return;

        if (transform.position.z > _staticHeightThreshold)
        {
            StopPhysics();
        }

        SetRotation();
    }

    void SetRotation()  
    {
        Vector3 velocity = new(_rb2D.linearVelocity.x, _rb2D.linearVelocity.y, _rb.linearVelocity.z);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, velocity.normalized);
        _rb.MoveRotation(rotation);
    }

    public void StopPhysics()
    {
        _rb.isKinematic = true;
        _rb2D.simulated = false;
        _isStatic = true;
    }
}
