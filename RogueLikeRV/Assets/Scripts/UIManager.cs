using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public Image healthBar; // Barra de vida
    public Text coinsText;
    public Text levelText;

    private VidaPlayer vidaPlayer;

    void Start()
    {
        // Buscar el player local instanciado por Photon
        StartCoroutine(FindLocalPlayer());
    }

    IEnumerator FindLocalPlayer()
    {
        while (vidaPlayer == null)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                PhotonView pv = obj.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    vidaPlayer = obj.GetComponent<VidaPlayer>();
                    break;
                }
            }
            yield return null;
        }
    }

    void Update()
    {
        if (vidaPlayer == null) return;

        UpdateHealthBar();
        UpdateCoinsText();
        UpdateLevelText();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float healthPercentage = Mathf.Clamp01((float)vidaPlayer.vida / vidaPlayer.maxVida);
            healthBar.fillAmount = healthPercentage;
        }
    }

    void UpdateCoinsText()
    {
        if (coinsText != null)
            coinsText.text = "Monedas: " + vidaPlayer.monedas;
    }

    void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = "Nivel: " + GameData.nivel;
    }
}
