using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform objectToFollow;
    public Vector3 offset;
    public bool ignoreX;
    public bool ignoreY;
    private float originalY;
    private float originalX;

    void Start()
    {
        originalX = transform.position.x;
        originalY = transform.position.y;
    }

    void LateUpdate()
    {
        Vector3 newPosition = objectToFollow.position + offset;
        if(ignoreX)
        {
            newPosition.x = originalX;
        }
        if(ignoreY)
        {
            newPosition.y = originalY;
        }
        transform.position = newPosition;
        
    }
}
