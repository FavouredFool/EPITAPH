using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoltController : MonoBehaviour
{
    [SerializeField] LayerMask _blockLayers;
    [SerializeField] LayerMask _boltPickup;
    [SerializeField, Range(1, 300)] float _shootSpeed = 15;

    [SerializeField] Collider2D _hitbox;
    [SerializeField] Collider2D _pickupBox;

    [SerializeField] Gradient _mainGradient;
    [SerializeField] Gradient _blockedGradient;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Transform _endPoint;

    public Rigidbody2D Rb2D { get; set; }
    public Transform BloodpointPlayer { get; set; }
    Rigidbody _rb3D;

    BoltType _boltType;

    public bool IsLungeable { get; set; }
    
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
        Rb2D = GetComponent<Rigidbody2D>();
        _rb3D = GetComponentInChildren<Rigidbody>();

        _hitbox.enabled = true;
        _pickupBox.enabled = false;
    }

    void Start()
    {
        Rb2D.AddForce(transform.up * _shootSpeed, ForceMode2D.Impulse);
    }

    void Update()
    {
        IsLungeable = TestLungeable();
    }

    void LateUpdate()
    {
        _lineRenderer.SetPosition(0, _endPoint.position);
        
        Vector3 playerPos = _lineRenderer.GetPosition(1);
        Vector2 position = BloodpointPlayer.position;
        
        playerPos.x = position.x;
        playerPos.y = position.y;
        
        _lineRenderer.SetPosition(1, playerPos);
        
        _lineRenderer.colorGradient = IsLungeable ? _mainGradient : _blockedGradient;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HitSomething(other);
    }

    public void HitSomething(Collider2D other = null)
    {
        Vector2 velocity = Rb2D.linearVelocity;
        
        Rb2D.bodyType = RigidbodyType2D.Kinematic;
        Rb2D.linearVelocity = Vector2.zero;

        _rb3D.GetComponent<Bolt3DVisual>().StopPhysics();
        LayerUtil.SetLayerRecursively(gameObject, LayerUtil.ExtractLayerFromMask(_boltPickup));
        
        _hitbox.enabled = false;
        _pickupBox.enabled = true;
        
		if (other != null)
        {
			Rb2D.transform.SetParent(other.transform.parent, true);
		}
        
        if (other != null)
        {
            if (other.GetComponentInParent<EnemyController>() is { } enemy)
            {
                enemy.EvaluateBoltHit(velocity);
            }
        }

        SignalBus.Fire(new Signal_ShowBoltMarker(transform,_boltType,true,false));
    }

    public bool TestLungeable()
    {
        Vector2 diff = BloodpointPlayer.position - _endPoint.position;
        RaycastHit2D hit = Physics2D.Raycast(_endPoint.position, diff.normalized, diff.magnitude, _blockLayers);
        
        return hit.collider == null;
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
