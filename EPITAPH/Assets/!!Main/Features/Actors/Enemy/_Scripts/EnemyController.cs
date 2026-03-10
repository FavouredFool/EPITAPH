using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] LayerMask _wallLayers;
    [SerializeField, UnityEngine.Range(1, 6)] int _maxHp;
    [SerializeField, UnityEngine.Range(0, 20)] float _speed;
    [SerializeField, UnityEngine.Range(1, 20)] float _knockbackDecay;
    [SerializeField, UnityEngine.Range(1, 20)] float _knockbackResistance = 1;
    [SerializeField, UnityEngine.Range(0.01f, 10)] float _pinKnockbackMagnitudeThreshold = 0.1f;
    [SerializeField] GameObject _deadSprite;

    Rigidbody2D _rb;
    int _currentHp;

    Vector2 _movementVelocity;
    Vector2 _knockbackVelocity;

    bool _isDead;

    NavMeshAgent _agent;

    public Animator animator;
    public EnemyState CurrentState { get; private set; }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = FindFirstObjectByType<PlayerController>().transform;
        _agent = GetComponent<NavMeshAgent>();

        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = _speed;

        _currentHp = _maxHp;
    }

    void Update()
    {
        ChaseBehaviourUpdateTick();
        _agent.nextPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (_isDead)
        {
            _rb.linearVelocity = Vector3.zero;
            return;
        }
        ChaseBehaviourFixedUpdateTick();
    }

    void Translate()
    {
        _movementVelocity = _agent.desiredVelocity;
        _knockbackVelocity *= Mathf.Exp(-_knockbackDecay * Time.deltaTime);
        _rb.linearVelocity = _movementVelocity + _knockbackVelocity;
    }

    void Rotate()
    {
        if (_agent.speed == 0) return;
        Vector2 dir = _rb.linearVelocity;
        dir.Normalize();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        _rb.MoveRotation(Quaternion.Euler(0f, 0f, angle));
    }

    #region ChaseBehaviour

    bool charging = false;
    float _lastChargeTime = -Mathf.Infinity;

    [SerializeField, UnityEngine.Range(1, 15)] float _chargeSpeed;
    [SerializeField, UnityEngine.Range(1, 15)] float _chargeStartRange;
    [SerializeField, UnityEngine.Range(1, 15)] float _chargeAttackRange;
    [SerializeField, UnityEngine.Range(1, 15)] float _chargeDuration;
    [SerializeField, UnityEngine.Range(1, 15)] float _chargeCooldown;

    public void ChaseBehaviourUpdateTick()
    {
        if (!charging)
        {
            _agent.destination = _target.position;
        }
    }

    public void ChaseBehaviourFixedUpdateTick()
    {
        if (IsTargetInRangeForCharge() && IsTargetVisible() && !charging && Time.time >= _lastChargeTime + _chargeCooldown)
        {
            StartCoroutine(MeleeChaseAttackLoop());
        }

        Translate();
        Rotate();
    }

    public IEnumerator MeleeChaseAttackLoop()
    {
        charging = true;

        bool attacked = false;
        animator.SetBool("charging", true);
        animator.SetTrigger("startCharge");
           // _agent.ResetPath();
        _agent.speed = 0;

        yield return new WaitForSeconds(1);
        _agent.speed = _chargeSpeed;

        float timer = 0;

        while (timer < _chargeDuration)
        {
            timer += Time.deltaTime;
            if (IsTargetInRangeForMelee())
            {
                _agent.ResetPath();
                _agent.speed = 0;
                animator.SetTrigger("chargeAttack");
                attacked = true;
                break;
            }

            if(Vector2.Distance(transform.position, _agent.destination) < 0.5f)
            {
                _agent.ResetPath();
                _agent.speed = 0;
                animator.SetTrigger("chargeAttack");
                attacked = true;
                break;
            }
            yield return null;
        }

        if (!attacked)
        {
            _agent.ResetPath();
            _agent.speed = 0;
            animator.SetTrigger("chargeAttack");
            attacked = true;
        }

        yield return new WaitForSeconds(1);
        animator.SetBool("charging", false);
        _agent.speed = _speed;

        charging = false;
        _lastChargeTime = Time.time;
    }

    public bool IsTargetVisible()
    {
        return true;
    }

    public bool IsTargetInRangeForCharge()
    {
        return Vector2.Distance(transform.position, _target.transform.position) < _chargeStartRange;
    }

    public bool IsTargetInRangeForMelee()
    {
        return Vector2.Distance(transform.position, _target.transform.position) < _chargeAttackRange;
    }

    #endregion

    public void Hit(Vector2 velocity)
    {
        _currentHp -= 1;
        PlayerAudio.PlayMeatHit(this.transform.position);

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
        _isDead = true;
    }

    void Knockback(Vector2 velocity)
    {
        _knockbackVelocity += velocity / _knockbackResistance;
    }

    void CheckForStake()
    {
        Collider2D[] contacts = new Collider2D[12];
        _rb.GetContacts(ContactFilter2D.noFilter, contacts);

        if (contacts.Any(e => e != null && LayerUtil.MaskContainsLayer(_wallLayers, e.gameObject.layer)))
        {
            if (_knockbackVelocity.magnitude > _pinKnockbackMagnitudeThreshold)
            {
                PlayerAudio.PlayWallHit(this.transform.position);
                Die();
            }
        }
    }

    public void EvaluateBoltHit(Vector2 velocity)
    {
        Hit(velocity);
        CheckForStake();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        CheckForStake();
    }
}