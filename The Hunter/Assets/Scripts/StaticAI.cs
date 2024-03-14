using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public abstract class StaticAI : MonoBehaviour
{
    [Header("AI")]
    [SerializeField,Range(1f,1000f)]protected int maxHealth = 1;
    [SerializeField]protected bool drawGizmos = true;
    [SerializeField,Range(1f,50f)]protected float triggerDistance = 1f;
    protected AIState state;
    protected float currentHealth;
    protected bool dead;
    protected Animator animator;
    [SerializeField]protected Location location;
    [SerializeField,Tooltip("OPTIONAL")]private GameObject deathParticle;
    [SerializeField,Tooltip("OPTIONAL")]protected Transform deathParticleSpawn;
    
    public virtual void TakeDamage(float damage){}

    protected virtual void Track(){}

    protected virtual void Die()
    {
        dead = true;
        if(deathParticle != null)
        {
            Instantiate(deathParticle,deathParticleSpawn.position,quaternion.identity);
        }
        Destroy(gameObject,0.2f);
    }

    public virtual bool IsDead() {return dead;}

    protected float GetDistanceFromPlayer() { return PlayerCombat.Instance.GetPosition().x - transform.position.x;}

    public void SetTriggerDistance(float distance) {triggerDistance = distance;}

    public void SetLocation(Location newValue){location = newValue;}

    protected bool IsPlayerOnTheRight()
    {
        float distance = GetDistanceFromPlayer();
        if(distance < 0)
        {   
            return false;
        }
        return true;
    }

    void OnDrawGizmos()
    {
        if(!drawGizmos) {return;}
        Gizmos.DrawWireSphere(transform.position,triggerDistance);
    }
}
