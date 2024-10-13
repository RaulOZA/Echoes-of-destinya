using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoSalud : MonoBehaviour
{
    EnemigosGene enemy;
    private Animator animator;
    private bool isDead = false; // Variable para evitar múltiples muertes
    private bool canReceiveDamage = true; // Para la invulnerabilidad temporal
    [SerializeField] private float knockbackForce = 5f; // Fuerza del retroceso
    [SerializeField] private float invulnerabilityTime = 1f; // Tiempo de invulnerabilidad después de ser golpeado

    private void Start()
    {
        enemy = GetComponent<EnemigosGene>();
        animator = GetComponent<Animator>(); // Obtener el Animator del enemigo
    }

    // Método para manejar el golpe del enemigo
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon") && !isDead && canReceiveDamage) // Solo si el enemigo puede recibir daño y no está muerto
        {
            enemy.healthPoints -= 2f;

            // Aplica la animación de recibir golpe
            animator.SetTrigger("RecibirGolpe");
            AudioManager.instance.PlayAudio(AudioManager.instance.oscuroHit);
            // Aplica el retroceso según la posición del jugador
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            // Iniciar el tiempo de invulnerabilidad
            StartCoroutine(InvulnerabilityCoroutine());

            // Si la vida llega a 0, inicia la secuencia de muerte
            if (enemy.healthPoints <= 0)
            {
                StartCoroutine(DeathSequence());
            }
        }
    }

    // Coroutine para manejar la invulnerabilidad temporal
    private IEnumerator InvulnerabilityCoroutine()
    {
        canReceiveDamage = false; // Evitar recibir más daño durante el tiempo de invulnerabilidad
        yield return new WaitForSeconds(invulnerabilityTime); // Esperar el tiempo de invulnerabilidad
        canReceiveDamage = true; // Volver a permitir que el enemigo reciba daño
    }

    // Coroutine para manejar la muerte del enemigo
    private IEnumerator DeathSequence()
    {
        isDead = true; // Marca al enemigo como muerto
        animator.SetTrigger("MuerteEnemigo"); // Activa la animación de muerte
        AudioManager.instance.PlayAudio(AudioManager.instance.oscuroDeath);
        // Espera a que la animación de muerte termine
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject); // Destruye el enemigo después de la animación
    }
}
