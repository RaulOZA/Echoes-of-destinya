using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f; // Velocidad del proyectil
    public float detectionRange = 10f; // Rango máximo del proyectil
    private Vector3 targetPosition;

    // Método para configurar el objetivo del proyectil
    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    private void Update()
    {
        // Mover el proyectil hacia el objetivo si está en el rango
        if (Vector3.Distance(transform.position, targetPosition) <= detectionRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Destruir el proyectil si llega al objetivo
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Si el objetivo está fuera de rango, destruye el proyectil
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el proyectil choca con algo, se destruye automáticamente
        if (collision.CompareTag("Player") || collision.CompareTag("Piso"))
        {
            Destroy(gameObject);
        }
    }
}
