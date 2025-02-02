using UnityEngine;

public class GravityPortal : MonoBehaviour
{
    private bool hasBeenUsed = false;
    private Color portalUpColor = new Color(0.4f, 1f, 0.4f); // Vert clair
    private Color portalDownColor = new Color(1f, 0.6f, 0.1f); // Orange
    
    private void Start()
    {
        // Vérifie si le joueur existe et si la gravité est inversée
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null && playerController.IsGravityReversed())
            {
                // Portail du bas (orange) - position à -1.5
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y - 1.5f, // Changé à -1.5f
                    transform.position.z
                );
                GetComponent<SpriteRenderer>().color = portalDownColor;
            }
            else
            {
                // Portail du haut (vert)
                GetComponent<SpriteRenderer>().color = portalUpColor;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasBeenUsed && collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                hasBeenUsed = true;
                
                Transform platform = transform.parent;
                if (platform != null)
                {
                    float teleportOffset = 0.75f;
                    Vector3 newPosition = collision.transform.position;
                    
                    if (player.IsGravityReversed())
                    {
                        newPosition.y = platform.position.y + teleportOffset;
                    }
                    else
                    {
                        newPosition.y = platform.position.y - teleportOffset;
                    }
                    
                    collision.transform.position = newPosition;
                }
                
                player.ReverseGravity();
                Destroy(gameObject, 0.1f);
            }
        }
    }
} 