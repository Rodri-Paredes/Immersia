using UnityEngine;

public class VidaObjeto : MonoBehaviour
{
    [Header("Vida y Loot")]
    public int vida = 100;
    public GameObject[] lootPrefabs; // Prefabs que puede soltar
    public float dropChance = 20f;   // Probabilidad de drop (%)

    [Header("Opciones de destrucción")]
    public bool destruirPadre = false; // 👈 aparece en el inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ProyectilPlayer"))
        {
            RestarVida(10);
            Destroy(other.gameObject);
        }
    }

    void RestarVida(int cantidad)
    {
        vida -= cantidad;

        if (vida <= 0)
        {
            DestruirObjeto();
        }
    }

    void DestruirObjeto()
    {
        // Intenta soltar loot
        TryDropLoot();

        // Solo destruye al padre si está marcado en el inspector
        if (destruirPadre && transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }

        // Siempre destruye este objeto
        Destroy(gameObject);
    }

    void TryDropLoot()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < dropChance && lootPrefabs.Length > 0)
        {
            GameObject lootPrefab = lootPrefabs[Random.Range(0, lootPrefabs.Length)];
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }
    }
}
