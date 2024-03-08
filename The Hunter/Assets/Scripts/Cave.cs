using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cave : MonoBehaviour
{
    [SerializeField]private CaveType type;
    [SerializeField]private Transform takeTo;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            if(type == CaveType.PressToInteract)
            {
                animator.SetBool("selected",true);
            }else
            {
                Send(collider.gameObject.transform);    
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") && type == CaveType.PressToInteract)
        {
            animator.SetBool("selected",false);
        }
    }

    public void Send(Transform obj)
    {
        GameManager.Instance.ChangePlayerLocation();
        obj.transform.position = takeTo.transform.position;
    }
}

public enum CaveType{
    PressToInteract,
    Collision
}
