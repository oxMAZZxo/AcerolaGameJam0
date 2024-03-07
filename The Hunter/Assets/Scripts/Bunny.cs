using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D),typeof(CharacterController2D))]
public class Bunny : AI
{
    [SerializeField,Range(1,100)]private int aliveTime = 1;

    [Header("Idle Movement")]
    [SerializeField,Range(1,10)]protected int maxDistanceToMove = 1;
    [SerializeField,Range(1,100)]protected int idleMovementChance = 1;
    [SerializeField,Range(1,30)]protected int waitUntilNextMovementTime = 1;
    private CapsuleCollider2D myCollider;
    private bool moving;

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
        Invoke("Die", aliveTime);
    }

    void FixedUpdate()
    {
        if(dead) { return;}
        switch(state)
        {
            case NPCState.Idle:
            IdleMovement();
            Track();
            break;
            case NPCState.Moving:
            Move();
            if(Mathf.Abs(GetDistanceFromPlayer()) >= triggerDistance)
            {
                state = NPCState.Idle;
                characterController.Move(0,false,false);
                animator.SetFloat("speed", 0);
                return;
            }
            break;
        }
    }

    protected void IdleMovement()
    {
        int shouldMove = UnityEngine.Random.Range(1,100);
        if(moving || shouldMove < idleMovementChance){return;}
        int direction = UnityEngine.Random.Range(-1,2);
        if(direction != 0)
        {   
            moving = true;
            int distance = UnityEngine.Random.Range(1,maxDistanceToMove);
            distance *= direction;
            float movement = moveSpeed * direction * Time.fixedDeltaTime;
            StartCoroutine(Movement(distance,movement, direction));
        }
    }

    protected IEnumerator Movement(int distance, float movement, int direction)
    {
        float originalPos = transform.position.x;
        float desiredPos = originalPos + distance;
        bool stop = false;
        while(stop == false)
        {
            
            characterController.Move(movement,false,false);
            animator.SetFloat("speed", Mathf.Abs(movement));
            if(direction > 0 && transform.position.x > desiredPos || direction < 0 && transform.position.x < desiredPos)
            {
                stop = true;
                animator.SetFloat("speed", 0);
            }            
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(waitUntilNextMovementTime);
        moving = false;
    }

    protected override void Track()
    {
        float distance = GetDistanceFromPlayer();
        if(Mathf.Abs(distance) <= triggerDistance)
        {
            state = NPCState.Moving;
            StopAllCoroutines();
            moving = false;
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
        animator.SetFloat("speed",Mathf.Abs(direction));
    } 
    
    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth < 1)
        {
            Die();
        }
    }

    protected override void Die()
    {
        StopAllCoroutines();
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
        Gizmos.DrawWireSphere(transform.position,triggerDistance);
    }

    public bool IsDead(){return dead;}
}
