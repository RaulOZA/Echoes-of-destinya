using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemigoSalud : MonoBehaviour
{
    public enum EnemyType { Regular, Skeleton, Necromancer, Boss } // Tipos de enemigos
    [SerializeField] public EnemyType enemyType;

    public EnemigosGene enemy;
    public Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    public bool isDead = false;
    private bool canReceiveDamage = true;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float invulnerabilityTime = 1f;

    [Header("Boss Settings")]
    public float bossHealth = 100f; // Salud máxima del jefe
    private float currentHealth;
    public Image healthBarImage; // Barra de salud del jefe
    [SerializeField] private float attackCooldown = 1f; // Tiempo entre ataques del jefe
    [SerializeField] private float meleeAttackRange = 2.0f; // Rango de ataque cuerpo a cuerpo del jefe
    [SerializeField] private Transform player; // Referencia al jugador
    private float lastAttackTime;

    [Header("Puntos")]
    [SerializeField] private int scorePoints = 10; // Puntos otorgados al morir este enemigo

    private void Start()
    {
        enemy = GetComponent<EnemigosGene>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Inicializar comportamiento del jefe si aplica
        if (enemyType == EnemyType.Boss)
        {
            currentHealth = bossHealth;
            if (healthBarImage != null) healthBarImage.fillAmount = 1.0f;
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (enemyType == EnemyType.Boss && player != null && !isDead)
        {
            BossBehavior();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon") && !isDead && canReceiveDamage)
        {
            float damage = 2f; // Daño base del arma

            if (enemyType == EnemyType.Boss)
            {
                // Comportamiento para el jefe
                TakeBossDamage(damage);
            }
            else
            {
                // Comportamiento para enemigos regulares
                enemy.healthPoints = Mathf.Max(enemy.healthPoints - damage, 0);
                animator.SetTrigger("RecibirGolpe");
                StartCoroutine(BlinkEffect());
            }

            // Aplica retroceso
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            StartCoroutine(InvulnerabilityCoroutine());

            // Si la salud llega a 0
            if (enemy.healthPoints <= 0 && !isDead)
            {
                StartCoroutine(DeathSequence());
            }
        }
    }

    public void TakeBossDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0) currentHealth = 0;
        if (healthBarImage != null) healthBarImage.fillAmount = currentHealth / bossHealth;

        // Activar la animación de daño para el jefe
        animator.SetBool("IsHit", true);
        StartCoroutine(ResetBossHitAnimation());

        if (currentHealth == 0)
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator ResetBossHitAnimation()
    {
        yield return new WaitForSeconds(0.5f); // Duración de la animación de daño
        animator.SetBool("IsHit", false);     // Reinicia la animación
    }

    void BossBehavior()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Ajustar dirección para mirar al jugador manteniendo la escala original
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // Si está en rango de ataque y puede atacar
        if (distanceToPlayer <= meleeAttackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            PerformBossAttack();
        }
    }


    private void PerformBossAttack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");

        // Detectar jugadores en el rango de ataque
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, meleeAttackRange, LayerMask.GetMask("Player"));

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

    private IEnumerator BlinkEffect()
    {
        float blinkDuration = 0.1f;
        int blinkCount = 5;

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkDuration);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        canReceiveDamage = false;
        yield return new WaitForSeconds(invulnerabilityTime);
        canReceiveDamage = true;
    }

    public void RecibirDaño(int daño, bool esPoderFuego)
    {
        if (isDead) return;

        // Resta vida al enemigo
        enemy.healthPoints -= daño;
        enemy.healthPoints = Mathf.Max(enemy.healthPoints, 0);

        Debug.Log($"{gameObject.name} recibió daño. Vida restante: {enemy.healthPoints}");

        // Si la vida llega a 0, inicia la secuencia de muerte
        if (enemy.healthPoints <= 0)
        {
            StartCoroutine(DeathSequence());
        }
    }

    public IEnumerator DeathSequence()
    {
        if (isDead) yield break; // Prevenir múltiples ejecuciones
        isDead = true;

        Debug.Log($"{gameObject.name} inició la secuencia de muerte.");

        // Detener movimiento
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            Debug.Log("Movimiento detenido.");
        }

        // Desactivar colisionadores
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
            Debug.Log($"Colisionador {collider.name} desactivado.");
        }

        // Desactivar scripts
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this) script.enabled = false;
            Debug.Log($"Script {script.GetType().Name} desactivado.");
        }

        // Reproducir animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("MuerteEnemigo");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        // Agregar puntos al GameManager
        GameManager.instance.AddScore(scorePoints);


        // Verifica el tipo de enemigo y actualiza el puntaje
        GameObject adminBDObject = GameObject.Find("Admin_BD");
        AdminMySQL _adminMYSQL = adminBDObject.GetComponent<AdminMySQL>();
        int userId = SessionManager.Instance?.CurrentUserId ?? 0; // Maneja el caso en que la sesión sea null
        _adminMYSQL.UpdateScore(userId, 10);

        // Eliminar hijos antes de destruir el enemigo
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Hijos eliminados.");

        // Destruir el enemigo
        Destroy(gameObject);
        Debug.Log($"{gameObject.name} fue destruido.");
    }


    private void OnDrawGizmosSelected()
    {
        if (enemyType == EnemyType.Boss)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
        }
    }
}