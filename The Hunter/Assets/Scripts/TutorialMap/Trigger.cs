using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    private bool triggered;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") && !triggered)
        {
            triggered = true;
            TutorialManager.Instance.OnTreeline();
        }
    } 
}
