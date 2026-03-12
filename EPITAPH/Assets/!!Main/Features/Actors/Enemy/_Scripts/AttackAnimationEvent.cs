using UnityEngine;

public class AttackAnimationEvent : MonoBehaviour
{
    public void Attack()
    {
        GetComponentInParent<EnemyController>().Attack(2.3f);
    }

    public void ChargeAttack()
    {
        GetComponentInParent<EnemyController>().Attack(2.3f);

    }
}
