using UnityEngine;

public class AttackAnimationEvent : MonoBehaviour
{
    public void Attack()
    {
        GetComponentInParent<EnemyController>().Attack(1.5f);
    }

    public void ChargeAttack()
    {
        GetComponentInParent<EnemyController>().Attack(1.5f);

    }
}
