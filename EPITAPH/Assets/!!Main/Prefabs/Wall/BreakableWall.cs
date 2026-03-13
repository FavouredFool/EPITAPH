using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] ParticleSystem GlassBreaking;
    [SerializeField] EnvObjAudioData audioData;

    EnvObjAudio audio;

    private void Awake()
    {
        audio = audioData?.Setup(this.gameObject);
    }

    public void BreakWall()
    {
        Instantiate(GlassBreaking.gameObject, transform.position, transform.rotation);
        CameraShake.Instance.TriggerShake(Random.insideUnitSphere, 0.2f);
        gameObject.SetActive(false);
        audio?.PlayInteractionEvent();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() is { } player)
        {
            if (player.StateMachine.CurrentState is LungeState)
            {
                BreakWall();
            }
        }
    }
}
