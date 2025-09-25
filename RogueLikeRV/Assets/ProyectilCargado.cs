using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilCargado : MonoBehaviour
{
    public float speed = 10f; // Velocidad del proyectil

    void Start()
    {
        // Destruir el proyectil si no ha colisionado con un enemigo en un tiempo específico
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Mover el proyectil hacia adelante
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Si el proyectil colisiona con un objeto con el tag "Enemy"

        if (other.CompareTag("Wall"))
        {
            // Puedes añadir aquí el código para dañar al enemigo
            Destroy(gameObject); // Destruir el proyectil al colisionar con un enemigo
        }
    }
}
