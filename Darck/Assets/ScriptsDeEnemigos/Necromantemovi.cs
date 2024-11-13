using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromantemovi : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform firePoint; // Punto desde donde se dispara el proyectil
    public float attackRange = 10f; // Rango de ataque
    public float attackCooldown = 2f; // Tiempo entre ataques
    private float attackTimer = 0f;
    public LayerMask playerLayer; // Capa del jugador

    private Transform player;

    // Referencia al Animator del necromante
    public Animator animator;

    private void Update()
    {
        // Buscar al jugador
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (playerCollider != null)
        {
            player = playerCollider.transform;

            // Atacar si el temporizador lo permite
            if (attackTimer <= 0f)
            {
                StartCoroutine(AttackPlayer());
                attackTimer = attackCooldown; // Reiniciar el temporizador
            }
        }
        else
        {
            player = null;
        }

        // Reducir el temporizador de ataque
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private IEnumerator AttackPlayer()
    {
        // Activar la animaci�n de ataque
        if (animator != null)
        {
            animator.SetBool("Atacar", true);
        }

        // Esperar la duraci�n de la animaci�n antes de lanzar el proyectil (ajusta el tiempo seg�n tu animaci�n)
        yield return new WaitForSeconds(0.8f);

        // Instanciar el proyectil y establecer su direcci�n
        if (player != null) // Verificar si el jugador sigue en rango
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // Calcular la posici�n objetivo
            Vector3 targetPosition = player.position + Vector3.up * 2f; // Ajusta la altura seg�n lo necesites

            // Configurar el proyectil hacia el objetivo
            projectile.GetComponent<Projectile>().SetTarget(targetPosition);
        }

        // Desactivar la animaci�n de ataque
        if (animator != null)
        {
            animator.SetBool("Atacar", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja el rango de ataque en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
