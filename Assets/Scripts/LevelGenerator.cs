using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject playerPrefab;
    
    [Header("Generation Settings")]
    [SerializeField] private float platformDistance = 20f;     // Augmenté significativement
    [SerializeField] private float heightVariation = 2f;
    [SerializeField] private float minY = -3f;
    [SerializeField] private float maxY = 3f;
    [SerializeField] private float portalProbability = 0.3f;
    
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
        Vector2 playerSpawnPos = startPosition + Vector2.up * 1f;
        GameObject playerObj = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        player = playerObj.transform;
        
        lastPlatformX = 0f;
        lastPlatformY = 0f;
        
        // Génère les premières plateformes
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
        // Distance horizontale fixe plus grande
        float nextX = lastPlatformX + platformDistance;
        
        // Calcul de la hauteur avec plus de variation
        float heightChange = Random.Range(-heightVariation, heightVariation);
        float nextY = Mathf.Clamp(lastPlatformY + heightChange, minY, maxY);
        
        // Évite les plateformes trop proches en Y
        if (Mathf.Abs(nextY - lastPlatformY) < 1f)
        {
            nextY = lastPlatformY + (heightChange >= 0 ? 1f : -1f);
        }
        
        // Garde la plateforme dans les limites
        nextY = Mathf.Clamp(nextY, minY, maxY);
        
        Vector2 position = new Vector2(nextX, nextY);
        GameObject platform = CreatePlatform(position);
        
        // Portails sur les grands écarts
        if (Random.value < portalProbability && Mathf.Abs(nextY - lastPlatformY) > heightVariation * 0.5f)
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
        Vector2 portalPosition = platformPosition + Vector2.up * 1.2f;
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