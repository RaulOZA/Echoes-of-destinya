using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigopro : MonoBehaviour
{
    public GameObject projectile; // Prefab del proyectil
    public Transform player; // Referencia al jugador
    public float timeToShoot = 2.0f;
    private float shootCooldown;

    public float detectionRange = 10f; // Distancia m�xima a la cual detecta al jugador
    public float projectileLifetime = 5f; // Tiempo de vida del proyectil en segundos

    void Start()
    {
        shootCooldown = timeToShoot;
    }

    void Update()
    {
        // Calcula la distancia entre el enemigo y el jugador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Si el jugador est� dentro del rango de detecci�n
        if (distanceToPlayer <= detectionRange)
        {
            shootCooldown -= Time.deltaTime;

            if (shootCooldown <= 0)
            {
                ShootProjectile();
                shootCooldown = timeToShoot; // Reinicia el cooldown
            }
        }
    }

    void ShootProjectile()
    {
        // Instancia el proyectil en la posici�n del enemigo
        GameObject ProjectSke = Instantiate(projectile, transform.position, Quaternion.identity);

        // Calcula la direcci�n hacia el jugador
        Vector2 direction = (player.position - transform.position).normalized;

        // Aplica la fuerza en la direcci�n del jugador
        ProjectSke.GetComponent<Rigidbody2D>().AddForce(direction * 300f, ForceMode2D.Force);

        // A�ade el script de colisi�n al proyectil y establece el tiempo de vida
        ProjectSke.AddComponent<ProjectileCollision>().lifetime = projectileLifetime;
    }
}

// Script para manejar la colisi�n del proyectil
public class ProjectileCollision : MonoBehaviour
{
    public float lifetime; // Tiempo de vida del proyectil

    private void Start()
    {
        // Destruye el proyectil despu�s de un tiempo si no impacta con el jugador
        Destroy(gameObject, lifetime);
    }

   
}
