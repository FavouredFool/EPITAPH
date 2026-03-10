using UnityEngine;

public class AimAssistV3 : MonoBehaviour
{
    // TODO Distance Used For Lock on Decision
    [Range(0f, 15f)] 
    [SerializeField] float _maxDistance = 10f;
    
    [Range(0f, 30f)]
    [SerializeField] float maxAssistAngle = 10f;
    
    [Range(0f, 16f)]
    [SerializeField] float smoothingPower = 1;
    
    [SerializeField] Transform[] enemies;

    public float GetAssistedAngle(float playerAimAngle, Vector2 playerPos)
    {
        if (!enabled || !gameObject.activeSelf) return playerAimAngle;
        
        // TODO lots of stuff to improve here. Distance, Block behind Walls, etc.
        
        float bestDiff = Mathf.Infinity;
        float bestAngle = playerAimAngle;
        
        foreach (var enemy in enemies)
        {
            if (!enemy) continue;

            Vector2 dir = (Vector2)enemy.position - playerPos;

            if (dir.sqrMagnitude > Mathf.Pow(_maxDistance, 2)) continue;

            float enemyAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

            float diff = Mathf.Abs(Mathf.DeltaAngle(playerAimAngle, enemyAngle));

            if (diff < bestDiff && diff <= maxAssistAngle)
            {
                bestDiff = diff;
                bestAngle = enemyAngle;
            }
        }
        
        if (float.IsPositiveInfinity(bestDiff))
            return playerAimAngle;
        
        float tDistance = Mathf.Clamp01(bestDiff / maxAssistAngle);
        
        float foundAngle = Mathf.LerpAngle(bestAngle, playerAimAngle, SmoothStep(tDistance, smoothingPower));

        return foundAngle;
    }
    
    float SmoothStep(float x, float power)
    {
        x = Mathf.Clamp01(x);
        x = Mathf.Pow(x, power);
        return x * x * (3f - 2f * x);
    }
}
