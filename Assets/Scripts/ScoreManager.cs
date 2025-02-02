using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private float scoreMultiplier = 1f;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private float score = 0f;
    private Vector3 startPosition;
    private Transform player;
    
    private void Start()
    {
        // Trouve le joueur
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            startPosition = player.position;
        }
        
        // Vérifie si le TextMeshProUGUI est assigné
        if (scoreText == null)
        {
            Debug.LogError("Score Text non assigné dans ScoreManager!");
        }
        
        UpdateScoreDisplay();
    }
    
    private void Update()
    {
        if (player != null)
        {
            // Calcule le score basé sur la distance parcourue
            float distance = player.position.x - startPosition.x;
            score = Mathf.Floor(distance * scoreMultiplier);
            
            UpdateScoreDisplay();
        }
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    public float GetScore()
    {
        return score;
    }
} 