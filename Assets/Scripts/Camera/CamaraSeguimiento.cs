using TMPro;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Offset respecto al jugador")]
    public Vector3 cameraOffset = new Vector3(0f, 0f, 0f);

    [Header("Configuración")]
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    /// <summary>
    /// Posición que la cámara debería alcanzar.
    /// </summary>
    public Vector3 TargetPosition { get; private set; }

    private void Update()
    {
        if (target == null)
            return;

        Vector3 targetPosition = transform.position;

        if (followX)
            targetPosition.x = target.position.x + cameraOffset.x;

        if (followY)
            targetPosition.y = target.position.y + cameraOffset.y;

        if (followZ)
            targetPosition.z = target.position.z + cameraOffset.z;

        TargetPosition = targetPosition;
    }
}