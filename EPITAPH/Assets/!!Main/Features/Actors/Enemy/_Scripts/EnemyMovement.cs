using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField, Range(1, 20)] float _speed;
    
    Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(Mathf.Sin(Time.time), 0) * _speed;
    }
}
