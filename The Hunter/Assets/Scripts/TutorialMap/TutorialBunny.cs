using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialBunny : Bunny
{
    [SerializeField,Range(1,100)]private int tutorialTriggerDistance;
    private static bool pickUpTriggered;
    private static bool goBackTriggered;
    private static bool shootBunnyTriggered;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Arrow") && !pickUpTriggered)
        {
            pickUpTriggered = true;
            TutorialManager.Instance.SetState(TutorialState.PickUp);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") && !goBackTriggered)
        {
            goBackTriggered = true;
            TutorialManager.Instance.SetState(TutorialState.GoBack);
        }
    }

    void FixedUpdate()
    {
        if(dead) { return;}
        if(Mathf.Abs(GetDistanceFromPlayer()) < tutorialTriggerDistance && !shootBunnyTriggered)
        {
            shootBunnyTriggered = true;
            TutorialManager.Instance.SetState(TutorialState.ShootBunny);
        }
        switch(state)
        {
            case AIState.Idle:
            IdleMovement();
            Track();
            break;
            case AIState.Moving:
            Move();
            if(Mathf.Abs(GetDistanceFromPlayer()) >= triggerDistance)
            {
                state = AIState.Idle;
                characterController.Move(0,false,false);
                animator.SetFloat("speed", 0);
                return;
            }
            break;
        }
    }

    void OnDrawGizmos()
    {
        if(!drawGizmos) {return;}
        Gizmos.DrawWireSphere(transform.position,triggerDistance);
        Gizmos.DrawWireSphere(transform.position,tutorialTriggerDistance);
    }
}
