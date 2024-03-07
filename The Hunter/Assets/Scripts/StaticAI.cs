using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D))]
public abstract class StaticAI : MonoBehaviour
{
    [Header("AI")]
    [SerializeField,Range(1f,500f)]protected int maxHealth = 1;
    [SerializeField]protected bool drawGizmos = true;
    [SerializeField,Range(1f,50f)]protected float triggerDistance = 1f;
    protected AIState state;
    protected float currentHealth;
    protected Rigidbody2D rb;
    protected bool dead;
    protected Animator animator;
    [SerializeField]private GameObject deathParticle;
    [SerializeField,Tooltip("OPTIONAL")]protected Transform deathParticleSpawn;
    
    public virtual void TakeDamage(float damage){}

    protected virtual void Track(){}

    protected virtual void Die()
    {
        if(deathParticle != null)
        {
            Instantiate(deathParticle,deathParticleSpawn.position,quaternion.identity);
        }
        Destroy(gameObject);
    }

    public virtual bool IsDead() {return dead;}

    protected float GetDistanceFromPlayer() { return PlayerCombat.Instance.GetPosition().x - transform.position.x;}

    void OnDrawGizmos()
    {
        if(!drawGizmos) {return;}
        Gizmos.DrawWireSphere(transform.position,triggerDistance);
    }
}
