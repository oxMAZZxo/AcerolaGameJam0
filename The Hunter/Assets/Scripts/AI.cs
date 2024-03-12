using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CharacterController2D))]
public class AI : StaticAI
{
    [SerializeField,Range(1f,100f)]protected float moveSpeed = 1f;
    [SerializeField,Range(1,250)]protected int aliveTime = 1;
    protected CharacterController2D characterController;
        protected Rigidbody2D rb;

    void Start()
    {
        state = AIState.Tracking;
        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
}

public enum AIState
{
    Idle,
    Moving,
    Tracking,
    Attacking,
    Spawning
}
