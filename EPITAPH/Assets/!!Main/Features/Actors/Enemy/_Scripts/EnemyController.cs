using System;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] LayerMask _hitLayers;
    [SerializeField, UnityEngine.Range(1, 6)] int _maxHp;
    [SerializeField, UnityEngine.Range(1, 20)] float _speed;
    [SerializeField, UnityEngine.Range(1, 20)] float _knockbackDecay;
    [SerializeField, UnityEngine.Range(1, 20)] float _knockbackResistance = 1;
    
    Rigidbody2D _rb;
    int _currentHp;

    Vector2 _movementVelocity;
    Vector2 _knockbackVelocity;
    

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentHp = _maxHp;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        _knockbackVelocity *= Mathf.Exp(-_knockbackDecay * Time.deltaTime);
        //Debug.Log(_knockbackVelocity);

        _rb.linearVelocity = _movementVelocity + _knockbackVelocity;
    }
    
    public void Hit(Vector2 velocity)
    {
        _currentHp -= 1;

        if (_currentHp <= 0)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Knockback(velocity);
    }
    
    void Knockback(Vector2 velocity)
    {
        _knockbackVelocity += velocity / _knockbackResistance;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((_hitLayers & (1 << other.gameObject.layer)) == 0) return;
        
        Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
        Assert.IsNotNull(rb);
            
        Hit(rb.linearVelocity);
    }
    

}
