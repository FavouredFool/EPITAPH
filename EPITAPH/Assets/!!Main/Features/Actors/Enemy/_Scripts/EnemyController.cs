using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] LayerMask _wallLayers;
    [SerializeField] LayerMask _hitLayers;
    [SerializeField, UnityEngine.Range(1, 6)] int _maxHp;
    [SerializeField, UnityEngine.Range(1, 20)] float _speed;
    [SerializeField, UnityEngine.Range(1, 20)] float _knockbackDecay;
    [SerializeField, UnityEngine.Range(1, 20)] float _knockbackResistance = 1;
    [SerializeField, UnityEngine.Range(0.01f, 10)] float _pinKnockbackMagnitudeThreshold = 0.1f;
    [SerializeField] GameObject _deadSprite;
    
    Rigidbody2D _rb;
    int _currentHp;

    Vector2 _movementVelocity;
    Vector2 _knockbackVelocity;

    bool _isDead;
    

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
        if (_isDead)
        {
            _rb.linearVelocity = Vector3.zero;
            return;
        }
        
        _knockbackVelocity *= Mathf.Exp(-_knockbackDecay * Time.deltaTime);
        //Debug.Log(_knockbackVelocity);

        _rb.linearVelocity = _movementVelocity + _knockbackVelocity;
    }
    
    public void Hit(Vector2 velocity)
    {
        _currentHp -= 1;

        if (_currentHp <= 0)
        {
            Die();
            return;
        }
        
        Knockback(velocity);
    }

    void Die()
    {
        _deadSprite.SetActive(true);
        // this should be a bit better guarded later on
        _isDead = true;
        
        //Destroy(this.gameObject);
    }
    
    void Knockback(Vector2 velocity)
    {
        _knockbackVelocity += velocity / _knockbackResistance;
    }

    void CheckForStake()
    {
        Collider2D[] contacts = new Collider2D[12];
        _rb.GetContacts(ContactFilter2D.noFilter, contacts);
        
        if (contacts.Any(e => e != null && MaskContainsLayer(_wallLayers, e.gameObject.layer)))
        {
            if (_knockbackVelocity.magnitude > _pinKnockbackMagnitudeThreshold)
            {
                Debug.Log("STAKED");
                Die();
                
            }
        }
        
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        CheckForStake();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!MaskContainsLayer(_hitLayers, other.gameObject.layer)) return;
        
        Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
        Assert.IsNotNull(rb);
            
        Hit(rb.linearVelocity);
        CheckForStake();
    }

    bool MaskContainsLayer(LayerMask mask, int layer)
    {
        return (mask & (1 << layer)) != 0;
    }
    

}
