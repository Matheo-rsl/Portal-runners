using UnityEngine;

public class GravityPortal : MonoBehaviour
{
    private bool hasBeenUsed = false;
    
    private void Start()
    {
        // Vérifie si le joueur existe
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null && playerController.IsGravityReversed())
            {
                // Si la gravité est inversée, place le portail en dessous de la plateforme
                transform.position = new Vector3(transform.position.x, transform.position.y - 2.4f, transform.position.z);
            }
        }
    }

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