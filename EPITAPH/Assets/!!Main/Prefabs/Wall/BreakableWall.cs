using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] ParticleSystem GlassBreaking;
    public void BreakWall()
    {
        gameObject.SetActive(false);
    }
}
