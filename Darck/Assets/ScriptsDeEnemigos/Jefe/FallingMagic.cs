using UnityEngine;

public class FallingMagic : MonoBehaviour
{
    public float fallSpeed = 10.0f; // Velocidad de caída.
    public int damage = 10;        // Daño que causa al jugador.
    public float lifetime = 5.0f;  // Tiempo en segundos antes de autodestruirse.

    private Animator animator;     // Referencia al Animator.
    private bool hasImpacted = false; // Indica si el poder ya ha impactado.

    void Start()
    {
        animator = GetComponent<Animator>();

        // Inicia la autodestrucción después del tiempo definido.
        Invoke("DestroyMagic", lifetime);
    }

    void Update()
    {
        // Movimiento hacia abajo solo si no ha impactado.
        if (!hasImpacted)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Impacto con el jugador.
        if (collision.collider.CompareTag("Player") && !hasImpacted)
        {
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.isInmune) // Daño solo si no está inmune.
            {
                // Reducir salud del jugador.
                playerHealth.health -= damage;

                if (playerHealth.health <= 0)
                {
                    playerHealth.health = 0;
                    playerHealth.Die();
                }
                else
                {
                    // Calcular dirección de retroceso horizontal.
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    knockbackDirection.y = 0; // No afectar la dirección vertical.

                    // Aplicar retroceso e inmunidad.
                    playerHealth.StartCoroutine(playerHealth.ApplyKnockback(knockbackDirection));
                    playerHealth.StartCoroutine(playerHealth.Inmunity());
                }
            }

            TriggerImpact();
        }
        // Impacto con el suelo.
        else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Piso") && !hasImpacted)
        {
            TriggerImpact();
        }
    }

    private void TriggerImpact()
    {
        if (hasImpacted) return; // Evitar múltiples impactos.

        hasImpacted = true; // Marcar que ya ha impactado.

        // Si existe un Animator, activa la animación de impacto antes de destruir.
        if (animator != null)
        {
            animator.SetTrigger("Impact");
        }

        // Destruir el objeto después de un pequeño retraso para que se complete la animación.
        Destroy(gameObject, 0.5f);
    }

    private void DestroyMagic()
    {
        if (!hasImpacted)
        {
            Destroy(gameObject);
        }
    }
}
