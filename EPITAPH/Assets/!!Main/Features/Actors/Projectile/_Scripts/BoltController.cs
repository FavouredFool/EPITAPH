using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoltController : MonoBehaviour
{
    [SerializeField] LayerMask _boltPickup;
    [SerializeField, Range(1, 300)] float _shootSpeed = 15;

    public Rigidbody2D RB2D { get; set; }
    Rigidbody _rb3D;

    BoltType _boltType;
    
    public BoltType BoltType
    {
        get => _boltType;
        set
        {
            gameObject.name = $"Bolt: {value}";
            _boltType = value;
        }
    }

    void Awake()
    {
        RB2D = GetComponent<Rigidbody2D>();
        _rb3D = GetComponentInChildren<Rigidbody>();
    }

    void Start()
    {
        RB2D.AddForce(transform.up * _shootSpeed, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HitSomething(other);
    }

    public void HitSomething(Collider2D other = null)
    {
        RB2D.linearVelocity = Vector2.zero;
        LayerUtil.SetLayerRecursively(gameObject, LayerUtil.ExtractLayerFromMask(_boltPickup));
        _rb3D.GetComponent<Bolt3DVisual>().StopPhysics();

        if (other != null)
        {
            RB2D.transform.SetParent(other.transform.parent, true);
        }
    }
}


public enum BoltType
{
    DOWN,
    LEFT,
    UP,
    RIGHT,
    NONE
}
