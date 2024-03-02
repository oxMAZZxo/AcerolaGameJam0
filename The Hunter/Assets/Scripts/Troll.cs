using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController2D))]
public class Troll : MonoBehaviour
{   
    [SerializeField,Range(1f,100f)]private int maxHealth = 1;
    private float currentHealth;

    [Header("AI")]
    [SerializeField]private bool drawGizmos = true;
    [SerializeField,Range(1f,50f)]private float acceptanceRadius = 1f;
    [SerializeField,Range(1f,10f)]private float stoppingDistance = 1f;
    [SerializeField,Range(1f,100f)]private float moveSpeed = 1f;
    private CharacterController2D characterController;
    private bool playerIsOnTheRight;
    private EnemyState state;

    
    
    void Start()
    {
        currentHealth = maxHealth;
        state = EnemyState.Tracking;
        characterController = GetComponent<CharacterController2D>();
    }

    void FixedUpdate()
    {
        switch(state)
        {
            case EnemyState.Tracking:
                Track();
            break;
            case EnemyState.Moving:
                if(MathF.Abs(GetDistanceFromPlayer()) > acceptanceRadius)
                {
                    state = EnemyState.Tracking;
                    characterController.Move(0,false,false);
                    return;
                } 
                Move();
            break;
            case EnemyState.Attacking:
                if(MathF.Abs(GetDistanceFromPlayer()) > stoppingDistance)
                {
                    state = EnemyState.Moving;
                    return;
                } 
                Attack();
            break;
        } 
    }

    private void Track()
    {
        float distance = GetDistanceFromPlayer();
        if(Mathf.Abs(distance) <= acceptanceRadius)
        {
            if(distance < 0)
            {   
                playerIsOnTheRight = false;
            }else
            {   
                playerIsOnTheRight = true;
            }
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
        if(!playerIsOnTheRight)
        {
            direction = -1f;
        }
        float movement = moveSpeed * direction * Time.fixedDeltaTime;
        characterController.Move(movement,false,false);  
    }

    private float GetDistanceFromPlayer() { return PlayerCombat.Instance.GetPosition().x - transform.position.x;}

    private void Attack()
    {
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0.1)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
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
