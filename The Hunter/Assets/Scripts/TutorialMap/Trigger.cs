using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    private bool triggered;
    private BoxCollider2D myCollider;
    public TriggerType type;

    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") && !triggered && type == TriggerType.Treeline)
        {
            triggered = true;
            TutorialManager.Instance.OnTreeline();
            //myCollider.isTrigger = false;
        }

        if(collider.CompareTag("Player") && !triggered && type == TriggerType.StartTutorial)
        {
            triggered = true;
            TutorialManager.Instance.SetState(TutorialState.Hunting);
            Destroy(gameObject);
        }
    } 
}

public enum TriggerType
{
    Treeline,
    StartTutorial
}
