using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovilidadEnemigo : MonoBehaviour
{
    public float speed = 2f;
    public Transform[] waypoints;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    private int currentWaypointIndex = 0;
    private Transform player;
    private bool isChasing = false;

    private Animator animator; // Referencia al Animator

    private bool playerInRange = false; // Saber si el jugador est� en el rango de detecci�n

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); // Obtener el Animator del enemigo
    }

    void Update()
    {
        if (playerInRange)
        {
            // Mover hacia el jugador
            MoveTowards(player.position);

            // Cambiar a animaci�n de caminar
            animator.SetBool("Caminar", true);

            // Comprobar si est� en rango de ataque
            if (Vector2.Distance(transform.position, player.position) <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                RotateTowardsPlayer(); // Rotar el enemigo hacia el jugador
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // Patrullar
            Patrol();

            // Cambiar a animaci�n de caminar si se est� moviendo
            animator.SetBool("Caminar", true);
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        MoveTowards(targetWaypoint.position);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

            // Si no se est� moviendo, cambiar a animaci�n Idle
            animator.SetBool("Caminar", false);
        }
    }

    void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position; // Direcci�n hacia el jugador
        if (direction.x < 0)
        {
            // Girar el enemigo si el jugador est� a la izquierda
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // Girar el enemigo si el jugador est� a la derecha
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void Attack()
    {
        // Activar la animaci�n de ataque
        animator.SetTrigger("Ataque");

        // Implementar la l�gica para el ataque
        Debug.Log("Attacking Player!");
    }

    // Detectar si el jugador entra en el rango de detecci�n
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Detectar si el jugador sale del rango de detecci�n
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            animator.SetBool("Caminar", false); // Cambiar a Idle si el jugador se va
        }
    }
}