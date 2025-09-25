using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSystem : MonoBehaviour
{
    public GameObject magicWandProjectilePrefab; // Prefab del proyectil del bastón mágico
    public Transform firePoint; // Punto desde donde se disparan los proyectiles
    public float projectileLifetime = 3f; // Tiempo de vida del proyectil antes de desaparecer
    public float fireCooldown = 0.3f; // Tiempo de enfriamiento entre disparos en segundos
    public bool isDoubleShotEnabled = false; // Mejora de arma: disparar dos proyectiles
    public float doubleShotOffset = 0.2f; // Separación entre los dos proyectiles
    public float projectileSizeMultiplier = 1f; // Multiplicador del tamaño del proyectil

    public GameObject posicionInicialPlayer; // Variable pública para asignar el GameObject de la posición inicial del jugador

    private bool canShoot = true; // Booleano para controlar si el jugador puede disparar
    private GameObject currentCofreGrande = null; // Referencia al CofreGrande con el que se colisiona

    void Start()
    {
        // Cargar los valores almacenados
        fireCooldown = GameData.weaponFireCooldown;
        isDoubleShotEnabled = GameData.weaponIsDoubleShotEnabled;
        projectileSizeMultiplier = GameData.weaponProjectileSizeMultiplier;
        GameData.weaponNivelContador = GameData.weaponNivelContador;
    }

    // Método para disparar el proyectil
    public void Shoot()
    {
        if (magicWandProjectilePrefab && firePoint && canShoot)
        {
            if (currentCofreGrande != null)
            {
                // Si hay un CofreGrande, aplicar la mejora
                ApplyRandomUpgrade();
                StartCoroutine(HandleCofreGrandeDestruction(currentCofreGrande));
                currentCofreGrande = null; // Reiniciar la referencia después de aplicar la mejora
            }
            else
            {
                // Disparar proyectiles normalmente
                if (isDoubleShotEnabled)
                {
                    // Disparar dos proyectiles con una pequeña separación
                    Vector3 offset = firePoint.right * doubleShotOffset;
                    ShootProjectile(firePoint.position + offset);
                    ShootProjectile(firePoint.position - offset);
                }
                else
                {
                    // Disparar un solo proyectil
                    ShootProjectile(firePoint.position);
                }

                // Iniciar el cooldown
                StartCoroutine(FireCooldown());
            }
        }
    }

    void ShootProjectile(Vector3 position)
    {
        // Crear el proyectil en la posición especificada
        GameObject projectile = Instantiate(magicWandProjectilePrefab, position, firePoint.rotation);

        // Aplicar el multiplicador de tamaño al proyectil
        projectile.transform.localScale *= projectileSizeMultiplier;

        // Configurar el tiempo de vida del proyectil
        Destroy(projectile, projectileLifetime);
    }

    IEnumerator FireCooldown()
    {
        // Desactivar la capacidad de disparar
        canShoot = false;

        // Esperar el tiempo de enfriamiento
        yield return new WaitForSeconds(fireCooldown);

        // Reactivar la capacidad de disparar
        canShoot = true;
    }

    void ApplyRandomUpgrade()
    {
        int upgradeChoice = Random.Range(0, 3);

        switch (upgradeChoice)
        {
            case 0:
                isDoubleShotEnabled = true;
                Debug.Log("Mejora obtenida: Disparo doble activado.");
                break;
            case 1:
                fireCooldown = Mathf.Max(0.1f, fireCooldown - 0.1f); // Evitar que el cooldown sea menor a 0.1
                Debug.Log("Mejora obtenida: Reducción del tiempo de enfriamiento.");
                break;
            case 2:
                projectileSizeMultiplier += 0.05f;
                Debug.Log("Mejora obtenida: Aumento del tamaño del proyectil.");
                break;
        }
    }

    IEnumerator HandleCofreGrandeDestruction(GameObject cofreGrande)
    {
        // Destruir el CofreGrande
        Destroy(cofreGrande);
        // Incrementar el contador de nivel
        GameData.nivel++;
        // Esperar 3 segundos
        yield return new WaitForSeconds(3f);

        // Incrementar el contador de nivel
        GameData.weaponNivelContador++;

        // Almacenar los valores actuales antes de recargar la escena
        GameData.weaponFireCooldown = fireCooldown;
        GameData.weaponIsDoubleShotEnabled = isDoubleShotEnabled;
        GameData.weaponProjectileSizeMultiplier = projectileSizeMultiplier;
        GameData.weaponNivelContador = GameData.weaponNivelContador;

        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CofreGrande"))
        {
            // Si colisiona con un CofreGrande, almacenar la referencia
            currentCofreGrande = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CofreGrande"))
        {
            // Si sale del área del CofreGrande, eliminar la referencia
            if (currentCofreGrande == other.gameObject)
            {
                currentCofreGrande = null;
            }
        }
    }
}
