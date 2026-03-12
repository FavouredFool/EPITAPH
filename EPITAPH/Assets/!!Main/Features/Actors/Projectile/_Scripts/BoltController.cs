using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoltController : MonoBehaviour
{
    [SerializeField] LayerMask _blockLayers;
    [SerializeField] LayerMask _boltPickup;

    [SerializeField] Collider2D _hitbox;
    [SerializeField] Collider2D _pickupBox;

    [SerializeField] Gradient _mainGradient; 
    [SerializeField] Gradient _blockedGradient;
    [SerializeField] Gradient _activatableGradient;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Transform _endPoint;
    //[SerializeField, Range(0, 2)] public float _testBoltRayThickness = 1;

    [Header("MaterialStuff")]
    [SerializeField] MeshRenderer[] _bloodRecolor;
    [SerializeField] Color _reColor;

    [Header("3D Stuff")]
    [SerializeField] Transform _visual3D;
    
    public Rigidbody2D Rb2D { get; set; }
    public PlayerController Player { get; set; }
    Rigidbody _rb3D;

    BoltType _boltType;

    float _baseWidth;

    public bool IsActivatable => IsSelected && IsLineOfSight && HasHitSomething /*&& IsStakeBolt*/;
    
    public bool IsSelected { get; set; }
    
    public bool IsLineOfSight { get; set; }
    public bool HasHitSomething { get; set; } = false;
    public bool IsStakeBolt { get; set; } = false;
    
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

        _baseWidth = _lineRenderer.startWidth;
        _lineRenderer.endWidth = _baseWidth;
    }

    public void GetShot(float shootSpeed)
    {
        Rb2D.AddForce(transform.up * shootSpeed, ForceMode2D.Impulse);
    }

    void Update()
    {
        IsLineOfSight = TestLineOfSight();
    }

    void LateUpdate()
    {
        _lineRenderer.SetPosition(0, _endPoint.position);
        
        Vector3 playerPos = _lineRenderer.GetPosition(1);
        Vector2 position = Player.BloodlineConnection.position;
        
        playerPos.x = position.x;
        playerPos.y = position.y;
        
        _lineRenderer.SetPosition(1, playerPos);
        
        if (IsActivatable)
        {
            _lineRenderer.colorGradient = _activatableGradient;
            _lineRenderer.startWidth = _baseWidth*3;
            _lineRenderer.endWidth = _baseWidth*3;
        }
        else
        {
            _lineRenderer.colorGradient = IsLineOfSight ? _mainGradient : _blockedGradient;
            _lineRenderer.startWidth = _baseWidth;
            _lineRenderer.endWidth = _baseWidth;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HitSomething(other);
    }

    public void HitSomething(Collider2D other = null)
    {
        if (other != null)
        {
            if (other.GetComponent<BreakableWall>() is { }
            breakableWall)
            {
                breakableWall.BreakWall();
                return;
            }
          
        }
        Vector2 velocity = Rb2D.linearVelocity;
        
        Rb2D.bodyType = RigidbodyType2D.Kinematic;
        Rb2D.linearVelocity = Vector2.zero;

        _rb3D.GetComponent<Bolt3DVisual>().StopPhysics();
        LayerUtil.SetLayerRecursively(gameObject, LayerUtil.ExtractLayerFromMask(_boltPickup));
        
        _hitbox.enabled = false;
        HasHitSomething = true;
        
		if (other != null)
        {
			Rb2D.transform.SetParent(other.transform.parent, true);
            
            if (other.GetComponentInParent<EnemyController>() is { } enemy)
            {
                StickToEnemy(enemy);
                enemy.EvaluateBoltHit(velocity);
            }
            else
            {
                EnablePickup();
            }
		}

        SignalBus.Fire(new Signal_ShowBoltMarker(transform,_boltType,true,false));
    }

    public bool TestLineOfSight()
    {
        // only if there is a clear line of sight to the enemy
        Vector2 diff = Player.BloodlineConnection.position - _endPoint.position;
        //Vector2 dir = diff.normalized;
        RaycastHit2D hit = Physics2D.Raycast(_endPoint.position, diff.normalized, diff.magnitude, _blockLayers);
        return hit.collider == null;
        
        //Vector2 start = _endPoint.position;
        //Vector2 center = start + diff * 0.5f;
        
        //RaycastHit2D hit = Physics2D.BoxCast(
        //    center,
        //    new Vector2(diff.magnitude, _testBoltRayThickness),
        //    Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg,
        //    Vector2.zero,
        //    0f,
        //    _blockLayers
        //);
        
        return hit.collider == null;
    }

    void MakeBloody()
    {
        foreach (MeshRenderer bloodMatRenderer in _bloodRecolor)
        {
            foreach (Material mat in bloodMatRenderer.materials)
            {
                mat.SetColor("_BoltColor", _reColor);
            }
        }
    }

    void StickToEnemy(EnemyController enemy)
    {
        enemy.CurrentlyStickingBolt = this;
        _visual3D.SetParent(enemy.BoltBone);
        _visual3D.localPosition = Vector3.zero;
        _visual3D.localRotation = Quaternion.identity;
        MakeBloody();
    }

    public void StickToNothing()
    {
        
        transform.SetParent(null, true);
        transform.position = _visual3D.position;
        _visual3D.SetParent(transform, true);
        // reductive tecnically
        _visual3D.localPosition = Vector3.zero;
        
        // do i want to keep the rotation maybe? Probably not.
        //_visual3D.localRotation = Quaternion.identity;
        
        EnablePickup();
    }

    public void EnablePickup()
    {
        _pickupBox.enabled = true;
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
