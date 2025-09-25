using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duende : MonoBehaviour
{
    public float patrolSpeed = 2f;            // Velocidad de patrullaje
    public float chargeSpeed = 10f;           // Velocidad del impulso
    public float detectionRange = 10f;        // Rango de detección del jugador
    public float chargeTime = 2f;             // Tiempo de recarga antes del impulso
    public float obstacleDetectionDistance = 1f; // Distancia a la que detecta obstáculos
    public LayerMask groundLayer;             // Capa del suelo para la detección de colisiones
    public LayerMask obstacleLayer;           // Capa de obstáculos para evitar colisiones

    private Transform player;
    private Vector3 randomDirection;
    private bool isCharging = false;
    private bool isCoolingDown = false;       // Indica si está en periodo de recarga después de un impulso

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetRandomDirection();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!isCharging && !isCoolingDown)
            {
                StopAndCharge();
            }
        }
        else
        {
            Patrol();
        }

        if (isCoolingDown)
        {
            RotateTowardsPlayer();
        }
    }

    void Patrol()
    {
        if (Physics.Raycast(transform.position, randomDirection, out RaycastHit hit, obstacleDetectionDistance, obstacleLayer))
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

    void StopAndCharge()
    {
        isCharging = true;

        Vector3 chargeDirection = (player.position - transform.position).normalized;
        chargeDirection.y = 0; // Evita la rotación en Y

        // Rotar hacia el jugador
        Quaternion targetRotation = Quaternion.LookRotation(chargeDirection);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = targetRotation;

        StartCoroutine(ChargeRoutine(chargeDirection));
    }

    IEnumerator ChargeRoutine(Vector3 direction)
    {
        float impulseDistance = 3f; // Distancia máxima del impulso
        float traveledDistance = 0f;

        while (traveledDistance < impulseDistance)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, obstacleDetectionDistance, obstacleLayer))
            {
                // Detiene el impulso al detectar un obstáculo
                transform.position = hit.point - direction * 0.1f; // Ajusta la posición para evitar atravesar el obstáculo
                break;
            }

            Vector3 movement = direction * chargeSpeed * Time.deltaTime;
            transform.position += movement;
            traveledDistance += movement.magnitude;

            yield return null;
        }

        isCharging = false;
        StartCoroutine(CoolDownRoutine());
    }

    IEnumerator CoolDownRoutine()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(chargeTime);
        isCoolingDown = false;
    }

    void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Evita la rotación en Y

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * patrolSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
