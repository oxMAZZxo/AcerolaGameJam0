using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTroll : Troll
{
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
