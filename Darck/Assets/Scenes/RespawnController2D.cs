using UnityEngine;

public class RespawnController2D : MonoBehaviour
{
    public Transform respawnPoint; // Assign your RespawnPoint here
    public GameObject player;      // Assign your Player object here

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            // Reset the player's position to the respawn point
            player.transform.position = respawnPoint.position;

            // Reset the player's velocity
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Stop any movement
            }
        }
    }
}