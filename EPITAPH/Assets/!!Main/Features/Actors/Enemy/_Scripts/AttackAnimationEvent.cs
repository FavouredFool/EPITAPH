using UnityEngine;

public class AttackAnimationEvent : MonoBehaviour
{
    public void Attack()
    {
        GetComponentInParent<EnemyController>().Attack(2f);
    }
}
