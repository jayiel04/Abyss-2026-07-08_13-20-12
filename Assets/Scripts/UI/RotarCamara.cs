using UnityEngine;

public class RotarCamara : MonoBehaviour
{
    [SerializeField] private float velocidadRotacion = 3f;

    void Update()
    {
        transform.Rotate(0f, velocidadRotacion * Time.deltaTime, 0f);
    }
}
