using TMPro.EditorUtilities;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class PlayerCameraController : MonoBehaviour
{
    public static CinemachineCamera cam;
    public CinemachineCamera _cam;

    void Awake()
    {
        cam=_cam;
        Debug.LogWarning("TODO: Impliment player camera controller fow stuff");
    }

    public static void SetFOV(float fov)
    {
        cam.Lens.FieldOfView=fov;
        Debug.Log("SET FOV "+fov);
    }

    public static void DoFOV(float fovTarget, float time, Ease ease)
    {
        return;
        DOTween.Kill(cam);
        Sequence seq= DOTween.Sequence(cam).SetUpdate(true);

        float fov = cam.Lens.FieldOfView;
        seq.Insert(0, DOTween.To(() => fov, x => fov = x, fovTarget, time).SetEase(ease).OnComplete(() =>
        {
            SetFOV(fov);
        }));
    }
}