using UnityEngine;
using System;

public class TutorialPlayerCombat : PlayerCombat
{
    void Start()
    {
        currentShootForce = minShootForce;
        currentDamageDealt = minDamageDealt;
        currentHealth = 50;

        healthBar.SetMaxValue(maxHealth);
        healthBar.SetCurrentValue(currentHealth);
        bowChargeBar.SetMaxValue(Convert.ToInt32(maxShootForce));
        bowChargeBar.SetMinValue(Convert.ToInt32(minShootForce));
        bowChargeBar.SetCurrentValue(Convert.ToInt32(minShootForce));
        
        animator = GetComponent<Animator>();
    }

    protected override void Death()
    {
        Debug.Log("Tutorial Player Combat");
    }
}
