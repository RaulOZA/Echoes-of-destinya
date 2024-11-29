using UnityEngine;

public class RespawnController2D : MonoBehaviour
{
    public Transform respawnPoint; // Assign your RespawnPoint here
    public GameObject player;      // Assign your Player object here

    public float respawnDamage = 12f; // Fixed damage applied on respawn

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Apply fixed damage to the player
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.health -= respawnDamage;

                    // Ensure the player's health doesn't drop below 0
                    if (playerHealth.health <= 0)
                    {
                        playerHealth.health = 0;
                        playerHealth.Die();
                    }
                    else
                    {
                        // Optionally, provide feedback for taking damage
                        StartCoroutine(playerHealth.Inmunity());
                    }
                }

                // Reset the player's position to the respawn point
                player.transform.position = respawnPoint.position;

                // Reset the player's velocity
                rb.velocity = Vector2.zero; // Stop any movement
            }
        }
    }
}
