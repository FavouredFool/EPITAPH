using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] ParticleSystem GlassBreaking;
    [SerializeField] EnvObjAudioData audioData;

    EnvObjAudio audio;

    private void Awake()
    {
        audio = audioData.Setup(this.gameObject);
    }

    public void BreakWall()
    {
        Instantiate(GlassBreaking.gameObject, transform.position, transform.rotation);
        gameObject.SetActive(false);
        audio.PlayInteractionEvent();
    }
}
