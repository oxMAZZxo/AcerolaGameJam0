using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    protected bool inRange = false;
    protected Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("inRange", inRange);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            inRange = true;
            animator.SetBool("inRange", inRange);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            inRange = false;
            animator.SetBool("inRange", inRange);
        }
    }
}
