using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthBar;  // Slider para la barra de vida
    public float health;
    public float maxHealth;
    bool isInmune;
    public float inmunityTime;
    Blink material;
    SpriteRenderer sprite;

    // Valores para el retroceso
    public float knockbackForce = 4f;   // Fuerza del retroceso
    public float knockbackDuration = 0.3f; // Duración del retroceso
    private Rigidbody2D rb; // Para controlar el movimiento del jugador
    public bool isDead;
    public GameObject gameOverImg;

    void Start()
    {
        gameOverImg.SetActive(false);
        sprite = GetComponent<SpriteRenderer>();
        material = GetComponent<Blink>();
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>(); // Obtener el componente Rigidbody2D

        // Inicializa la barra de vida
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        if (!isDead)
        {
            gameOverImg.GetComponent<CanvasGroup>().alpha = 0.0f;
        }
    }

    void Update()
    {
        IsDead();
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        // Actualiza la barra de vida según el valor actual de salud
        healthBar.value = health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo") && !isInmune)
        {
            health -= collision.GetComponent<EnemigosGene>().damageToGive;
            // Determinar la dirección del retroceso basado en la posición del enemigo
            Vector2 knockbackDirection = transform.position - collision.transform.position;
            knockbackDirection.Normalize(); // Asegurarse de que la dirección esté normalizada
            StartCoroutine(ApplyKnockback(knockbackDirection));

            StartCoroutine(Inmunity());

            if (health <= 0)
            {
                // Lógica para muerte o pantalla de game over
                Time.timeScale = 0;
                gameOverImg.SetActive(true);
                //Detener otros sonidos
                AudioManager.instance.PlayAudio(AudioManager.instance.gameOver);
                isDead = true;
                print("Has muerto");
            }

            // Actualiza la barra de vida después de recibir daño
            healthBar.value = health;
        }
    }

    public void IsDead()
    {
        if (isDead)
        {
            Time.timeScale = 0;
            gameOverImg.SetActive(true);

            if (gameOverImg.GetComponent<CanvasGroup>().alpha < 1f)
            {
                gameOverImg.GetComponent<CanvasGroup>().alpha += 0.005f;
            }
        }
    }

    IEnumerator ApplyKnockback(Vector2 direction)
    {
        float timer = 0;
        while (timer < knockbackDuration)
        {
            timer += Time.deltaTime;
            rb.velocity = new Vector2(direction.x * knockbackForce, rb.velocity.y); // Aplicar retroceso solo en el eje X
            yield return null;
        }
        rb.velocity = Vector2.zero; // Detener el retroceso después de la duración
    }

    IEnumerator Inmunity()
    {
        isInmune = true;
        sprite.material = material.blink;
        yield return new WaitForSeconds(inmunityTime);
        sprite.material = material.original;
        isInmune = false;
    }
}