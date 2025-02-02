using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 6f;
    
    [Header("Détection")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Gravité")]
    [SerializeField] private float gravityScale = 2f;
    
    [Header("Game Over")]
    [SerializeField] private float outOfBoundsTime = 1f;
    
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    
    private Rigidbody2D rb;
    private bool isGravityReversed = false;
    private bool isGrounded;
    private bool isReversingGravity = false;
    private float outOfBoundsTimer = 0f;
    private Camera mainCamera;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        
        // Configure le material physique pour glisser
        PhysicsMaterial2D slipperyMaterial = new PhysicsMaterial2D();
        slipperyMaterial.friction = 0f; // Pas de friction
        slipperyMaterial.bounciness = 0f; // Pas de rebond
        GetComponent<Collider2D>().sharedMaterial = slipperyMaterial;
    }
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Caméra principale non trouvée!");
        }
    }
    
    private void Update()
    {
        CheckGround();
        
        if (mainCamera != null)
        {
            CheckOutOfBounds();
        }
        
        // Déplacement horizontal constant
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        
        // Saut uniquement si au sol
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }
    
    private void CheckGround()
    {
        Vector2 rayDirection = isGravityReversed ? Vector2.up : Vector2.down;
        Vector2 rayStart = transform.position;
        
        bool centerHit = Physics2D.Raycast(rayStart, rayDirection, groundCheckDistance, groundLayer);
        bool leftHit = Physics2D.Raycast(rayStart + Vector2.left * 0.3f, rayDirection, groundCheckDistance, groundLayer);
        bool rightHit = Physics2D.Raycast(rayStart + Vector2.right * 0.3f, rayDirection, groundCheckDistance, groundLayer);
        
        isGrounded = centerHit || leftHit || rightHit;
        
        // Debug visuel des raycast
        Debug.DrawRay(rayStart, rayDirection * groundCheckDistance, isGrounded ? Color.green : Color.red);
        Debug.DrawRay(rayStart + Vector2.left * 0.3f, rayDirection * groundCheckDistance, isGrounded ? Color.green : Color.red);
        Debug.DrawRay(rayStart + Vector2.right * 0.3f, rayDirection * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
    
    private void Jump()
    {
        float jumpDirection = isGravityReversed ? -1f : 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpDirection);
    }
    
    public void ReverseGravity()
    {
        if (isReversingGravity) return;
        isReversingGravity = true;
        isGravityReversed = !isGravityReversed;
        rb.gravityScale *= -1;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        Invoke("ResetGravityReverse", 0.5f);
    }
    
    private void ResetGravityReverse()
    {
        isReversingGravity = false;
    }
    
    private void CheckOutOfBounds()
    {
        if (mainCamera == null) return;
        
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool isVisible = viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
        
        if (!isVisible)
        {
            outOfBoundsTimer += Time.deltaTime;
            if (outOfBoundsTimer >= outOfBoundsTime)
            {
                GameOver();
            }
        }
        else
        {
            outOfBoundsTimer = 0f;
        }
    }

    private void GameOver()
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null && gameOverScoreText != null)
        {
            float finalScore = scoreManager.GetScore();
            gameOverScoreText.text = $"Score Final: {finalScore}";
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGravityReversed()
    {
        return isGravityReversed;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}