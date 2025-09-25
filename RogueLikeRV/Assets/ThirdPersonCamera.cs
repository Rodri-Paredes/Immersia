using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; // El jugador
    public Vector3 offset = new Vector3(0, 2, -4); // Distancia de la cámara respecto al jugador
    public float sensitivity = 3f; // Sensibilidad del mouse
    public float rotationSmoothTime = 0.12f; // Suavidad de rotación

    private float yaw;   // Rotación en Y (horizontal)
    private float pitch; // Rotación en X (vertical)
    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;

    void Start()
    {
        // Comenzar mirando a -90 en Y
        yaw = -90f;
        pitch = 0f;

        currentRotation = new Vector3(pitch, yaw);
        transform.eulerAngles = currentRotation;
        transform.position = target.position + Quaternion.Euler(currentRotation) * offset;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Rotación con el mouse
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;

        // Limitar ángulo vertical
        pitch = Mathf.Clamp(pitch, -30, 60);

        // Suavizar rotación
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);

        // Aplicar rotación a la cámara
        transform.eulerAngles = currentRotation;

        // Mantener posición con respecto al jugador
        transform.position = target.position + Quaternion.Euler(currentRotation) * offset;
    }
}
