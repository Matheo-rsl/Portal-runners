using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private float offsetX = 4f;
    [SerializeField] private float smoothSpeed = 5f;
    
    private Transform target;
    
    private void Update()
    {
        // Cherche le joueur à chaque frame s'il n'est pas trouvé
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            return;
        }
        
        // Calcule la position désirée de la caméra
        Vector3 desiredPosition = new Vector3(
            target.position.x + offsetX,
            transform.position.y,
            transform.position.z
        );
        
        // Déplace la caméra smoothly vers la position désirée
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
        
        transform.position = smoothedPosition;
    }
} 