using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D),typeof(CharacterController2D))]
public class Troll : AI
{   
    [SerializeField,Range(1f,10f)]protected float stoppingDistance = 1f;
    [Header("Combat")]
    [SerializeField,Range(1f,100f)]protected int attackDamage = 1;
    [SerializeField,Range(0.1f,10f)]protected float attackInterval = 1f;
    [SerializeField,Tooltip("This is for the directional force for when the Troll is attacking the player")]private Vector2 jumpForce;
    [SerializeField,Tooltip("This is for the directional force for when the Troll is attacking the player")]private Vector2 pushBackForce;
    protected float attackTimer;
    protected Action attack;
    private AudioManager audioManager;

    void Start()
    {
        if(deathParticleSpawn == null) { deathParticleSpawn = transform;}
        currentHealth = maxHealth;
        state = AIState.Tracking;
        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        attackTimer = attackInterval;
        animator = GetComponent<Animator>();
        audioManager =  GetComponent<AudioManager>();
        StartCoroutine(PlayIdleSound());
        Invoke("Die",aliveTime);
    }

    void FixedUpdate()
    {
        if(PlayerCombat.Instance.IsDead() || GameManager.Instance.HasGameEnded()) { return;}
        switch(state)
        {
            case AIState.Tracking:
                Track();
                animator?.SetFloat("speed", 0);
            break;
            case AIState.Moving:
                if(MathF.Abs(GetDistanceFromPlayer()) > triggerDistance )
                {
                    state = AIState.Tracking;
                    characterController.Move(0,false,false);
                    break;
                } 
                Move();
            break;
            case AIState.Attacking:
                if(MathF.Abs(GetDistanceFromPlayer()) > stoppingDistance)
                {
                    state = AIState.Moving;
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
        if(Mathf.Abs(distance) <= triggerDistance && location == GameManager.Instance.GetPlayerLocation())
        {
            state = AIState.Moving;
        }
    }

    private IEnumerator PlayIdleSound()
    {
        while(!dead)
        {

            if(UnityEngine.Random.Range(0,101) > 70 && !audioManager.IsSoundPlaying("Idle"))
            {
                audioManager.Play("Idle");
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    protected void Move()
    {
        if(Mathf.Abs(GetDistanceFromPlayer()) <= stoppingDistance)
        {
            state = AIState.Attacking;
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
        audioManager.Play("Attack");
        Vector2 attackDirection = jumpForce;
        if(!IsPlayerOnTheRight())
        {
            attackDirection.x = -jumpForce.x;
        }
        rb.AddForce(attackDirection,ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !dead)
        {
            PlayerCombat.Instance.TakeDamage(attackDamage);
            Vector2 pushBack = pushBackForce;
            if(IsPlayerOnTheRight())
            {
                pushBack.x = -pushBackForce.x;
            }
            rb.AddForce(pushBack,ForceMode2D.Force);
        }
        if(collision.collider.CompareTag("NPC"))
        {
            collision.gameObject.GetComponent<AI>().TakeDamage(attackDamage);
        }
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        audioManager.Play("Hurt");
        animator.SetTrigger("hurt");
        if(currentHealth <= 0.1)
        {
            Die();
        }
    }

    protected override void Die()
    {
        base.Die();
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

