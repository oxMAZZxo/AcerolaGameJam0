using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D),typeof(CharacterController2D))]
public class Troll : AI
{   
    [SerializeField,Range(1f,10f)]private float stoppingDistance = 1f;
    [Header("Combat")]
    [SerializeField,Range(1f,100f)]protected int atackDamage = 1;
    [SerializeField,Range(0.1f,10f)]private float attackInterval = 1f;
    [SerializeField,Tooltip("This is for the directional force for when the Troll is attacking the player")]private Vector2 jumpForce;
    [SerializeField,Tooltip("This is for the directional force for when the Troll is attacking the player")]private Vector2 pushBackForce;
    private float attackTimer;
    private Action attack;

    void Start()
    {
        currentHealth = maxHealth;
        state = NPCState.Tracking;
        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        attackTimer = attackInterval;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if(PlayerCombat.Instance.IsDead()) { return;}
        switch(state)
        {
            case NPCState.Tracking:
                Track();
                animator?.SetFloat("speed", 0);
            break;
            case NPCState.Moving:
                if(MathF.Abs(GetDistanceFromPlayer()) > triggerDistance)
                {
                    state = NPCState.Tracking;
                    characterController.Move(0,false,false);
                    break;
                } 
                Move();
            break;
            case NPCState.Attacking:
                if(MathF.Abs(GetDistanceFromPlayer()) > stoppingDistance)
                {
                    state = NPCState.Moving;
                    break;
                } 
                animator?.SetFloat("speed", 0);
                attackTimer += Time.fixedDeltaTime;
                if(attackTimer >= attackInterval)
                {
                    attack.Invoke();
                    attackTimer = 0;
                }
            break;
        } 
    }

    protected override void Track()
    {
        float distance = GetDistanceFromPlayer();
        if(Mathf.Abs(distance) <= triggerDistance)
        {
            state = NPCState.Moving;
        }
    }

    private void Move()
    {
        if(Mathf.Abs(GetDistanceFromPlayer()) <= stoppingDistance)
        {
            state = NPCState.Attacking;
            characterController.Move(0,false,false);
            return;
        }
        float direction = 1;
        if(!IsPlayerOnTheRight())
        {
            direction = -1f;
        }
        float movement = moveSpeed * direction * Time.fixedDeltaTime;
        characterController.Move(movement,false,false);  
        animator.SetFloat("speed", Mathf.Abs(movement));
    }

    private void OnAttack()
    {
        Vector2 attackDirection = jumpForce;
        if(!IsPlayerOnTheRight())
        {
            attackDirection.x = -jumpForce.x;
        }
        rb.AddForce(attackDirection,ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            PlayerCombat.Instance.TakeDamage(atackDamage);
            Vector2 pushBack = pushBackForce;
            if(IsPlayerOnTheRight())
            {
                pushBack.x = -pushBackForce.x;
            }
            rb.AddForce(pushBack,ForceMode2D.Force);
        }
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("hurt");
        if(currentHealth <= 0.1)
        {
            Die();
        }
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    void OnEnable()
    {
        attack += OnAttack;
    }

    void OnDisable()
    {
        attack -= OnAttack;
    }

    void OnDrawGizmos()
    {
        if(!drawGizmos) {return;}
        Gizmos.DrawWireSphere(transform.position,triggerDistance);
        Gizmos.DrawWireSphere(transform.position,stoppingDistance);
    }
}

