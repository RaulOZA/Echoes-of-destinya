using UnityEngine;

public class FallingMagic : MonoBehaviour
{
    public float fallSpeed = 10.0f; // Velocidad de ca�da.
    public int damage = 10;        // Da�o que causa al jugador.
    public float lifetime = 5.0f;  // Tiempo en segundos antes de autodestruirse.

    private Animator animator;     // Referencia al Animator.
    private bool hasImpacted = false; // Indica si el poder ya ha impactado.

    void Start()
    {
        animator = GetComponent<Animator>();

        // Inicia la autodestrucci�n despu�s del tiempo definido.
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
            if (playerHealth != null && !playerHealth.isInmune) // Da�o solo si no est� inmune.
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
                    // Calcular direcci�n de retroceso horizontal.
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    knockbackDirection.y = 0; // No afectar la direcci�n vertical.

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
        if (hasImpacted) return; // Evitar m�ltiples impactos.

        hasImpacted = true; // Marcar que ya ha impactado.

        // Si existe un Animator, activa la animaci�n de impacto antes de destruir.
        if (animator != null)
        {
            animator.SetTrigger("Impact");
        }

        // Destruir el objeto despu�s de un peque�o retraso para que se complete la animaci�n.
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
