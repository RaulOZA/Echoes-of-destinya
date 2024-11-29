using UnityEngine;
using System.Collections;

public class PlayerFallDamage : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerHealth playerHealth;
    private bool isFalling = false;
    private float fallStartY;

    // Parameters for fall damage
    public float minimumFallDistance = 3f;    // Minimum fall distance to start taking damage
    public float damageMultiplier = 5f;       // Damage per unit of fall distance above the minimum
    public float maxFallDamage = 100f;        // Maximum damage player can receive from falling

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // Check if player starts falling
        if (!isFalling && rb.velocity.y < 0)
        {
            isFalling = true;
            fallStartY = transform.position.y;
        }

        // Reset fall if player is not moving vertically
        if (isFalling && rb.velocity.y >= 0)
        {
            isFalling = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ensure we only calculate fall damage when landing on the ground
        if (isFalling && collision.gameObject.CompareTag("Piso"))
        {
            isFalling = false;
            float fallDistance = fallStartY - transform.position.y;

            if (fallDistance > minimumFallDistance)
            {
                float damage = (fallDistance - minimumFallDistance) * damageMultiplier;
                damage = Mathf.Min(damage, maxFallDamage);

                if (damage > 0)
                {
                    playerHealth.health -= damage;

                    if (playerHealth.health <= 0)
                    {
                        playerHealth.health = 0;
                        playerHealth.Die();
                    }
                    else
                    {
                        StartCoroutine(playerHealth.Inmunity());
                    }
                }
            }
        }
    }
}
