using UnityEngine;

[RequireComponent(typeof(CameraFollow))]
public class CameraSmooth : MonoBehaviour
{
    [Header("Suavizado")]
    [Min(0f)]
    public float smoothTime = 0.15f;

    private CameraFollow cameraFollow;
    private Vector3 velocity;

    private void Awake()
    {
        cameraFollow = GetComponent<CameraFollow>();
    }

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            cameraFollow.TargetPosition,
            ref velocity,
            smoothTime
        );
    }
}
