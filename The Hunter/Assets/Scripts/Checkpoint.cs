using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
public class Checkpoint : MonoBehaviour
{
    public Transform transformToSave;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") && GameManager.Instance.IsFinishedLoading())
        {
            GameManager.Instance.Save(transformToSave.position);
        }
    }

}
