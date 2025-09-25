using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class VidaPlayer : MonoBehaviour
{
    [Header("Stats")]
    public int vida = 100;
    public int maxVida = 100;
    public int monedas = 0;

    [Header("UI")]
    public GameObject gameOverPanel;
    public Button revivirButton;
    public Button salirButton;
    public Button fireButton;

    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv != null && !pv.IsMine) return; // Solo jugador local

        // Cargar valores
        vida = GameData.playerVida;
        maxVida = GameData.playerMaxVida;
        monedas = GameData.playerMonedas;

        // Buscar panel y botones en escena si no están asignados
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");

        if (revivirButton == null)
            revivirButton = GameObject.Find("RevivirButton")?.GetComponent<Button>();

        if (salirButton == null)
            salirButton = GameObject.Find("SalirButton")?.GetComponent<Button>();

        if (fireButton == null)
            fireButton = GameObject.Find("FireButton")?.GetComponent<Button>();

        // Configurar botones
        if (revivirButton != null) revivirButton.onClick.AddListener(Revivir);
        if (salirButton != null) salirButton.onClick.AddListener(Salir);
        if (fireButton != null) fireButton.onClick.AddListener(HandleFireButtonPress);

        // Asegurarse de que el panel de GameOver esté desactivado
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pv != null && !pv.IsMine) return; // Solo jugador local

        if (other.CompareTag("Enemy"))
            RestarVida(10);
        else if (other.CompareTag("Vida"))
        {
            CurarVida(20);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Moneda"))
        {
            AumentarMonedas(10);
            Destroy(other.gameObject);
        }
    }

    void RestarVida(int cantidad)
    {
        vida -= cantidad;
        if (vida <= 0)
        {
            vida = 0;
            DesactivarJuego();
        }
    }

    void CurarVida(int cantidad)
    {
        vida = Mathf.Min(vida + cantidad, maxVida);
    }

    void AumentarMonedas(int cantidad)
    {
        monedas += cantidad;
    }

    void DesactivarJuego()
    {
        if (pv != null && !pv.IsMine) return; // Solo jugador local

        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            ActualizarEstadoRevivir();
        }
    }

    void Revivir()
    {
        if (monedas >= 50)
        {
            monedas -= 50;
            vida = Mathf.Min(vida + 50, maxVida);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            Salir();
        }
    }

    void Salir()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void ActualizarEstadoRevivir()
    {
        if (revivirButton != null)
            revivirButton.interactable = monedas >= 50;
    }

    void ApplyRandomUpgrade()
    {
        int choice = Random.Range(0, 3);
        switch (choice)
        {
            case 0:
                maxVida += 10;
                vida = Mathf.Min(vida, maxVida);
                Debug.Log("Mejora obtenida: Aumento de vida máxima.");
                break;
            case 1:
                CurarVida(25);
                Debug.Log("Posion obtenida: +25 vida.");
                break;
            case 2:
                AumentarMonedas(30);
                Debug.Log("Botin obtenido: +30 monedas.");
                break;
        }
    }

    void HandleFireButtonPress()
    {
        if (pv != null && !pv.IsMine) return; // Solo jugador local

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("CofreNormal"))
            {
                ApplyRandomUpgrade();
                Destroy(hit.gameObject);
                break;
            }
        }
    }

    void OnDestroy()
    {
        if (pv != null && !pv.IsMine) return; // Solo jugador local

        GameData.playerVida = vida;
        GameData.playerMaxVida = maxVida;
        GameData.playerMonedas = monedas;
    }
}
