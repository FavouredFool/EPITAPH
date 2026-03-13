using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{

    [SerializeField] Transform _target;
    [SerializeField] LayerMask _wallLayers;
    //[SerializeField, UnityEngine.Range(1, 6)] int _maxHp;
    [SerializeField, UnityEngine.Range(0, 20)] float _speed;
    [SerializeField, UnityEngine.Range(1, 20)] float _knockbackDecay;
    [SerializeField, UnityEngine.Range(1, 20)] float _knockbackResistance = 1;
    //[field: SerializeField, UnityEngine.Range(1, 20)] public float FinalCollapsePush { get; set; } = 1;
    [field: SerializeField] public Vector2 ReviveRange { get; set; } = new(2, 5);


    [Header("3D Stuff")]
    [field: SerializeField] public Transform BoltBone { get; set; }

    [field: SerializeField, UnityEngine.Range(0.01f, 100)] public float KnockbackMagnitudeThreshold { get; set; } = 1f;
    [SerializeField] GameObject _deadSprite;

    public float KnockbackDecay => _knockbackDecay;

    public Rigidbody2D Rb { get; set; }
    //public int CurrentHp { get; set; }

    Vector2 _movementVelocity;
    public Vector2 KnockbackVelocity { get; set; }

    NavMeshAgent _agent;

    public BoltController CurrentlyStickingBolt { get; set; }

    [field: SerializeField] public Animator Animator { get; set; }

    public StateMachine StateMachine { get; set; }

    public TriggerPredicate StakedTrigger { get; private set; }
    public TriggerPredicate NormalDeathTrigger { get; private set; }
    public TriggerPredicate EnterKnockback { get; private set; }
    public TriggerPredicate ExitKnockback { get; private set; }

    public TriggerPredicate EnterStun { get; private set; }
    public TriggerPredicate ExitStun { get; private set; }

    public TriggerPredicate ReviveTrigger { get; private set; }

    public TriggerPredicate EnterChase { get; private set; }

    [SerializeField] public ParticleSystem GetParried;
    [SerializeField] public ParticleSystem Anticipation;
    [SerializeField] public ParticleSystem ChargeAnticipation;

    public Vector2 LatestHitVelocity { get; set; }

    public Vector3 offset;
    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        _target = FindFirstObjectByType<PlayerController>().transform;
        _agent = GetComponent<NavMeshAgent>();

        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = _speed;

        offset = UnityEngine.Random.insideUnitCircle.normalized;


        //CurrentHp = _maxHp;

        InitStateMachine();
    }


    void InitStateMachine()
    {
        StateMachine = new StateMachine();

        EnemyStateContext ctx = new(this);

        EverythingState chaseState = new(ctx);
        HitAndKnockbackedState hitAndKnockbackedState = new(ctx);
        NormalDeathState normalDeathState = new(ctx);
        StakedState stakedState = new(ctx);
        StunnedState stunnedState = new(ctx);
        IdleState idleState = new(ctx);

        EnterKnockback = new TriggerPredicate();
        ExitKnockback = new TriggerPredicate();
        StakedTrigger = new TriggerPredicate();
        NormalDeathTrigger = new TriggerPredicate();
        EnterStun = new TriggerPredicate();
        ExitStun = new TriggerPredicate();
        EnterChase = new TriggerPredicate();
        ReviveTrigger = new TriggerPredicate();

        //At(hitAndKnockbackedState, chaseState, ExitKnockback);

        Any(hitAndKnockbackedState, EnterKnockback);

        At(hitAndKnockbackedState, stakedState, StakedTrigger);
        //At(hitAndKnockbackedState, normalDeathState, NormalDeathTrigger);

        Any(normalDeathState, NormalDeathTrigger);

        At(stunnedState, chaseState, ExitStun);
        At(idleState, chaseState, EnterChase);
        At(chaseState, stunnedState, EnterStun);

        At(normalDeathState, chaseState, ReviveTrigger);
        StateMachine.SetState(idleState);
    }

    void At(IState from, IState to, IStatePredicate condition) =>
        StateMachine.AddTransition(from, to, condition);

    void Any(IState to, IStatePredicate condition) =>
        StateMachine.AddAnyTransition(to, condition);


    // please dont add anything here, use methods below
    void Update()
    {
        StateMachine.Update();
        //Debug.Log(StateMachine.CurrentState);
    }

    // please dont add anything here, use methods below
    void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    public void EverythingUpdate()
    {
        ChaseBehaviourUpdateTick();
        _agent.nextPosition = transform.position;
    }

    public void EverythingFixedUpdate()
    {
        ChaseBehaviourFixedUpdateTick();
        _agent.nextPosition = transform.position;
    }


    void TranslateMovement()
    {
        _movementVelocity = _agent.desiredVelocity;
        Rb.linearVelocity = _movementVelocity;
    }

    void Rotate()
    {
        if (_agent.speed == 0) return;
        Vector2 dir = Rb.linearVelocity;
        dir.Normalize();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Rb.MoveRotation(Quaternion.Euler(0f, 0f, angle));
    }

    #region ChaseBehaviour

    bool charging = false;
    bool attacking = false;
    float _lastChargeTime = -Mathf.Infinity;

    [SerializeField, UnityEngine.Range(0, 15)] float _chargeSpeed;
    [SerializeField, UnityEngine.Range(0, 15)] float _chargeStartRange;
    [SerializeField, UnityEngine.Range(0, 15)] float _chargeAttackRange;
    [SerializeField, UnityEngine.Range(0, 15)] float _chargeDuration;
    [SerializeField, UnityEngine.Range(0, 15)] float _chargeCooldown;
    [SerializeField, UnityEngine.Range(0, 15)] float _meleeAttackAntipicationTime;


    public void ChaseBehaviourUpdateTick()
    {

        if (Rb.linearVelocity.magnitude > 0.1f)
        {
            Animator.SetBool("walking", true);
        }
        else
        {
            Animator.SetBool("walking", false);
        }


        if (!charging)
        {

            _agent.destination = _target.position + offset * 0.5f;
        }
    }

    public void ChaseBehaviourFixedUpdateTick()
    {
        if (IsTargetInRangeForCharge() && IsTargetVisible() && !charging && Time.time >= _lastChargeTime + _chargeCooldown)
        {
            StartCoroutine(MeleeChaseAttackLoop());
        }

        if (IsTargetInRangeForMelee() && !charging && !attacking)
        {

            StartCoroutine(NormalAttackLoop());

        }

        if (!charging && !attacking)
        {
            TranslateMovement();
            Rotate();
        }

    }

    public IEnumerator NormalAttackLoop()
    {
        attacking = true;
        Rb.linearVelocity = Vector2.zero;
        Anticipation.Play();
        yield return new WaitForSeconds(_meleeAttackAntipicationTime);
        Animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.8f);
        attacking = false;
    }

    public IEnumerator MeleeChaseAttackLoop()
    {
        charging = true;


        //_agent.destination = (_target.transform.position-transform.position).normalized*500;
        bool attacked = false;
        Animator.SetBool("charging", true);
        Animator.SetTrigger("startCharge");
        _agent.speed = 0;
        Rb.linearVelocity = Vector2.zero;


        Vector2 decidedMovementVelocity = (_target.transform.position - transform.position).normalized * _chargeSpeed;
        ChargeAnticipation.Play();
        yield return new WaitForSeconds(1);
        Anticipation.Play();
        _agent.speed = _chargeSpeed;
        Rb.linearVelocity = decidedMovementVelocity;

        float timer = 0;

        while (timer < _chargeDuration)
        {
            timer += Time.deltaTime;
            if (IsTargetInRangeForMelee())
            {
                _agent.ResetPath();
                _agent.speed = 0;
                Rb.linearVelocity = Vector2.zero;

                Animator.SetTrigger("chargeAttack");
                attacked = true;
                break;
            }

            /*
            if(Vector2.Distance(transform.position, _agent.destination) < 0.5f)
            {
                Debug.Log("path cut short");
                _agent.ResetPath();
                _agent.speed = 0;
                _rb.linearVelocity = Vector2.zero;

                animator.SetTrigger("chargeAttack");
                attacked = true;
                break;
            }
            */
            yield return null;
        }



        if (!attacked)
        {
            _agent.ResetPath();
            _agent.speed = 0;
            Rb.linearVelocity = Vector2.zero;

            Animator.SetTrigger("chargeAttack");
            attacked = true;
        }

        yield return new WaitForSeconds(0.5f);

        Animator.SetBool("charging", false);
        _agent.speed = _speed;
        Rb.linearVelocity = Vector2.zero;

        charging = false;
        _lastChargeTime = Time.time;
    }


    private void OnDrawGizmos()
    {
      
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.up * 1f, 1.5f);
    }
    public void Attack(float attackRadius)
    {
        Physics2D.CircleCastAll(transform.position+transform.up * 1f, attackRadius, Vector2.zero).ToList().ForEach(e =>
        {
            if (e.transform.TryGetComponent<PlayerController>(out var player))
            {
                Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;

                //Debug.Log(Vector2.Angle(transform.right, dirToPlayer));
                
                if (player.IsParrying)
                {
                    EnterStun.Trigger();
                    player.ParrySuccessful();
                }
                else
                {
                    player.Hit((player.transform.position - transform.position).normalized);
                }
            }
        });
    }
    public bool IsTargetVisible()
    {
        return true;
    }

    public bool IsTargetInRangeForCharge()
    {
        return Vector2.Distance(transform.position, _target.transform.position) < _chargeStartRange && Vector2.Distance(transform.position, _target.transform.position) > 2;
    }

    public bool IsTargetInRangeForMelee()
    {
        return Vector2.Distance(transform.position, _target.transform.position) < _chargeAttackRange;
    }

    public bool IsTargetInRangeForChaseBegin()
    {
        return Vector2.Distance(transform.position, _target.transform.position) < 8;
    }

    #endregion

    public void Hit(Vector2 velocity)
    {
        LatestHitVelocity = velocity;
        EnterKnockback.Trigger();
    }

    public void Die()
    {
        SignalBus.Fire(new Signal_EnemyDeath(this));
    }

    public void Knockback(Vector2 velocity)
    {
        KnockbackVelocity += velocity / _knockbackResistance;
        Rb.linearVelocity = KnockbackVelocity;
        StopAllCoroutines();
        charging = false;
        attacking = false;
    }

    void CheckForStake()
    {
        Collider2D[] contacts = new Collider2D[12];
        Rb.GetContacts(ContactFilter2D.noFilter, contacts);

        if (contacts.Any(e => e != null && LayerUtil.MaskContainsLayer(_wallLayers, e.gameObject.layer)))
        {
            StakedTrigger.Trigger();
        }
    }

    public void EvaluateBoltHit(Vector2 velocity)
    {
        Hit(velocity);
        CheckForStake();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (StateMachine.CurrentState is HitAndKnockbackedState)
        {
            CheckForStake();
        }
    }
}

public struct EnemyStateContext
{
    public EnemyController EnemyController { get; set; }

    public EnemyStateContext(EnemyController enemyController)
    {
        EnemyController = enemyController;
    }
}
