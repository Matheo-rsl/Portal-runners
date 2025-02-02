using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    
    [Header("Détection du sol")]
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Gravité")]
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private float teleportOffset = 1.5f;
    
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
        // Initialisation des composants
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D manquant sur le Player!");
        }
        
        // Initialisation de la gravité
        isGravityReversed = false;
        if (rb != null)
        {
            rb.gravityScale = gravityScale;
        }
    }
    
    private void Start()
    {
        // Recherche de la caméra principale
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Caméra principale non trouvée!");
        }
    }
    
    private void Update()
    {
        if (rb == null) return;
        
        CheckGround();
        
        if (mainCamera != null)
        {
            CheckOutOfBounds();
        }
        
        // Mouvement horizontal constant
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        
        // Gestion du saut
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }
    
    private void CheckGround()
    {
        Vector2 rayDirection = isGravityReversed ? Vector2.up : Vector2.down;
        Vector2 rayStart = transform.position;
        
        RaycastHit2D hitLeft = Physics2D.Raycast(rayStart + Vector2.left * 0.3f, rayDirection, groundCheckDistance, groundLayer);
        RaycastHit2D hitCenter = Physics2D.Raycast(rayStart, rayDirection, groundCheckDistance, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rayStart + Vector2.right * 0.3f, rayDirection, groundCheckDistance, groundLayer);
        
        isGrounded = hitLeft.collider != null || hitCenter.collider != null || hitRight.collider != null;
    }
    
    private void Jump()
    {
        if (rb == null) return;
        
        float jumpDirection = isGravityReversed ? -1f : 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpDirection);
    }
    
    public void ReverseGravity()
    {
        if (isReversingGravity || rb == null) return;
        
        isReversingGravity = true;
        
        try
        {
            // Cherche la plateforme la plus proche
            Collider2D[] nearbyPlatforms = Physics2D.OverlapCircleAll(transform.position, 5f, groundLayer);
            
            if (nearbyPlatforms.Length == 0)
            {
                Debug.LogWarning("Aucune plateforme trouvée!");
                return;
            }
            
            GameObject closestPlatform = null;
            float closestDistance = float.MaxValue;
            
            foreach (Collider2D platform in nearbyPlatforms)
            {
                if (platform != null)
                {
                    float distance = Mathf.Abs(platform.transform.position.y - transform.position.y);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlatform = platform.gameObject;
                    }
                }
            }
            
            if (closestPlatform != null)
            {
                // Inverse la gravité
                isGravityReversed = !isGravityReversed;
                rb.gravityScale *= -1;
                
                // Calcule la nouvelle position
                float platformY = closestPlatform.transform.position.y;
                float platformHeight = closestPlatform.transform.localScale.y;
                
                // Téléporte de l'autre côté de la plateforme
                float newY;
                if (isGravityReversed)
                {
                    newY = platformY - (platformHeight / 2) - teleportOffset;
                }
                else
                {
                    newY = platformY + (platformHeight / 2) + teleportOffset;
                }
                
                // Applique la nouvelle position
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                
                // Réinitialise la vélocité verticale
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erreur lors de l'inversion de la gravité : {e.Message}");
        }
        
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
        // Récupère le score final
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null && gameOverScoreText != null)
        {
            float finalScore = scoreManager.GetScore();
            gameOverScoreText.text = $"Score Final: {finalScore}";
        }
        
        // Recharge la scène
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGravityReversed()
    {
        return isGravityReversed;
    }
}