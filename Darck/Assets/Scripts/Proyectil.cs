using System.Collections;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    [SerializeField] private int daño = 10;
    [SerializeField] private float tiempoDeVida = 2f; // Tiempo máximo antes de destruir el proyectil
    private Animator animator;
    private bool impactado = false; // Evita múltiples impactos

    [SerializeField] public bool esPoderFuego = false; // Indica si es el poder de fuego

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("El Animator no está asignado al proyectil.");
        }

        // Configurar rotación del proyectil
        if (esPoderFuego)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Horizontal
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 89); // Vertical
        }

        // Destruir el proyectil automáticamente después de cierto tiempo
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10) // Colisiona con enemigos (Layer 10)
        {
            if (!impactado)
            {
                impactado = true;

                // Obtener el script del enemigo
                EnemigoSalud enemigoSalud = collision.GetComponent<EnemigoSalud>();
                if (enemigoSalud != null)
                {
                    Debug.Log($"Proyectil impactó a: {collision.name}");

                    // Aplica daño al enemigo
                    enemigoSalud.RecibirDaño(daño, esPoderFuego); // Unifica el manejo de daño
                }
            }

            // Destruir el proyectil tras el impacto
            if (!esPoderFuego)
            {
                if (animator != null)
                {
                    animator.SetTrigger("Impacto");
                }
                StartCoroutine(DestruirProyectilDespuesDeImpacto());
            }
        }
    }

    private IEnumerator DestruirProyectilDespuesDeImpacto()
    {
        if (animator == null || esPoderFuego)
        {
            Destroy(gameObject);
            yield break;
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}
