using UnityEngine;

public class Enemy : MonoBehaviour
{
    public delegate void DeathAction();
    public event DeathAction OnDeath;

    void OnDestroy()
    {
        // Notificar al sistema que el enemigo ha sido destruido
        if (OnDeath != null)
        {
            OnDeath();
        }
    }
}
