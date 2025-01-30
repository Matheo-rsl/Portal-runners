using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    
    [Header("Détection du sol")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Gravité")]
    [SerializeField] private float gravityScale = 2f;
    
    private Rigidbody2D rb;
    private bool isGravityReversed = false;
    private bool isGrounded;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        
        // Vérifie que le Layer est bien configuré
        if (groundLayer.value == 0)
        {
            Debug.LogError("Ground Layer n'est pas configuré sur le PlayerController!");
        }
    }
    
    private void Update()
    {
        CheckGround();
        
        // Déplacement automatique
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        
        // Debug du statut au sol
        Debug.Log($"Is Grounded: {isGrounded}");
        
        // Saut avec espace
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            Debug.Log("Jump attempted!");
        }
    }
    
    private void CheckGround()
    {
        Vector2 rayDirection = isGravityReversed ? Vector2.up : Vector2.down;
        Vector2 rayStart = transform.position;
        
        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
        
        // Debug visuel
        Color rayColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayStart, rayDirection * groundCheckDistance, rayColor);
    }
    
    private void Jump()
    {
        float jumpDirection = isGravityReversed ? -1f : 1f;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce * jumpDirection);
    }
    
    public void ReverseGravity()
    {
        isGravityReversed = !isGravityReversed;
        rb.gravityScale *= -1;
    }
} 