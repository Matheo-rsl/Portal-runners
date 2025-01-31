using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    
    [Header("Détection du sol")]
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Gravité")]
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private float teleportOffset = 1f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    
    private Rigidbody2D rb;
    private bool isGravityReversed = false;
    private bool isGrounded;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D manquant sur le Player!");
            return;
        }
        rb.gravityScale = gravityScale;
        
        // Debug au démarrage
        Debug.Log($"Ground Layer value: {groundLayer.value}");
        Debug.Log($"Ground Check Distance: {groundCheckDistance}");
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Debug des configurations
        Debug.Log($"Layer Mask Value: {groundLayer.value}");
        Debug.Log($"Player Y Position: {transform.position.y}");
        Debug.Log($"Player Layer: {gameObject.layer}");
        
        // Vérifie si une plateforme est sous le joueur
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider != null)
        {
            Debug.Log($"Found platform at start! Layer: {hit.collider.gameObject.layer}");
        }
        else
        {
            Debug.Log("No platform found at start!");
        }
    }
    
    private void Update()
    {
        if (rb == null) return;
        
        CheckGround();
        
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }
    
    private void CheckGround()
    {
        Vector2 rayDirection = isGravityReversed ? Vector2.up : Vector2.down;
        Vector2 rayStart = transform.position;
        
        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
        
        Debug.DrawRay(rayStart, rayDirection * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
    
    private void Jump()
    {
        float jumpDirection = isGravityReversed ? -1f : 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpDirection);
    }
    
    public void ReverseGravity()
    {
        Debug.Log("ReverseGravity appelé");
        
        // Trouve la plateforme la plus proche
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 2f, groundLayer);
        
        foreach (var collider in nearbyColliders)
        {
            Debug.Log($"Plateforme trouvée : {collider.gameObject.name}");
            
            // Calcule la position de téléportation
            float platformY = collider.transform.position.y;
            float platformHeight = collider.transform.localScale.y;
            
            // Position actuelle relative à la plateforme
            bool isAbove = transform.position.y > platformY;
            Debug.Log($"Position actuelle : au {(isAbove ? "dessus" : "dessous")} de la plateforme");
            
            // Nouvelle position
            Vector3 newPosition = transform.position;
            if (!isGravityReversed) // Si on va inverser la gravité
            {
                newPosition.y = platformY - platformHeight - teleportOffset;
            }
            else // Si on revient à la gravité normale
            {
                newPosition.y = platformY + platformHeight + teleportOffset;
            }
            
            Debug.Log($"Téléportation de {transform.position.y} à {newPosition.y}");
            transform.position = newPosition;
            break; // Utilise seulement la première plateforme trouvée
        }
        
        // Inverse la gravité
        isGravityReversed = !isGravityReversed;
        rb.gravityScale *= -1;
        
        Debug.Log($"Gravité inversée : {rb.gravityScale}");
    }
    
    private void OnDrawGizmos()
    {
        // Visualise la zone de détection des plateformes
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
        
        // Dessine des sphères pour visualiser les points de raycast
        if (showDebug)
        {
            Gizmos.color = Color.yellow;
            Vector3 pos = transform.position;
            Gizmos.DrawWireSphere(pos + Vector3.left * 0.3f, 0.1f);
            Gizmos.DrawWireSphere(pos, 0.1f);
            Gizmos.DrawWireSphere(pos + Vector3.right * 0.3f, 0.1f);
        }
    }
}