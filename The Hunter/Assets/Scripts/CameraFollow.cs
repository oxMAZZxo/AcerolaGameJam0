using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform objectToFollow;
    public Vector3 offset;

    void LateUpdate()
    {
        transform.position = objectToFollow.position + offset;
    }
}
