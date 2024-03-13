using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Farmer : AI
{
    private CapsuleCollider2D myCollider;
    [SerializeField,Range(0,100)]private int minIdleChance = 1;
    [SerializeField,Range(0,100)]private int maxIdleChance = 1;
    private bool isDead;
    private AudioManager audioManager;

    void Start()
    {
        myCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        audioManager = GetComponent<AudioManager>();
    }

    public void CalculateNextAnimation()
    {
        if(isDead) {return;}
        int rnd = Random.Range(minIdleChance,maxIdleChance);
        if(rnd > (maxIdleChance - 10))
        {
            animator.SetBool("farm", true);
            animator.SetBool("idle1",false);
            animator.SetBool("idle3",false);
        }else
        {
            animator.SetBool("farm",false);
            switch(Random.Range(1,2))
            {
                case 2:
                    animator.SetBool("idle1",true);
                    animator.SetBool("idle3",false);
                break;
                case 1:
                    animator.SetBool("idle1",false);
                    animator.SetBool("idle3",true);
                break;
            }
        }
    }

    public void SetDeath(bool newValue)
    {
        isDead = newValue;
        animator.SetBool("isDead",newValue);
        animator.SetTrigger("dead");
        rb.gravityScale = 0;
        myCollider.isTrigger = true;
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth < 1)
        {
            Die();
        }
    }

    protected override void Die()
    {
        isDead = true;
        animator.SetBool("isDead",true);
        animator.SetTrigger("dead");
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        myCollider.isTrigger = true;
    }

    public void PlayFarmSound()
    {
        audioManager.Play("Farm");
    }
}
