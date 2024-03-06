using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D),typeof(CharacterController2D))]
public class Troll : MonoBehaviour
{   
    [Header("Combat")]
    [SerializeField,Range(1f,500f)]private int maxHealth = 1;
    [SerializeField,Range(1f,100f)]private int atackDamage = 1;
    [SerializeField,Range(0.1f,10f)]private float attackInterval = 1f;
    private float currentHealth;
    [SerializeField,Tooltip("This is for the directional force for when the Troll is attacking the player")]private Vector2 jumpForce;
    [SerializeField,Tooltip("This is for the directional force for when the Troll is attacking the player")]private Vector2 pushBackForce;
    private float attackTimer;
    private Action attack;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("AI")]
    [SerializeField]private bool drawGizmos = true;
    [SerializeField,Range(1f,50f)]private float acceptanceRadius = 1f;
    [SerializeField,Range(1f,10f)]private float stoppingDistance = 1f;
    [SerializeField,Range(1f,100f)]private float moveSpeed = 1f;
    private CharacterController2D characterController;
    private EnemyState state;
    
    void Start()
    {
        currentHealth = maxHealth;
        state = EnemyState.Tracking;
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
            case EnemyState.Tracking:
                Track();
                animator?.SetFloat("speed", 0);
            break;
            case EnemyState.Moving:
                if(MathF.Abs(GetDistanceFromPlayer()) > acceptanceRadius)
                {
                    state = EnemyState.Tracking;
                    characterController.Move(0,false,false);
                    break;
                } 
                Move();
            break;
            case EnemyState.Attacking:
                if(MathF.Abs(GetDistanceFromPlayer()) > stoppingDistance)
                {
                    state = EnemyState.Moving;
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

    private void Track()
    {
        float distance = GetDistanceFromPlayer();
        if(Mathf.Abs(distance) <= acceptanceRadius)
        {
            state = EnemyState.Moving;
        }
    }

    private void Move()
    {
        if(Mathf.Abs(GetDistanceFromPlayer()) <= stoppingDistance)
        {
            state = EnemyState.Attacking;
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

    private bool IsPlayerOnTheRight()
    {
        float distance = GetDistanceFromPlayer();
        if(distance < 0)
        {   
            return false;
        }
        return true;
    }

    private float GetDistanceFromPlayer() { return PlayerCombat.Instance.GetPosition().x - transform.position.x;}

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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("hurt");
        if(currentHealth <= 0.1)
        {
            Death();
        }
    }

    public void Death()
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
        Gizmos.DrawWireSphere(transform.position,acceptanceRadius);
        Gizmos.DrawWireSphere(transform.position,stoppingDistance);
    }
}


public enum EnemyState
{
    Tracking,
    Moving,
    Attacking
}
