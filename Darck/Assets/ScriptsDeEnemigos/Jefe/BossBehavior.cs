using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    public float meleeAttackRange = 2.0f; // Rango de ataque cuerpo a cuerpo.
    public float closeRange = 5.0f; // Rango donde el jefe camina hacia el jugador.
    public float farRange = 10.0f; // Rango donde el jefe corre hacia el jugador.
    public float walkSpeed = 3.0f; // Velocidad de caminar.
    public float runSpeed = 6.0f; // Velocidad de correr.
    public float magicAbilityCooldown = 5.0f; // Tiempo de espera entre habilidades mágicas.
    public float attackCooldown = 1.0f; // Tiempo entre ataques cuerpo a cuerpo.
    public int meleeDamage = 10; // Daño del ataque cuerpo a cuerpo.
    public GameObject magicPrefab; // Prefab de la habilidad mágica.
    public Transform magicSpawnPoint; // Punto de inicio de la habilidad mágica.
    public LayerMask playerLayer; // Capa del jugador para detectar.

    // Variables originales
    private Transform player; // Referencia al jugador.
    private float lastMeleeAttackTime;
    private float lastMagicAbilityTime;

    private Animator animator; // Referencia al Animator del jefe.
    private Rigidbody2D rb; // Referencia al Rigidbody2D.

    private int attackState = 0; // Estado del ataque: 0 -> Ataque 1, 1 -> Ataque 2, 2 -> Combo.

    public float bossHealth, currentHealth;
    public Image HealthImg;

    // Variables para la lluvia de poderes mágicos
    public GameObject fallingMagicPrefab; // Prefab del ataque mágico.
    public float fallingMagicCooldown = 8.0f; // Tiempo entre lluvias de poderes mágicos.
    public float magicSpawnHeight = 15.0f; // Altura desde donde aparecen los poderes mágicos.
    public int numberOfPowers = 5; // Cantidad de poderes que caen durante la lluvia.
    public float spawnAreaRadius = 8.0f; // Radio alrededor del jugador donde caerán los poderes mágicos.
    private float lastFallingMagicTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = bossHealth;
        HealthImg.fillAmount = 1.0f; // Barra de vida llena al inicio.
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            RotateTowardsPlayer();

            if (distanceToPlayer > farRange)
            {
                RunTowardsPlayer();
            }
            else if (distanceToPlayer > meleeAttackRange && distanceToPlayer <= farRange)
            {
                WalkTowardsPlayer();
            }
            else if (distanceToPlayer <= meleeAttackRange && Time.time >= lastMeleeAttackTime + attackCooldown)
            {
                PerformMeleeAttack();
            }
            else
            {
                StopMovement();
            }

            // Ataque mágico normal
            if (Time.time >= lastMagicAbilityTime + magicAbilityCooldown)
            {
                CastMagicAbility();
            }

            // Lluvia de poderes mágicos
            if (Time.time >= lastFallingMagicTime + fallingMagicCooldown)
            {
                CastFallingMagic();
            }
        }
    }

    void RotateTowardsPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(8.29f, 8.29f, 1.03625f);
        }
        else
        {
            transform.localScale = new Vector3(-8.29f, 8.29f, 1.03625f);
        }
    }

    void WalkTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * walkSpeed, rb.velocity.y);
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsRunning", false);
    }

    void RunTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * runSpeed, rb.velocity.y);
        animator.SetBool("IsRunning", true);
        animator.SetBool("IsWalking", false);
    }

    void StopMovement()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
    }

    void PerformMeleeAttack()
    {
        lastMeleeAttackTime = Time.time;
        rb.velocity = Vector2.zero;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);

        switch (attackState)
        {
            case 0:
                StartCoroutine(HandleMeleeAttack("Attack1"));
                attackState = 1;
                break;

            case 1:
                StartCoroutine(HandleMeleeAttack("Attack2"));
                attackState = 2;
                break;

            case 2:
                StartCoroutine(PerformComboAttack());
                attackState = 0;
                break;
        }
    }

    IEnumerator HandleMeleeAttack(string animationTrigger)
    {
        animator.SetTrigger(animationTrigger);

        // Esperar el tiempo que dura la animación antes de aplicar daño.
        yield return new WaitForSeconds(0.5f); // Ajusta el tiempo al de la animación.

        // Aplica el daño al jugador.
        ApplyDamage();
    }

    IEnumerator PerformComboAttack()
    {
        Debug.Log("Jefe realiza combo de ataque");

        // Primer ataque del combo.
        animator.SetTrigger("Attack1");
        ApplyDamage(); // Aplica daño del primer ataque inmediatamente.
        yield return new WaitForSeconds(0.3f); // Tiempo reducido antes del siguiente ataque.

        // Segundo ataque del combo.
        animator.SetTrigger("Attack2");
        ApplyDamage(); // Aplica daño del segundo ataque inmediatamente.
        yield return new WaitForSeconds(0.3f); // Tiempo reducido para finalizar el combo.
    }

    void ApplyDamage()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, meleeAttackRange, playerLayer);

        foreach (Collider2D playerCollider in hitPlayers)
        {
            PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Reducir la salud directamente
                playerHealth.health -= 10; // Cambia el valor del daño según lo necesites

                // Verificar si la salud del jugador es menor o igual a 0
                if (playerHealth.health <= 0)
                {
                    playerHealth.health = 0; // Evitar que sea negativo
                    playerHealth.Die();      // Llamar al método Die en PlayerHealth
                }

                // Actualizar la barra de salud del jugador
                playerHealth.healthBar.value = playerHealth.health;
            }
        }
    }

    void CastMagicAbility()
    {
        lastMagicAbilityTime = Time.time;

        if (magicPrefab != null && magicSpawnPoint != null)
        {
            Instantiate(magicPrefab, magicSpawnPoint.position, magicSpawnPoint.rotation);
        }
    }

    void CastFallingMagic()
    {
        lastFallingMagicTime = Time.time;

        if (fallingMagicPrefab != null)
        {
            for (int i = 0; i < numberOfPowers; i++)
            {
                // Generar una posición aleatoria cerca del jugador.
                Vector3 randomPosition = new Vector3(
                    player.position.x + Random.Range(-spawnAreaRadius, spawnAreaRadius),
                    player.position.y + magicSpawnHeight,
                    player.position.z
                );

                // Instanciar el poder mágico.
                GameObject fallingMagic = Instantiate(fallingMagicPrefab, randomPosition, Quaternion.identity);

                // Asegurar que la animación de creación se active.
                Animator magicAnimator = fallingMagic.GetComponent<Animator>();
                if (magicAnimator != null)
                {
                    magicAnimator.Play("jefePoder");
                }
            }

        }
    }

    public void DamageBoss(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        HealthImg.fillAmount = currentHealth / bossHealth;

        PlayHitAnimation();

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void OnDestroy()
    {
        BossUI.instance.BossDeactivator();
    }

    public void PlayHitAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsHit", true);
            StartCoroutine(ResetHitAnimation());
        }
    }

    private IEnumerator ResetHitAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsHit", false);
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        rb.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 10.0f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, farRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, closeRange);
    }
}
