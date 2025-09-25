using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    private Camera camera1; // Primera cámara
    public Camera camera2; // Segunda cámara
    public Button switchCameraButton; // Botón que cambiará la cámara

    void Start()
    {
        // Buscar la cámara con el tag "camaraAR"
        camera1 = GameObject.FindWithTag("camaraAR").GetComponent<Camera>();

        // Asegúrate de que solo una cámara esté activa al inicio
        SetActiveCamera(camera1, true);

        // Asignar el método al evento onClick del botón
        switchCameraButton.onClick.AddListener(SwitchCamera);
    }

    void SwitchCamera()
    {
        // Alternar entre las cámaras
        if (camera1.gameObject.activeSelf)
        {
            SetActiveCamera(camera2, true);
        }
        else
        {
            SetActiveCamera(camera1, true);
        }
    }

    void SetActiveCamera(Camera cam, bool isActive)
    {
        // Activar la cámara seleccionada y desactivar la otra
        cam.gameObject.SetActive(isActive);

        // Desactivar la otra cámara
        if (cam == camera1)
        {
            camera2.gameObject.SetActive(!isActive);
        }
        else
        {
            camera1.gameObject.SetActive(!isActive);
        }
    }
}
