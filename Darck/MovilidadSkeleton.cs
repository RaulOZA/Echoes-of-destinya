using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovilidadSkeleton : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform[] waypoints; // Waypoints para patrullaje
    [SerializeField] private float waitTimeAtWaypoint = 2f; // Tiempo de espera en cada waypoint
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private Animator animator;

    private bool isFacingRight = true;
    private bool isGrounded;
    private bool isDead = false; // Control de muerte
    private bool isWaiting = false; // Control de espera en waypoints
    private int currentWaypoint = 0; // Waypoint actual

    void Update()
    {
        if (!isDead && !isWaiting) // Solo se mueve si no est� muerto y no est� esperando
        {
            isGrounded = IsGrounded();
            bool isNearWall = IsNearWall();

            if (isGrounded && !isNearWall)
            {
                Patrol();
            }
            else
            {
                animator.SetBool("Caminar", false); // Asegurarse de detener la animaci�n al girar
                Flip();
            }
        }
        else
        {
            animator.SetBool("Caminar", false); // Detener la animaci�n al morir o esperar
        }
    }

    void Patrol()
    {
        Transform targetWaypoint = waypoints[currentWaypoint]; // Waypoint objetivo
        Vector2 targetPosition = new Vector2(targetWaypoint.position.x, transform.position.y);

        // Solo activa la animaci�n de caminar si el enemigo realmente se est� moviendo
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            animator.SetBool("Caminar", true); // Activar animaci�n de caminar
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            FlipTowards(targetPosition.x); // Orientar hacia el waypoint
        }
        else
        {
            StartCoroutine(WaitAtWaypoint()); // Iniciar la espera si ya lleg� al waypoint
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true; // Activar estado de espera
        animator.SetBool("Caminar", false); // Detener animaci�n al esperar

        yield return new WaitForSeconds(waitTimeAtWaypoint); // Esperar el tiempo definido

        // Cambiar al siguiente waypoint
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length; // Ciclar entre waypoints

        isWaiting = false; // Terminar estado de espera
    }

    void FlipTowards(float targetX)
    {
        float direction = targetX - transform.position.x;
        if (Mathf.Abs(direction) > 0.1f)
        {
            if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    bool IsNearWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, Vector2.right * (isFacingRight ? 1 : -1), wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    public void SetDead(bool deadStatus)
    {
        isDead = deadStatus;
        if (isDead)
        {
            animator.SetBool("Caminar", false); // Detener la animaci�n si est� muerto
        }
    }
}
