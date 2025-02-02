using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private float score = 0;
    private GameObject player;
    private Vector3 lastPosition;
    private float scoreMultiplier = 0.2f; // Multiplicateur de score ajouté
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lastPosition = player.transform.position;
        }
    }
    
    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                lastPosition = player.transform.position;
            }
            return;
        }
        
        float distance = player.transform.position.x - lastPosition.x;
        score += distance * scoreMultiplier; // Multiplication par 0.2
        lastPosition = player.transform.position;
        
        if (scoreText != null)
        {
            scoreText.text = $"Score: {Mathf.FloorToInt(score)}";
        }
    }
    
    public float GetScore()
    {
        return score;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f; // Reprend le temps normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}