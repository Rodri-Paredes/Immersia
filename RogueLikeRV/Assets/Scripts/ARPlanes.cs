using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlanes : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject model3DPrefab;

    private GameObject placedObject;

    private void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Solo colocar el objeto una vez
        if (placedObject != null) return;

        foreach (var plane in args.added)
        {
            if (plane.size.x * plane.size.y >= 0.4f)
            {
                // Instanciar el prefab exactamente en el centro del plano
                placedObject = Instantiate(
                    model3DPrefab,
                    plane.center,
                    Quaternion.identity
                );

                // Detener la detección de planos
                StopPlaneDetection();
                break;
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
