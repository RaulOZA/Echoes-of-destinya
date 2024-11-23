using System.Collections;
using UnityEngine;

public class MovilidadEnemigo : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform player;
    [SerializeField] private float followDistance = 10f;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] public float damage = 10f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D weaponCollider; // Collider del arma
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 1f;

    private int currentWaypoint;
    private bool isWaiting;
    private bool isFollowingPlayer;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    void Start()
    {
        currentWaypoint = 0;
        isWaiting = false;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (IsGrounded())
        {
            if (!isAttacking)
            {
                if (distanceToPlayer <= followDistance && distanceToPlayer > attackDistance)
                {
                    isFollowingPlayer = true;
                    PatrolWaypoints(false);
                    FollowPlayer();
                }
                else if (distanceToPlayer <= attackDistance && Time.time >= nextAttackTime)
                {
                    isFollowingPlayer = false;
                    PatrolWaypoints(false);
                    StartCoroutine(AttackPlayer());
                }
                else if (distanceToPlayer > followDistance)
                {
                    isFollowingPlayer = false;
                    PatrolWaypoints(true);
                }
            }
        }
        else
        {
            animator.SetBool("Caminar", false);
        }
    }

    void PatrolWaypoints(bool shouldPatrol)
    {
        if (shouldPatrol)
        {
            if (!isWaiting)
            {
                Vector2 targetPosition = new Vector2(waypoints[currentWaypoint].position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                animator.SetBool("Caminar", true);
                FlipTowards(targetPosition.x);

                if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                {
                    StartCoroutine(WaitAtWaypoint());
                }
            }
        }
        else
        {
            animator.SetBool("Caminar", false);
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetBool("Caminar", false);
        yield return new WaitForSeconds(waitTime);
        currentWaypoint++;
        if (currentWaypoint >= waypoints.Length)
        {
            currentWaypoint = 0;
        }
        isWaiting = false;
    }

    void FollowPlayer()
    {
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        animator.SetBool("Caminar", true);
        FlipTowards(targetPosition.x);
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetBool("Caminar", false);
        animator.SetBool("Atacar", true);

        // Activar el collider de arma en el momento adecuado
        yield return new WaitForSeconds(0.5f); // Sincronizar con la animaci�n de ataque
        ActivateWeaponCollider(); // Activa el collider del arma

        // Aplicar daño directamente al jugador
        if (Vector2.Distance(transform.position, player.position) <= attackDistance)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.health -= damage;
                playerHealth.health = Mathf.Clamp(playerHealth.health, 0, playerHealth.maxHealth);
                playerHealth.healthBar.value = playerHealth.health;

                if (playerHealth.health <= 0)
                {
                    playerHealth.Die();
                }
                else
                {
                    StartCoroutine(playerHealth.Inmunity());
                }
            }
        }

        yield return new WaitForSeconds(0.2f); // Duración del daño
        DeactivateWeaponCollider(); // Desactiva el collider del arma al finalizar el ataque

        nextAttackTime = Time.time + attackCooldown;
        animator.SetBool("Atacar", false);
        isAttacking = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (weaponCollider.enabled && collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.health -= damage; // Aplica el daño directamente
                playerHealth.health = Mathf.Clamp(playerHealth.health, 0, playerHealth.maxHealth); // Asegura que la salud no sea negativa
                playerHealth.healthBar.value = playerHealth.health; // Actualiza la barra de vida

                if (playerHealth.health <= 0)
                {
                    playerHealth.Die();
                }
                else
                {
                    StartCoroutine(playerHealth.Inmunity());
                }

                DeactivateWeaponCollider(); // Evita múltiples daños por la misma activación
            }
        }
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

    public void ActivateWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    public void DeactivateWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
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
}


