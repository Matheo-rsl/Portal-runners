using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float scoreMultiplier = 0.1f;
    
    private float score = 0f;
    private Vector3 startPosition;
    private Transform player;
    private bool isInitialized = false;
    
    private void Update()
    {
        if (!isInitialized)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                startPosition = player.position;
                isInitialized = true;
                Debug.Log("Joueur trouvé, position initiale : " + startPosition);
            }
        }
        
        if (player != null)
        {
            float distance = (player.position.x - startPosition.x);
            score = Mathf.Floor(distance * scoreMultiplier);
            UpdateScoreDisplay();
            
            if (score > 0)
            {
                Debug.Log("Distance : " + distance + ", Score : " + score);
            }
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