using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button replayButton;
    
    private ScoreManager scoreManager;
    private GameObject player;
    private bool isGameOver = false;
    private float gameOverTimer = 0f;
    private const float gameOverDelay = 1f; // Délai d'une seconde
    
    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        gameOverPanel.SetActive(false);
        replayButton.onClick.AddListener(RestartGame);
    }
    
    private void Update()
    {
        if (isGameOver) return; // Si game over, on ne fait plus rien
        
        // Cherche le joueur s'il n'existe pas
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }
        
        // Vérifie si le joueur est hors de l'écran
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(player.transform.position);
        bool isOffScreen = screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
        
        if (isOffScreen)
        {
            gameOverTimer += Time.deltaTime;
            if (gameOverTimer >= gameOverDelay)
            {
                ShowGameOver();
            }
        }
        else
        {
            gameOverTimer = 0f;
        }
    }
    
    private void ShowGameOver()
    {
        if (isGameOver) return; // Évite d'appeler plusieurs fois
        
        isGameOver = true;
        
        // Désactive le joueur au lieu de le détruire
        if (player != null)
        {
            player.SetActive(false);
        }
        
        // Affiche le score final
        if (finalScoreText != null && scoreManager != null)
        {
            finalScoreText.text = $"Score Final: {Mathf.FloorToInt(scoreManager.GetScore())}";
        }
        
        // Active le panel
        gameOverPanel.SetActive(true);
        
        // Pause le jeu
        Time.timeScale = 0f;
    }
    
    public void RestartGame()
    {
        // Reprend le temps normal
        Time.timeScale = 1f;
        
        // Recharge la scène
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
} 