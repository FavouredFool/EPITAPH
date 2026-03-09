using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoltController : MonoBehaviour
{
    [SerializeField, Range(1, 20)] float _shootSpeed = 15;

    Rigidbody2D _rb;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _rb.AddForce(transform.up * _shootSpeed, ForceMode2D.Impulse);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        _rb.simulated = false;
        
        // if you hit a wall, 
        
        _rb.transform.SetParent(other.transform.parent, true);
    }
    
}
