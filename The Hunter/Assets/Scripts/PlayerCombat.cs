using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;
    [SerializeField,Range(1f,100f)]private int maxHealth = 1;
    [SerializeField]private GameObject arrowPrefab;
    [SerializeField]private InputActionReference combat;
    [SerializeField]private Transform firepoint;
    [SerializeField,Range(1f,1000f)]private float minShootForce = 100f;
    [SerializeField,Range(40f,10000f)]private float maxShootForce = 1000f;
    [SerializeField,Range(1f,1000f)]private float forceIncrementation = 100f;
    [SerializeField,Range(1,50f)]private float minDamageDealt = 1f;
    [SerializeField,Range(1,200f)]private float maxDamageDealt = 1f;
    [SerializeField,Range(1,200f)]private float damageIncrement = 1f;
    [SerializeField]private StatusBar healthBar;
    [SerializeField]private StatusBar bowChargeBar;
    private float currentShootForce;
    private float currentDamageDealt;
    private bool combatHold;
    private int currentHealth;
    private Animator animator;
    private GameObject currentArrowObj;

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
        currentHealth = 50;

        healthBar.SetMaxValue(maxHealth);
        healthBar.SetCurrentValue(currentHealth);
        bowChargeBar.SetMaxValue(Convert.ToInt32(maxShootForce));
        bowChargeBar.SetMinValue(Convert.ToInt32(minShootForce));
        bowChargeBar.SetCurrentValue(Convert.ToInt32(minShootForce));
        
        animator = GetComponent<Animator>();
    }

    private IEnumerator BowCharge()
    {
        while(combatHold)
        {
            currentShootForce += forceIncrementation * Time.deltaTime;
            currentDamageDealt += damageIncrement * Time.deltaTime;
            if(currentShootForce >= maxShootForce)
            {
                currentShootForce = maxShootForce;
            }
            if(currentDamageDealt >= maxDamageDealt)
            {
                currentDamageDealt = maxDamageDealt;
            }
            bowChargeBar.SetCurrentValue(Convert.ToInt32(currentShootForce));
            yield return new WaitForSeconds(0);
        }
    }

    void OnCombatHold(InputAction.CallbackContext input)
    {
        PlayerMovement.Instance.enabled = false;
        combatHold = true;
        StartCoroutine(BowCharge());
        currentArrowObj = Instantiate(arrowPrefab,firepoint.position,quaternion.identity);
        currentArrowObj.GetComponent<Rigidbody2D>().gravityScale = 0;
        currentArrowObj.GetComponent<Arrow>().Shoot(false);
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

    private void Death()
    {
        Debug.Log("Player has died");
    }
    
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
}
