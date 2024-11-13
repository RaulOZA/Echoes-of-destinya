using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoSalud : MonoBehaviour
{
    public enum EnemyType { Oscuro, Skeleton, Necromancer } // Tipos de enemigos
    [SerializeField] private EnemyType enemyType;

    private EnemigosGene enemy;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    private bool canReceiveDamage = true;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float respawnTime = 5f;
    private float maxHealth;

    private MovilidadSkeleton skeletonMovement; // Referencia al script de movimiento
    [SerializeField] private List<EnemigoSalud> controlledSkeletons = new List<EnemigoSalud>(); // Skeletons controlados por el Necromancer

    [SerializeField] private int scorePoints; // Puntos que otorgará este enemigo al morir

    [Header("Drop Settings")] // Configuración de drops
    [SerializeField] private GameObject healthItemPrefab; // Prefab del ítem de salud
    [SerializeField] private GameObject manaItemPrefab; // Prefab del ítem de maná
    [SerializeField] private float dropChance = 0.5f; // Probabilidad general de que el enemigo suelte algo

    private void Start()
    {
        enemy = GetComponent<EnemigosGene>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        skeletonMovement = GetComponent<MovilidadSkeleton>(); // Obtener la referencia al script de movimiento
        maxHealth = enemy.healthPoints;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon") && !isDead && canReceiveDamage)
        {
            enemy.healthPoints = Mathf.Max(enemy.healthPoints - 2f, 0); // Asegura que la salud no sea menor a 0

            if (enemyType != EnemyType.Skeleton)
            {
                animator.SetTrigger("RecibirGolpe");
            }
            else
            {
                StartCoroutine(BlinkEffect());
            }

            AudioManager.instance.PlayAudio(AudioManager.instance.oscuroHit);

            // Aplica el retroceso según la posición del jugador
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            StartCoroutine(InvulnerabilityCoroutine());

            if (enemy.healthPoints <= 0 && !isDead)
            {
                StartCoroutine(DeathSequence());
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
        skeletonMovement?.SetDead(true); // Detener el movimiento al morir si existe skeletonMovement
        animator.SetTrigger("MuerteEnemigo"); // Activa la animación de muerte
        AudioManager.instance.PlayAudio(AudioManager.instance.oscuroDeath);

        // **Agregamos los puntos al puntaje global usando GameManager**
        GameManager.instance.AddScore(scorePoints);

        // Generar ítems al morir
        DropItem();

        // Espera la duración de la animación de muerte antes de proceder
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (enemyType == EnemyType.Skeleton)
        {
            yield return new WaitForSeconds(respawnTime);
            ReviveEnemy();
        }
        else if (enemyType == EnemyType.Necromancer)
        {
            EliminateControlledSkeletons();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ReviveEnemy()
    {
        isDead = false;
        skeletonMovement?.SetDead(false); // Reactivar el movimiento al revivir si existe skeletonMovement
        enemy.healthPoints = maxHealth;
        animator.SetTrigger("Revive");
    }

    private void EliminateControlledSkeletons()
    {
        foreach (EnemigoSalud Skeleton in controlledSkeletons)
        {
            if (Skeleton != null)
            {
                Destroy(Skeleton.gameObject); // Destruir cada Skeleton controlado
            }
        }
    }

    private void DropItem()
    {
        // Determinar si el enemigo soltará algo basado en la probabilidad general
        if (Random.value < dropChance)
        {
            // Decidir aleatoriamente qué ítem soltar
            float itemRoll = Random.value;
            if (itemRoll < 0.5f) // 50% de posibilidades de soltar un ítem de salud
            {
                Instantiate(healthItemPrefab, transform.position, Quaternion.identity);
            }
            else // 50% de posibilidades de soltar un ítem de maná
            {
                Instantiate(manaItemPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
