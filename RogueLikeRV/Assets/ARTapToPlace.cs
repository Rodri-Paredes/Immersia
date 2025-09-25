using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlace : MonoBehaviour
{
    [SerializeField] private GameObject model3DPrefab; // Tu prefab del juego
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;

    private GameObject placedObject;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        // Si ya colocamos el objeto, no hacemos nada más
        if (placedObject != null) return;

        // Detectar toque en pantalla
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Raycast contra planos detectados
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    // Posición del primer plano detectado
                    Pose hitPose = hits[0].pose;

                    // Instanciar el prefab
                    placedObject = Instantiate(model3DPrefab, hitPose.position, hitPose.rotation);

                    // Opcional: que mire hacia la cámara (solo en eje Y)
                    Vector3 lookPos = Camera.main.transform.position;
                    lookPos.y = placedObject.transform.position.y;
                    placedObject.transform.LookAt(lookPos);

                    // Apagar detección de planos después de colocar el objeto
                    StopPlaneDetection();
                }
            }
        }
    }

    private void StopPlaneDetection()
    {
        planeManager.requestedDetectionMode = PlaneDetectionMode.None;

        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }
}
