using System.Collections;
using UnityEngine;

public class BossSimple : MonoBehaviour
{
    [Header("Movimiento")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 12f;
    public float fireDistance = 8f;
    public float obstacleDetectionDistance = 1f;
    public LayerMask obstacleLayer;

    [Header("Ataques")]
    public float fireRate = 2f; // frecuencia de disparo normal
    public float specialAttackCooldown = 8f; // cada cuánto tiempo hace el especial
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f;
    public float projectileLifetime = 3f;
    public Transform firePoint;

    private Transform player;
    private Vector3 randomDirection;
    private float nextFireTime = 0f;
    private float nextSpecialAttackTime = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetRandomDirection();
        nextSpecialAttackTime = Time.time + specialAttackCooldown; // inicia el timer del especial
    }

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
                AttackPlayer();
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
            SetRandomDirection();
        }
        else
        {
            Vector3 movement = randomDirection * patrolSpeed * Time.deltaTime;
            movement.y = 0;
            transform.position += movement;
        }

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
        direction.y = 0; // 👈 ignoramos Y

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chaseSpeed);

        transform.position += direction * chaseSpeed * Time.deltaTime;
    }

    void AttackPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // 👈 ignoramos Y

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * chaseSpeed);

        // Ataque básico
        if (Time.time >= nextFireTime)
        {
            Fire(direction);
            nextFireTime = Time.time + fireRate;
        }

        // Ataque especial
        if (Time.time >= nextSpecialAttackTime)
        {
            StartCoroutine(SpecialAttack(direction));
            nextSpecialAttackTime = Time.time + specialAttackCooldown;
        }
    }

    void Fire(Vector3 direction)
    {
        if (firePoint != null && projectilePrefab != null)
        {
            direction.y = 0f; // 🔹 Mantener plano en Y

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
            Destroy(projectile, projectileLifetime);
        }
    }


    IEnumerator SpecialAttack(Vector3 direction)
    {
        direction.y = 0f; // 🔹 también aquí

        int numProjectiles = 3;
        float angleSpread = 20f;

        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = (i - 1) * angleSpread;
            Quaternion spreadRotation = Quaternion.Euler(0, angle, 0);
            Vector3 spreadDirection = (spreadRotation * direction).normalized;
            spreadDirection.y = 0f; // 🔹 igual en cada bala

            Fire(spreadDirection);
        }

        yield return null;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
