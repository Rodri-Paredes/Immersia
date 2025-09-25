using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public Vector3 offset; // Desplazamiento de la cámara respecto al jugador

    void Start()
    {
        // Configurar el desplazamiento inicial si no se ha asignado manualmente
        if (offset == Vector3.zero && player != null)
        {
            // Establecer la distancia entre la cámara y el jugador en el inicio
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        // Mantener la posición de la cámara respecto al jugador
        if (player != null)
        {
            Vector3 newPosition = player.position + offset;
            // Ajustar solo la posición, no la rotación
            transform.position = newPosition;
        }
    }
}
