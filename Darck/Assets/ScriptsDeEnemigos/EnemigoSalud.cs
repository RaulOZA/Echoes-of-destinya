using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemigoSalud : MonoBehaviour
{
    public enum EnemyType { Regular, Skeleton, Necromancer, Boss } // Tipos de enemigos
    [SerializeField] private EnemyType enemyType;

    public EnemigosGene enemy;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isDead = false;
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
            else if (enemyType == EnemyType.Regular) {
                AudioManager.instance.PlayAudio(AudioManager.instance.oscuroHit);
                enemy.healthPoints = Mathf.Max(enemy.healthPoints - damage, 0);
                animator.SetTrigger("RecibirGolpe");
                StartCoroutine(BlinkEffect());
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

    private void TakeBossDamage(float damage)
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

    private IEnumerator DeathSequence()
    {
        isDead = true;
        animator.SetTrigger("MuerteEnemigo");

        if (enemyType == EnemyType.Boss)
        {
                // Actualiza el puntaje en la base de datos
        AdminMySQL _adminMYSQL = GameObject.Find("Admin_BD").GetComponent<AdminMySQL>();
        int userId = SessionManager.Instance.CurrentUserId; // Obtén el ID del usuario actual
        _adminMYSQL.UpdateScore(userId, 80); // Asegúrate de tener un método UpdateScore en AdminMySQL
            Debug.Log("El jefe ha muerto");
        }
        else
        {
        // Actualiza el puntaje en la base de datos
            AdminMySQL _adminMYSQL = GameObject.Find("Admin_BD").GetComponent<AdminMySQL>();
            int userId = SessionManager.Instance.CurrentUserId; // Obtén el ID del usuario actual
            _adminMYSQL.UpdateScore(userId, 50); // Asegúrate de tener un método UpdateScore en AdminMySQL
            Debug.Log("Enemigo regular muerto");
        }

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (enemyType == EnemyType.Boss)
        {
                    // Actualiza el puntaje en la base de datos
            AdminMySQL _adminMYSQL = GameObject.Find("Admin_BD").GetComponent<AdminMySQL>();
            int userId = SessionManager.Instance.CurrentUserId; // Obtén el ID del usuario actual
            _adminMYSQL.UpdateScore(userId, 200); // Asegúrate de tener un método UpdateScore en AdminMySQL
            Destroy(gameObject); // Elimina al jefe
        }
        else
        {
            Destroy(gameObject); // Elimina al enemigo regular
        }
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
