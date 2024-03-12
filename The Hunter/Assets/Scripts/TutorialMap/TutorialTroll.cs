using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialTroll : Troll
{

    void FixedUpdate()
    {
        if(PlayerCombat.Instance.IsDead()) { return;}
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
        if(Mathf.Abs(distance) <= triggerDistance)
        {
            state = AIState.Moving;
        }
    }

    protected override void Die()
    {
        TutorialManager.Instance.SetState(TutorialState.GoBack);
        base.Die();
    }

    public void DecreaseStats(int currentHealth,int damage)
    {
        base.currentHealth = currentHealth;
        base.attackDamage = damage;
    }
}
