using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{
    public bool drawGizmos;
    public static PlayerCombat Instance;
    [SerializeField,Range(1f,100f)]protected int maxHealth = 1;
    [SerializeField]protected GameObject arrowPrefab;
    [SerializeField]private InputActionReference combat;
    [SerializeField]private Transform firepoint;
    [SerializeField,Range(1f,1000f)]protected float minShootForce = 100f;
    [SerializeField,Range(40f,10000f)]protected float maxShootForce = 1000f;
    [SerializeField,Range(1f,1000f)]private float forceIncrementation = 100f;
    [SerializeField,Range(1,50f)]protected float minDamageDealt = 1f;
    [SerializeField,Range(1,200f)]private float maxDamageDealt = 1f;
    [SerializeField,Range(1,200f)]private float damageIncrement = 1f;
    [SerializeField,Range(0,2)]private float maxArrowGlowIntensity = 1;
    [SerializeField,Range(0.01f,0.5f)]private float minArrowGlowIntensity = 1f;
    [SerializeField,Range(0.1f,2f)]private float glowIntensityIncrement = 0.1f;
    [SerializeField,Range(1f,10f)]private float ressurectionExplosionRadius = 0.1f;
    [SerializeField]protected StatusBar healthBar;
    [SerializeField]protected StatusBar bowChargeBar;
    [SerializeField]private GameObject ressurectionParticle;
    protected float currentShootForce;
    protected float currentDamageDealt;
    private bool combatHold;
    protected int currentHealth;
    protected Animator animator;
    private GameObject currentArrowObj;
    private bool dead;
    private float currentIntensity;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentShootForce = minShootForce;
        currentDamageDealt = minDamageDealt;
        currentHealth = maxHealth;

        healthBar.SetMaxValue(maxHealth);
        healthBar.SetCurrentValue(currentHealth);
        bowChargeBar.SetMaxValue(Convert.ToInt32(maxShootForce));
        bowChargeBar.SetMinValue(Convert.ToInt32(minShootForce));
        bowChargeBar.SetCurrentValue(Convert.ToInt32(minShootForce));
        currentIntensity = minArrowGlowIntensity;
        animator = GetComponent<Animator>();
    }

    private IEnumerator BowCharge()
    {
        while(combatHold)
        {
            currentShootForce += forceIncrementation * Time.deltaTime;
            currentDamageDealt += damageIncrement * Time.deltaTime;
            currentIntensity += glowIntensityIncrement * Time.deltaTime;
            if(currentShootForce >= maxShootForce)
            {
                currentShootForce = maxShootForce;
            }
            if(currentDamageDealt >= maxDamageDealt)
            {
                currentDamageDealt = maxDamageDealt;
            }
            if(currentIntensity >= maxArrowGlowIntensity)
            {
                currentIntensity = maxArrowGlowIntensity;
            }
            currentArrowObj.GetComponent<Arrow>().SetLightIntensity(currentIntensity);
            bowChargeBar.SetCurrentValue(Convert.ToInt32(currentShootForce));
            yield return new WaitForSeconds(0);
        }
    }

    void OnCombatHold(InputAction.CallbackContext input)
    {
        PlayerMovement.Instance.enabled = false;
        combatHold = true;
        currentArrowObj = Instantiate(arrowPrefab,firepoint.position,quaternion.identity);
        currentArrowObj.GetComponent<Rigidbody2D>().gravityScale = 0;
        currentArrowObj.GetComponent<Arrow>().Shoot(false);
        StartCoroutine(BowCharge());
        animator.SetBool("chargeBow",true);
    }

    void OnCombatRelease(InputAction.CallbackContext input)
    {
        PlayerMovement.Instance.enabled = true;   
        combatHold = false;
        animator.SetBool("chargeBow",false);
        Shoot();
    }

    void Shoot()
    {
        Rigidbody2D currentArrowRB = currentArrowObj.GetComponent<Rigidbody2D>();
        Arrow currentArrow = currentArrowObj.gameObject.GetComponent<Arrow>();
        currentArrow.SetDamage(currentDamageDealt);
        if(PlayerMovement.Instance.IsLookingRight())
        {
            currentArrowRB.AddForce(transform.right * currentShootForce,ForceMode2D.Force);
        }else
        {
            currentArrowRB.AddForce(-transform.right * currentShootForce,ForceMode2D.Force);
        }
        currentArrowObj.GetComponent<Arrow>().Shoot(true);
        currentArrowRB.gravityScale = 0.7f;
        currentDamageDealt = minDamageDealt;
        currentShootForce = minShootForce;
        bowChargeBar.SetCurrentValue(Convert.ToInt32(minShootForce));
        currentIntensity = minArrowGlowIntensity;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetCurrentValue(currentHealth);
        animator.SetTrigger("hurt");
        if(currentHealth <= 0)
        {
            Death();
        }
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        healthBar.SetCurrentValue(currentHealth);
    }

    protected virtual void Death()
    {
        dead = true;
        Debug.Log("Player has died");
        animator.SetTrigger("death");
        animator.SetBool("isDead",true);
        PlayerMovement.Instance.enabled = false;
        enabled = false;
        StartCoroutine(GameManager.Instance.Restart());
    }

    public void Ressurect(int maxHealth, int maxShootForce, int minShootForce, int maxDamageDealt, int minDamageDealt)
    {
        enabled = true;
        this.maxHealth = maxHealth;
        this.maxShootForce = maxShootForce;
        this.minShootForce = minShootForce;
        this.maxDamageDealt = maxDamageDealt;
        this.minDamageDealt = minDamageDealt;
        currentHealth = maxHealth;
        healthBar.SetMaxValue(maxHealth);
        PlayerMovement.Instance.enabled = true;
        animator.SetBool("isDead",false);
        Instantiate(ressurectionParticle,transform.position,quaternion.identity);
        dead = false;
    }
    
    public void Ressurect()
    {
        maxHealth = maxHealth / 2;
        healthBar.SetMaxValue(maxHealth);
        currentHealth = maxHealth;
        PlayerMovement.Instance.enabled = true;
        animator.SetBool("isDead",false);
        Instantiate(ressurectionParticle,transform.position,quaternion.identity);
        dead = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,ressurectionExplosionRadius);
        foreach(Collider2D collider in colliders)
        {
            if(collider.CompareTag("AI"))
            {
                collider.GetComponent<StaticAI>().TakeDamage(80);
            }
        }
    }

    public void SetCurrentHealth(int newValue) 
    {
        currentHealth = newValue;
        healthBar.SetCurrentValue(currentHealth);
    }
    
    public int GetCurrentHealth() {return currentHealth;}
    
    void OnEnable()
    {
        combat.action.Enable();
        combat.action.performed += OnCombatHold;
        combat.action.canceled += OnCombatRelease;
    }

    void OnDisable()
    {
        combat.action.Disable();
        combat.action.performed -= OnCombatHold;
        combat.action.canceled -= OnCombatRelease;
    }
    public Vector2 GetPosition(){return transform.position;}

    public Transform GetFirepoint() {return firepoint;}

    public bool IsDead(){return dead;}

    void OnDrawGizmos()
    {
        if(!drawGizmos) {return;}
        Gizmos.DrawWireSphere(transform.position,ressurectionExplosionRadius);
    }
}
