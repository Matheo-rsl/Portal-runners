using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float initialPlatformDistance = 8.5f;
    [SerializeField] private float minY = -1.5f;
    [SerializeField] private float maxY = 1.5f;
    [SerializeField] private float portalProbability = 0.2f;
    [SerializeField] private float maxHeightDifference = 2.5f; // Maximum de différence de hauteur possible
    
    [Header("Generation Settings")]
    [SerializeField] private float generationDistance = 10f;
    [SerializeField] private float deletionDistance = 15f;
    
    private float lastGeneratedX;
    private float lastGeneratedY;
    private GameObject player;
    private List<GameObject> generatedObjects = new List<GameObject>();
    private ScoreManager scoreManager;
    private PlayerController playerController;
    private float currentPlatformDistance;
    private int lastDifficultyIncrease = 0;
    
    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        currentPlatformDistance = initialPlatformDistance;
        
        Vector2 firstPlatformPosition = new Vector2(0, 0);
        GameObject firstPlatform = Instantiate(platformPrefab, firstPlatformPosition, Quaternion.identity);
        generatedObjects.Add(firstPlatform);
        lastGeneratedY = 0;
        
        // Crée le joueur avec la nouvelle taille
        Vector2 playerPosition = firstPlatformPosition + Vector2.up * 0.5f;
        player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        player.transform.localScale = new Vector3(0.5f, 1f, 1f); // Large de 0.5 et haut de 1
        playerController = player.GetComponent<PlayerController>();
        
        lastGeneratedX = 0;
        
        // Génère les premières plateformes
        for (int i = 0; i < 5; i++)
        {
            GenerateNextPlatform();
        }
    }
    
    private void Update()
    {
        if (player != null && scoreManager != null)
        {
            // Augmente la difficulté tous les 15 points
            int currentScore = Mathf.FloorToInt(scoreManager.GetScore());
            int difficultyLevel = currentScore / 15;
            
            if (difficultyLevel > lastDifficultyIncrease)
            {
                // Augmente la vitesse du joueur de 5%
                float newSpeed = playerController.GetMoveSpeed() * 1.05f;
                playerController.SetMoveSpeed(newSpeed);
                
                // Augmente la distance entre les plateformes de 5%
                currentPlatformDistance *= 1.05f;
                
                lastDifficultyIncrease = difficultyLevel;
                Debug.Log($"Difficulté augmentée ! Niveau : {difficultyLevel}, Vitesse : {newSpeed}, Distance : {currentPlatformDistance}");
            }
            
            // Génère de nouvelles plateformes
            while (lastGeneratedX - player.transform.position.x < 10f)
            {
                GenerateNextPlatform();
            }
            
            // Supprime les anciennes plateformes
            for (int i = generatedObjects.Count - 1; i >= 0; i--)
            {
                if (generatedObjects[i] != null)
                {
                    float distance = generatedObjects[i].transform.position.x - player.transform.position.x;
                    if (distance < -15f)
                    {
                        Destroy(generatedObjects[i]);
                        generatedObjects.RemoveAt(i);
                    }
                }
                else
                {
                    generatedObjects.RemoveAt(i);
                }
            }
        }
    }
    
    private void GenerateNextPlatform()
    {
        float nextX = lastGeneratedX + currentPlatformDistance;
        float minPossibleY = Mathf.Max(minY, lastGeneratedY - maxHeightDifference);
        float maxPossibleY = Mathf.Min(maxY, lastGeneratedY + maxHeightDifference);
        float nextY = Random.Range(minPossibleY, maxPossibleY);
        Vector2 position = new Vector2(nextX, nextY);
        
        GameObject platform = Instantiate(platformPrefab, position, Quaternion.identity);
        generatedObjects.Add(platform);
        
        if (Random.value < portalProbability)
        {
            Vector3 portalPos = position;
            portalPos.y += 0.75f; // Changé à 0.75
            GameObject portal = Instantiate(portalPrefab, portalPos, Quaternion.identity);
            portal.transform.localScale = new Vector3(0.5f, 1f, 1f);
            portal.transform.parent = platform.transform;
            generatedObjects.Add(portal);
        }
        
        lastGeneratedX = nextX;
        lastGeneratedY = nextY;
    }
} 