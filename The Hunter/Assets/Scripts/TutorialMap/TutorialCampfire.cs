using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialCampfire : Campfire
{
    [SerializeField]private TutorialManager tutorialManager;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            if(tutorialManager.GetState() == TutorialState.FinishedRessurection)
            {
                tutorialManager.SetState(TutorialState.Cook);

            }
            inRange = true;
            animator.SetBool("inRange",inRange);
        }
    }
}
