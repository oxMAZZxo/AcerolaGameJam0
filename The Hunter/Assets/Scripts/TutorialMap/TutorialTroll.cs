using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTroll : Troll
{
    protected override void Death()
    {
        TutorialManager.Instance.SetState(TutorialState.GoBack);
        base.Death();
    }

    public void DecreaseStats(int currentHealth,int damage)
    {
        base.currentHealth = currentHealth;
        base.atackDamage = damage;
    }
}
