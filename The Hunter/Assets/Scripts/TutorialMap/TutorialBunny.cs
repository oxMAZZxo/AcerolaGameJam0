using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialBunny : Bunny
{
    [SerializeField,Range(1,100)]private int triggerDistance;
    [SerializeField]private TutorialManager tutorialManager;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Arrow"))
        {
            tutorialManager.SetState(TutorialState.PickUp);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            tutorialManager.SetState(TutorialState.GoBack);
        }
    }

    void FixedUpdate()
    {
        if(dead) { return;}
        switch(state)
        {
            case NPCState.Idle:
            Track();
            break;
            case NPCState.Moving:
            tutorialManager.SetState(TutorialState.ShootBunny);
            Move();
            if(Mathf.Abs(GetDistanceFromPlayer()) >= acceptanceRadius)
            {
                state = NPCState.Idle;
                characterController.Move(0,false,false);
                return;
            }
            break;
        }
    }

    void OnDrawGizmos()
    {
        if(!drawGizmos) {return;}
        Gizmos.DrawWireSphere(transform.position,acceptanceRadius);
        Gizmos.DrawWireSphere(transform.position,triggerDistance);
    }
}
