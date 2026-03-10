using UnityEngine;

public class EnemyState : MonoBehaviour
{

    public EnemyController enemyController;

    public void Start()
    {
        enemyController= GetComponentInParent<EnemyController>();
    }
    public virtual void UpdateTick()
    {

    }
   


}
