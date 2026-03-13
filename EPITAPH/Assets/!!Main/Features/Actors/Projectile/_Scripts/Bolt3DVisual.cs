
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bolt3DVisual : MonoBehaviour
{
    [SerializeField, UnityEngine.Range(-0.1f, 0.5f)] float _staticHeightThreshold;

    Rigidbody _rb;
    BoltController _boltController;
    bool _isStatic = false;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _boltController = GetComponentInParent<BoltController>();

    }

    void FixedUpdate()
    {
        if (_isStatic) return;

        if (transform.position.z > _staticHeightThreshold)
        {
            _boltController.EnablePickup();
            _boltController.HitSomething();
        }

        SetRotation();
    }

    void SetRotation()  
    {
        Vector3 velocity = new(_boltController.Rb2D.linearVelocity.x, _boltController.Rb2D.linearVelocity.y, _rb.linearVelocity.z);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, velocity.normalized);
        _rb.MoveRotation(rotation);
    }

    // this is invoked through the BoltController and not directly from here
    public void StopPhysics()
    {
        _rb.isKinematic = true;
        _isStatic = true;
    }
}
