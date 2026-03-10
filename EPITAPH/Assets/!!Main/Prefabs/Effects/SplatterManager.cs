using UnityEngine;

public class SplatterManager : MonoBehaviour
{
    [SerializeField] ParticleSystem EnemyBlood;
  
    private void OnEnable()
    {
        SignalBus.Subscribe<Hit_Enemy>(TriggerEnemyBlood);
    }
    private void OnDisable()
    {
        SignalBus.Unsubscribe<Hit_Enemy>(TriggerEnemyBlood);
    }
    public void TriggerEnemyBlood(Hit_Enemy signal) => TriggerEnemyBlood(signal.EnemyPosition);

    public void TriggerEnemyBlood(Vector3 position)
    {
     
        ParticleSystem EnemyBloodInstance = Instantiate(EnemyBlood, position, transform.rotation);
      
    }
    private void OnDrawGizmos()
    {
    
    }

}
