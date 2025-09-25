using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Array de prefabs de enemigos
    public Transform spawnPoint; // Punto alrededor del cual se generarán los enemigos
    public float spawnRangeX = 5f; // Rango en el eje X para la generación de enemigos
    public float spawnRangeZ = 5f; // Rango en el eje Z para la generación de enemigos
    public GameObject objectToActivate; // GameObject que se activará cuando se generen los enemigos
    public GameObject finalObjectToActivate; // GameObject que se activará después de las rondas
    public float regenerationDelay = 5f; // Tiempo en segundos para esperar antes de regenerar enemigos
    public int maxRounds = 3; // Número máximo de rondas para regenerar enemigos

    private bool enemiesGenerated = false;
    private int currentRound = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !enemiesGenerated)
        {
            GenerateEnemies();
            objectToActivate.SetActive(true);
            enemiesGenerated = true;
            StartCoroutine(CheckForEnemies());
        }
    }

    IEnumerator CheckForEnemies()
    {
        while (currentRound < maxRounds)
        {
            // Espera el tiempo definido antes de comprobar si hay enemigos
            yield return new WaitForSeconds(regenerationDelay);

            // Comprobar si hay hijos (enemigos) en el objeto
            if (transform.childCount == 0)
            {
                if (currentRound < maxRounds - 1)
                {
                    // Si no hay enemigos y no hemos alcanzado el máximo de rondas, generar nuevos enemigos
                    GenerateEnemies();
                    currentRound++;
                }
                else
                {
                    // Si se ha alcanzado el máximo de rondas, desactivar el GameObject y destruir el generador
                    objectToActivate.SetActive(false);
                    finalObjectToActivate.SetActive(true); // Activar el GameObject final
                    Destroy(gameObject); // Destruir el generador de enemigos
                }
            }
        }
    }

    void GenerateEnemies()
    {
        // Número total de enemigos a generar
        int totalEnemies = Random.Range(1, 3); // Genera entre 1 y 5 enemigos por cuarto

        for (int i = 0; i < totalEnemies; i++)
        {
            // Seleccionar un prefab de enemigo aleatorio
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Calcular una posición aleatoria alrededor del punto de generación
            float randomX = Random.Range(-spawnRangeX, spawnRangeX);
            float randomZ = Random.Range(-spawnRangeZ, spawnRangeZ);
            Vector3 randomPosition = spawnPoint.position + new Vector3(randomX, 0, randomZ);

            // Instanciar el enemigo en la posición calculada y hacer que sea hijo del objeto que contiene este script
            Instantiate(enemyPrefab, randomPosition, Quaternion.identity, transform);
        }
    }
}
