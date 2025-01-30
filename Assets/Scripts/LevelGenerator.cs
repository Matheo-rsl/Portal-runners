using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject playerPrefab;
    
    [Header("Generation Settings")]
    [SerializeField] private float platformDistance = 4f;
    [SerializeField] private float minY = -2f;
    [SerializeField] private float maxY = 2f;
    [SerializeField] private float portalProbability = 0.3f;
    [SerializeField] private float minVerticalDistance = 1f;
    
    private float lastPlatformX = 0f;
    private float lastPlatformY = 0f;
    private List<GameObject> generatedObjects = new List<GameObject>();
    private Transform player;
    
    private void Start()
    {
        // Nettoie les objets existants
        foreach (var obj in generatedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        generatedObjects.Clear();
        
        // Crée la plateforme initiale
        Vector2 startPosition = Vector2.zero;
        GameObject startPlatform = CreatePlatform(startPosition);
        
        // Place le joueur au-dessus de la première plateforme
        Vector2 playerSpawnPos = startPosition + new Vector2(0f, 1f);
        GameObject playerObj = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        player = playerObj.transform;
        
        lastPlatformX = 0f;
        lastPlatformY = 0f;
        
        // Génère les plateformes initiales
        for (int i = 0; i < 5; i++)
        {
            GenerateNextPlatform();
        }
    }
    
    private void Update()
    {
        if (player != null)
        {
            // Génère de nouvelles plateformes quand le joueur avance
            while (player.position.x > lastPlatformX - 10f)
            {
                GenerateNextPlatform();
            }
            
            // Nettoie les anciennes plateformes
            CleanupOldPlatforms();
        }
    }
    
    private void GenerateNextPlatform()
    {
        // Augmente la distance horizontale
        float nextX = lastPlatformX + platformDistance;
        
        // Calcule une nouvelle hauteur qui évite la superposition
        float heightChange = Random.Range(-1f, 1f);
        float nextY = Mathf.Clamp(lastPlatformY + heightChange, minY, maxY);
        
        // S'assure que la plateforme n'est pas trop proche de la dernière
        if (Mathf.Abs(nextY - lastPlatformY) < minVerticalDistance)
        {
            nextY = lastPlatformY + (heightChange >= 0 ? minVerticalDistance : -minVerticalDistance);
        }
        
        Vector2 position = new Vector2(nextX, nextY);
        GameObject platform = CreatePlatform(position);
        
        if (Random.value < portalProbability)
        {
            CreatePortal(position);
        }
        
        lastPlatformX = nextX;
        lastPlatformY = nextY;
    }
    
    private GameObject CreatePlatform(Vector2 position)
    {
        GameObject platform = Instantiate(platformPrefab, position, Quaternion.identity);
        generatedObjects.Add(platform);
        return platform;
    }
    
    private void CreatePortal(Vector2 platformPosition)
    {
        Vector2 portalPosition = platformPosition + new Vector2(0f, 1f);
        GameObject portal = Instantiate(portalPrefab, portalPosition, Quaternion.identity);
        generatedObjects.Add(portal);
    }
    
    private void CleanupOldPlatforms()
    {
        if (player == null) return;
        
        float cleanupX = player.position.x - 15f;
        generatedObjects.RemoveAll(obj => 
        {
            if (obj != null && obj.transform.position.x < cleanupX)
            {
                Destroy(obj);
                return true;
            }
            return false;
        });
    }
} 