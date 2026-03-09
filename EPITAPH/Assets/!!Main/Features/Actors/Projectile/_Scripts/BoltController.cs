using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoltController : MonoBehaviour
{
    [SerializeField, Range(1, 300)] float _shootSpeed = 15;

    Rigidbody2D _rb;
    Rigidbody _rb3D;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb3D = GetComponentInChildren<Rigidbody>();
    }

    void Start()
    {
        _rb.AddForce(transform.up * _shootSpeed, ForceMode2D.Impulse);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        _rb.simulated = false;
        _rb3D.GetComponent<Bolt3DVisual>().StopPhysics();
        
        _rb.transform.SetParent(other.transform.parent, true);
    }
    
}
