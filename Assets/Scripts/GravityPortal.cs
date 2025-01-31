using UnityEngine;

public class GravityPortal : MonoBehaviour
{
    private bool hasBeenUsed = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenUsed) return;
        
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                hasBeenUsed = true;
                player.ReverseGravity();
                Destroy(gameObject, 0.1f); // Petit délai avant destruction
            }
        }
    }
} 