using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTroll : Troll
{
    protected override void Die()
    {
        TutorialManager.Instance.SetState(TutorialState.GoBack);
        base.Die();
    }

    public void DecreaseStats(int currentHealth,int damage)
    {
        base.currentHealth = currentHealth;
        base.atackDamage = damage;
    }
}
