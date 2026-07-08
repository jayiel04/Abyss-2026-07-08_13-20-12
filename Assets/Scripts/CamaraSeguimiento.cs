using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Offset de la cámara respecto al jugador")]
    public Vector3 cameraOffset = new Vector3(0f, 1f, -3.5f);

    [Header("Configuración de seguimiento")]
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 newPosition = transform.position;

        if (followX)
            newPosition.x = target.position.x + cameraOffset.x;

        if (followY)
            newPosition.y = target.position.y + cameraOffset.y;

        if (followZ)
            newPosition.z = target.position.z + cameraOffset.z;

        transform.position = newPosition;
    }
}