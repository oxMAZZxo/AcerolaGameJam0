using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D),typeof(CharacterController2D))]
public class AI : MonoBehaviour
{
    [Header("AI")]
    [SerializeField,Range(1f,500f)]protected int maxHealth = 1;
    [SerializeField]protected bool drawGizmos = true;
    [SerializeField,Range(1f,50f)]protected float triggerDistance = 1f;
    [SerializeField,Range(1f,100f)]protected float moveSpeed = 1f;
    protected Animator animator;
    protected NPCState state;
    protected Rigidbody2D rb;
    protected bool dead;
    protected CharacterController2D characterController;
    protected float currentHealth;

    void Start()
    {
        state = NPCState.Tracking;
        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(float damage){}

    protected virtual void Track(){}

    protected virtual void Die(){}

    protected float GetDistanceFromPlayer() { return PlayerCombat.Instance.GetPosition().x - transform.position.x;}

    protected bool IsPlayerOnTheRight()
    {
        float distance = GetDistanceFromPlayer();
        if(distance < 0)
        {   
            return false;
        }
        return true;
    }
}

public enum NPCState
{
    Idle,
    Moving,
    Tracking,
    Attacking
}
