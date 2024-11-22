using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // Variables principales
    public Slider healthBar;          // Barra de vida (UI)
    public float health;              // Salud actual del jugador
    public float maxHealth;           // Salud máxima del jugador
    public bool isInmune;            // Estado de inmunidad
    public float inmunityTime;        // Duración de la inmunidad
    private SpriteRenderer sprite;    // Sprite del jugador
    public GameObject gameOverImg;    // Imagen de Game Over
    private Rigidbody2D rb;           // Rigidbody del jugador para retroceso
    public float knockbackForce = 4f; // Fuerza del retroceso
    public float knockbackDuration = 0.3f; // Duración del retroceso
    public bool isDead;               // Verificar si el jugador está muerto

    void Start()
    {
        // Inicializar variables
        gameOverImg.SetActive(false);        // Ocultar pantalla de Game Over
        sprite = GetComponent<SpriteRenderer>(); // Obtener el sprite del jugador
        health = maxHealth;                 // Salud inicial igual a la máxima
        rb = GetComponent<Rigidbody2D>();   // Obtener Rigidbody2D del jugador

        // Configuración inicial de la barra de vida
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    void Update()
    {
        // Evitar que la salud exceda el máximo
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        // Actualizar la barra de vida
        healthBar.value = health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Recolectar ítems de salud
        if (collision.CompareTag("HealthItem"))
        {
            RestoreHealth(20f);          // Restaurar 20 puntos de salud
            Destroy(collision.gameObject); // Eliminar el ítem
        }

        // Daño recibido de enemigos
        if ((collision.CompareTag("Enemigo") || collision.gameObject.layer == LayerMask.NameToLayer("Enemigo")) && !isInmune)
        {
            float damage = 10f; // Daño del enemigo
            health -= damage;   // Reducir la salud

            // Retroceso
            Vector2 knockbackDirection = transform.position - collision.transform.position;
            knockbackDirection.Normalize();
            StartCoroutine(ApplyKnockback(knockbackDirection));

            // Activar inmunidad temporal
            StartCoroutine(Inmunity());

            // Verificar si el jugador murió
            if (health <= 0)
            {
                health = 0; // Asegurar que no baje de 0
                Die();
            }
        }
    }

    public void RestoreHealth(float amount)
    {
        // Restaurar la salud
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth; // Limitar al máximo
        }
        healthBar.value = health; // Actualizar la barra de vida
        Debug.Log("Salud restaurada: " + health);

        // Feedback visual al recoger salud
        StartCoroutine(HealthPickupFeedback());
    }

    public void Die()
    {
        isDead = true;
        AudioManager.instance.lvlbgmsc.Stop();
        Time.timeScale = 0; // Pausar el juego
        gameOverImg.SetActive(true); // Mostrar imagen de Game Over
        StartCoroutine(FadeInGameOver());
        Debug.Log("Has muerto");
    }

    IEnumerator FadeInGameOver()
    {
        CanvasGroup canvasGroup = gameOverImg.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component missing on GameOver image!");
            yield break;
        }

        canvasGroup.alpha = 0f; // Iniciar con alpha 0
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime * 0.5f; // Velocidad de fade in
            yield return null; // Esperar al siguiente frame
        }
        canvasGroup.alpha = 1f; // Asegurar que termine en 1
    }

    public IEnumerator ApplyKnockback(Vector2 direction)
    {
        float timer = 0;
        while (timer < knockbackDuration)
        {
            timer += Time.deltaTime;
            rb.velocity = new Vector2(direction.x * knockbackForce, rb.velocity.y); // Aplicar fuerza de retroceso
            yield return null;
        }
        rb.velocity = Vector2.zero; // Detener movimiento tras el retroceso
    }

    public IEnumerator Inmunity()
    {
        isInmune = true;             // Activar inmunidad
        sprite.color = Color.red;    // Cambiar color a rojo
        yield return new WaitForSeconds(inmunityTime); // Esperar el tiempo de inmunidad
        sprite.color = Color.white; // Restaurar color original
        isInmune = false;            // Desactivar inmunidad
    }

    IEnumerator HealthPickupFeedback()
    {
        sprite.color = Color.green;  // Cambiar color a verde al recoger salud
        yield return new WaitForSeconds(0.2f);
        sprite.color = Color.white; // Restaurar color original
    }
}
