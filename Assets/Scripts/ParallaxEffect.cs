using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    // A reference to the camera we are following
    public Transform cameraTransform;
    
    // How fast the background moves. 0 = stays still, 1 = moves with camera.
    // A value like 0.3 or 0.5 works well.
    [Header("Parallax Effect Multiplier")]
    public float parallaxEffectX = 0.5f;
    public float parallaxEffectY = 0.5f;

    private float startPositionX;
    private float startPositionY;
    private float length;

    void Start()
    {
        // Get the starting position of this background object
        startPositionX = transform.position.x;
        startPositionY = transform.position.y;
        
        // Get the horizontal size of the sprite
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // How far the camera has moved from its starting point
        float cameraDistX = (cameraTransform.position.x);
        float cameraDistY = (cameraTransform.position.y);

        // Calculate the new position for the background
        float newPosX = startPositionX + (cameraDistX * parallaxEffectX);
        float newPosY = startPositionY + (cameraDistY * parallaxEffectY);

        // Set the new position
        transform.position = new Vector3(newPosX, newPosY, transform.position.z);

        // --- This part handles the infinite repeating ---
        float temp = (cameraTransform.position.x * (1 - parallaxEffectX));
        
        if (temp > startPositionX + length) 
        {
            startPositionX += length;
        }
        else if (temp < startPositionX - length)
        {
            startPositionX -= length;
        }
    }
}