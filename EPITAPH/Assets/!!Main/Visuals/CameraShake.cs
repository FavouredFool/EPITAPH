using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{



    public CinemachineImpulseSource impulseSource;
    public static CameraShake Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }



    public void TriggerBasicShake()
    {
        TriggerShake(Random.insideUnitCircle, 2f);
    }


    public void TriggerShake(Vector3 velocity,float intensity)
    {
        impulseSource.GenerateImpulse(velocity * intensity);

    }
}
