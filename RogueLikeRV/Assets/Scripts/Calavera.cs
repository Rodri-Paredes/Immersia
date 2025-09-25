using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calavera : MonoBehaviour
{
    public float patrolSpeed = 2f;             // Velocidad de patrullaje
    public float chaseSpeed = 4f;              // Velocidad de persecución
    public float detectionRange = 10f;         // Rango de detección del jugador
    public float attackDistance = 2f;          // Distancia mínima para atacar al jugador
    public float obstacleDetectionDistance = 1f; // Distancia a la que detecta obstáculos
    public LayerMask groundLayer;              // Capa del suelo para la detección de colisiones
    public LayerMask obstacleLayer;            // Capa de obstáculos para evitar colisiones
    public float safeDistance = 3f;            // Distancia segura mínima después de teletransportarse

    private Transform player;
    private Vector3 randomDirection;
    private Queue<Vector3> positionHistory;    // Cola para almacenar la historia de posiciones
    private float historyDuration = 4f;        // Duración en segundos para almacenar la historia

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetRandomDirection();
        positionHistory = new Queue<Vector3>();
        StartCoroutine(StorePositionHistory());
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer >= attackDistance)
            {
                ChasePlayer();
            }
            else
            {
                TeleportToPreviousPosition();
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (Physics.Raycast(transform.position, randomDirection, obstacleDetectionDistance, obstacleLayer))
        {
            SetRandomDirection(); // Cambia de dirección al detectar un obstáculo
        }
        else
        {
            Vector3 movement = randomDirection * patrolSpeed * Time.deltaTime;
            movement.y = 0; // Evita el movimiento en Y
            transform.position += movement;
        }

        // Mantén la rotación solo en el eje Y
        Quaternion targetRotation = Quaternion.LookRotation(randomDirection);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * patrolSpeed);
    }

    void SetRandomDirection()
    {
        randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Evita la rotación en Y

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chaseSpeed);

        transform.position += direction * chaseSpeed * Time.deltaTime;
    }

    void TeleportToPreviousPosition()
    {
        if (positionHistory.Count > 0)
        {
            Vector3 teleportPosition = positionHistory.Dequeue();

            // Mantén la posición en Y actual
            teleportPosition.y = transform.position.y;

            // Verifica si el jugador está cerca
            float distanceToPlayer = Vector3.Distance(teleportPosition, player.position);
            if (distanceToPlayer < safeDistance)
            {
                // Aumenta la distancia teletransportándote un poco más lejos del jugador
                Vector3 directionFromPlayer = (teleportPosition - player.position).normalized;
                teleportPosition += directionFromPlayer * safeDistance;

                // Mantén la posición en Y actual después de ajustar la distancia
                teleportPosition.y = transform.position.y;
            }

            transform.position = teleportPosition; // Teletransporta a la posición calculada
        }
        else
        {
            Debug.LogWarning("No se encontró una posición en la historia para teletransportarse.");
        }
    }

    private IEnumerator StorePositionHistory()
    {
        while (true)
        {
            if (positionHistory.Count >= historyDuration / Time.fixedDeltaTime)
            {
                positionHistory.Dequeue(); // Elimina la posición más antigua si la historia supera los 3 segundos
            }
            positionHistory.Enqueue(transform.position); // Almacena la posición actual
            yield return new WaitForSeconds(Time.fixedDeltaTime); // Espera un frame fijo antes de almacenar la siguiente posición
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportToPreviousPosition();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
