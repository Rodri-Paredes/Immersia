using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogToUI : MonoBehaviour
{
    public Text logText; // Referencia al Text UI donde se mostrarán los logs
    public float fadeDuration = 2f; // Duración del desvanecimiento en segundos
    public float displayDuration = 5f; // Duración antes de comenzar a desvanecer

    private void OnEnable()
    {
        // Subscribirse al evento de log
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        // Desubscribirse del evento de log
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Añadir el nuevo log al Text UI
        logText.text = logString;

        // Resetear la transparencia al 100%
        Color originalColor = logText.color;
        logText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        // Detener cualquier corrutina de desvanecimiento anterior y empezar de nuevo
        StopAllCoroutines();
        StartCoroutine(FadeOutLog());
    }

    IEnumerator FadeOutLog()
    {
        // Esperar antes de empezar a desvanecer el texto
        yield return new WaitForSeconds(displayDuration);

        // Capturar el color inicial del texto
        Color originalColor = logText.color;

        // Proceso de desvanecimiento
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            // Calcular la nueva transparencia
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            logText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Asegurarse de que el texto sea completamente transparente
        logText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Limpiar el contenido del texto
        logText.text = "";
    }
}
