using UnityEngine;
using System;
using Unity.VisualScripting;

public class TutorialPlayerCombat : PlayerCombat
{
    [SerializeField]private TutorialManager tutorialManager;
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
        base.Death();
        tutorialManager.SetState(TutorialState.Ressurection);
        tutorialManager.GetTutorialPanelAnimator().gameObject.SetActive(true);
    }
}
