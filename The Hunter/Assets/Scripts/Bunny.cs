using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D),typeof(CharacterController2D))]
public class Bunny : MonoBehaviour
{
    [Header("AI")]
    [SerializeField]protected bool drawGizmos = true;
    [SerializeField,Range(1f,50f)]protected float acceptanceRadius = 1f;
    [SerializeField,Range(1f,100f)]private float moveSpeed = 1f;
    protected CharacterController2D characterController;
    protected NPCState state;
    private Animator animator;
    private float currentHealth;
    private Rigidbody2D rb;
    protected bool dead;
    private CapsuleCollider2D myCollider;

    void Start()
    {
        dead = false;
        currentHealth = 5;
        characterController = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
        state = NPCState.Idle;
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CapsuleCollider2D>();
        Physics2D.IgnoreLayerCollision(3,11,true);
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

    protected void Track()
    {
        float distance = GetDistanceFromPlayer();
        if(Mathf.Abs(distance) <= acceptanceRadius)
        {
            state = NPCState.Moving;
        }
    }

    protected void Move()
    {
        float direction = 1;
        if(IsPlayerOnTheRight())
        {
            direction = -1;
        }
        float movement = direction * moveSpeed * Time.fixedDeltaTime;
        characterController.Move(movement,false,false);
        animator.SetFloat("speed",direction);
    } 
    
    protected float GetDistanceFromPlayer() { return PlayerCombat.Instance.GetPosition().x - transform.position.x;}

    private bool IsPlayerOnTheRight()
    {
        float distance = GetDistanceFromPlayer();
        if(distance < 0)
        {   
            return false;
        }
        return true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth < 1)
        {
            Die();
        }
    }

    private void Die()
    {
        dead = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("dead",true);
        rb.gravityScale = 0;
        myCollider.isTrigger = true;
        Physics2D.IgnoreLayerCollision(3,11,false);
    }

    void OnDrawGizmos()
    {
        if(!drawGizmos) {return;}
        Gizmos.DrawWireSphere(transform.position,acceptanceRadius);
    }

    public bool IsDead(){return dead;}
}

public enum NPCState
{
    Idle,
    Moving
}
