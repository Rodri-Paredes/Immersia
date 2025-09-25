using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fantasma : MonoBehaviour
{
    public float patrolSpeed = 2f;            // Velocidad de patrullaje
    public float chaseSpeed = 4f;             // Velocidad de persecución
    public float detectionRange = 10f;        // Rango de detección del jugador
    public float fireRate = 2f;               // Frecuencia de disparo en segundos
    public float fireDistance = 7f;           // Distancia mínima para disparar al jugador
    public float obstacleDetectionDistance = 1f; // Distancia a la que detecta obstáculos
    public GameObject projectilePrefab;       // Prefab del proyectil
    public float projectileSpeed = 10f;       // Velocidad del proyectil
    public float projectileLifetime = 3f;     // Tiempo de vida del proyectil
    public LayerMask groundLayer;             // Capa del suelo para la detección de colisiones
    public LayerMask obstacleLayer;           // Capa de obstáculos para evitar colisiones
    public Transform firePoint;                // Punto de disparo

    private Transform player;
    private Vector3 randomDirection;
    private float nextFireTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetRandomDirection();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer >= fireDistance)
            {
                ChasePlayer();
            }
            else
            {
                StopAndFire();
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

    void StopAndFire()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Evita la rotación en Y

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chaseSpeed);

        if (Time.time >= nextFireTime)
        {
            Fire(direction);
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire(Vector3 direction)
    {
        if (firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
            Destroy(projectile, projectileLifetime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
