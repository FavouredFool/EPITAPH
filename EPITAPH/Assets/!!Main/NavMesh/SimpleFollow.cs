using UnityEngine;
using UnityEngine.AI;

public class SimpleFollow : MonoBehaviour
{
    public Transform target;
    public float AttackDistance;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;

    void Start()
    {
        m_Agent= GetComponent<NavMeshAgent>();
        m_Animator=GetComponent<Animator>();
    }

    void Update()
    {
        m_Distance= Vector3.Distance(m_Agent.transform.position,target.position);
        if (m_Distance < AttackDistance)
        {
            m_Agent.isStopped=true;
            Debug.Log(name + "CAN NOW ATTACK!");
        }
        else
        {
            m_Agent.isStopped=false;
            m_Agent.destination= target.position;
        }
    }
}
