using System.Collections;
using UnityEngine;
using Photon.Pun;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 10, -10);
    public float smoothSpeed = 0.125f;

    private Transform player;

    void Start()
    {
        // Iniciar corutina para esperar al jugador
        StartCoroutine(FindLocalPlayer());
    }

    IEnumerator FindLocalPlayer()
    {
        // Esperar hasta que el jugador local exista
        while (player == null)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                PhotonView pv = obj.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    player = obj.transform;
                    break;
                }
            }
            yield return null; // espera un frame y vuelve a intentar
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(player);
    }
}
