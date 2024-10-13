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

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        material = GetComponent<Blink>();
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>(); // Obtener el componente Rigidbody2D

        // Inicializa la barra de vida
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        // Actualiza la barra de vida según el valor actual de salud
        healthBar.value = health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el jugador ha colisionado con un arma y no está en estado inmune.
        if (collision.CompareTag("Weapon") && !isInmune)
        {
            // Obtener el script MovilidadEnemigo desde el objeto que tiene el collider.
            MovilidadEnemigo enemigo = collision.GetComponentInParent<MovilidadEnemigo>();

            if (enemigo != null)
            {
                // Aplicar daño al jugador según el daño del enemigo.
                float damage = enemigo.damage;
                health -= damage;

                // Determinar la dirección del retroceso basado en la posición del arma.
                Vector2 knockbackDirection = transform.position - collision.transform.position;
                knockbackDirection.Normalize(); // Asegurarse de que la dirección esté normalizada
                StartCoroutine(ApplyKnockback(knockbackDirection));

                // Activar inmunidad temporal.
                StartCoroutine(Inmunity());

                // Verificar si la salud del jugador ha llegado a 0.
                if (health <= 0)
                {
                    print("Has muerto");
                }

                // Actualizar la barra de vida después de recibir daño.
                healthBar.value = health;
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
