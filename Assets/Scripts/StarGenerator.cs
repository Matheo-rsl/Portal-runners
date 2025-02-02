using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    [SerializeField] private int starsPerUnit = 3; // Nombre d'étoiles par unité de largeur
    [SerializeField] private float minStarSize = 0.05f;
    [SerializeField] private float maxStarSize = 0.15f;
    [SerializeField] private Color starColor = new Color(1f, 1f, 1f, 0.8f);
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;
    [SerializeField] private float generationWidth = 30f;
    [SerializeField] private float removalDistance = 30f;
    
    // Paramètres de parallaxe inversée
    [SerializeField] private float farStarSpeed = 0.2f; // Les étoiles lointaines bougent à 20% de la vitesse
    [SerializeField] private float nearStarSpeed = 0.4f; // Les étoiles proches bougent à 40% de la vitesse
    
    private Camera mainCamera;
    private GameObject starsContainer;
    private float lastGeneratedX;
    private float starSpacing;
    private Vector3 lastCameraPosition;
    
    private void Start()
    {
        mainCamera = Camera.main;
        starsContainer = new GameObject("Stars Container");
        starSpacing = 1f / starsPerUnit; // Espace entre les étoiles
        lastCameraPosition = mainCamera.transform.position;
        
        // Position initiale de génération
        float startX = mainCamera.transform.position.x - removalDistance;
        lastGeneratedX = startX;
        
        // Génère les premières étoiles de manière uniforme
        while (lastGeneratedX < mainCamera.transform.position.x + generationWidth)
        {
            GenerateStar(lastGeneratedX);
            lastGeneratedX += Random.Range(starSpacing * 0.8f, starSpacing * 1.2f);
        }
    }
    
    private void LateUpdate()
    {
        float cameraX = mainCamera.transform.position.x;
        float deltaX = cameraX - lastCameraPosition.x;
        
        // Déplace chaque étoile selon sa vitesse de parallaxe
        foreach (Transform star in starsContainer.transform)
        {
            float parallaxSpeed = star.GetComponent<StarParallax>().parallaxSpeed;
            star.position += Vector3.right * (deltaX * parallaxSpeed); // Mouvement simplifié
        }
        
        // Génère de nouvelles étoiles
        while (lastGeneratedX < cameraX + generationWidth)
        {
            GenerateStar(lastGeneratedX);
            lastGeneratedX += Random.Range(starSpacing * 0.8f, starSpacing * 1.2f);
        }
        
        // Supprime les étoiles trop loin
        foreach (Transform star in starsContainer.transform)
        {
            if (star.position.x < cameraX - removalDistance)
            {
                Destroy(star.gameObject);
            }
        }
        
        lastCameraPosition = mainCamera.transform.position;
    }
    
    private void GenerateStar(float x)
    {
        GameObject star = new GameObject("Star");
        star.transform.parent = starsContainer.transform;
        
        float y = Random.Range(minY, maxY);
        star.transform.position = new Vector3(x, y, 1f);
        
        SpriteRenderer renderer = star.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateStarSprite();
        renderer.color = starColor;
        
        // La taille influence la vitesse de parallaxe (plus petites = plus lointaines = plus lentes)
        float sizeRatio = Random.Range(0f, 1f);
        float size = Mathf.Lerp(minStarSize, maxStarSize, sizeRatio);
        star.transform.localScale = new Vector3(size, size, 1f);
        
        // Les petites étoiles bougent plus lentement (effet de profondeur)
        StarParallax parallax = star.AddComponent<StarParallax>();
        parallax.parallaxSpeed = Mathf.Lerp(farStarSpeed, nearStarSpeed, sizeRatio);
        
        renderer.sortingOrder = -1;
    }
    
    private Sprite CreateStarSprite()
    {
        Texture2D texture = new Texture2D(2, 2);
        Color[] colors = new Color[4] { Color.white, Color.white, Color.white, Color.white };
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point; // Empêche le flou
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 100f);
    }
}

// Classe pour stocker la vitesse de parallaxe de chaque étoile
public class StarParallax : MonoBehaviour
{
    public float parallaxSpeed;
} 