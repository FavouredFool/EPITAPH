using UnityEngine;

public class SplatterManager : MonoBehaviour
{
    [SerializeField] ParticleSystem EnemyBlood;
    [SerializeField] ParticleSystem PlayerBlood;

    private void OnEnable()
    {
        SignalBus.Subscribe<Hit_Enemy>(TriggerEnemyBlood);
        SignalBus.Subscribe<Hit_Player>(TriggerPlayerBlood);
    }
    private void OnDisable()
    {
        SignalBus.Unsubscribe<Hit_Enemy>(TriggerEnemyBlood);
        SignalBus.Unsubscribe<Hit_Player>(TriggerPlayerBlood);
    }
    public void TriggerEnemyBlood(Hit_Enemy signal) => TriggerEnemyBlood(signal.EnemyPosition);
    public void TriggerPlayerBlood(Hit_Player signal) => TriggerPlayerBlood(signal.PlayerTransform.position);
    public void TriggerEnemyBlood(Vector3 position)
    {   
        ParticleSystem EnemyBloodInstance = Instantiate(EnemyBlood, position, transform.rotation);
    }

    public void TriggerPlayerBlood(Vector3 position)
    {
        ParticleSystem PlayerBloodInstance = Instantiate(PlayerBlood, position, transform.rotation);
    }
    private void OnDrawGizmos()
    {
    
    }

}
