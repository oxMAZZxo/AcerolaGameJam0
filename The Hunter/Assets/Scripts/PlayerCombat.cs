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
    [SerializeField]private GameObject arrowPrefab;
    [SerializeField]private InputActionReference combat;
    [SerializeField]private Transform firepoint;
    [SerializeField,Range(1f,1000f)]private float minShootForce = 100f;
    [SerializeField,Range(40f,10000f)]private float maxShootForce = 1000f;
    [SerializeField,Range(1f,1000f)]private float forceIncrementation = 100f;
    [SerializeField,Range(1,50f)]private float minDamageDealt = 1f;
    [SerializeField,Range(1,200f)]private float maxDamageDealt = 1f;
    [SerializeField,Range(1,200f)]private float damageIncrement = 1f;
    private float currentShootForce;
    private float currentDamageDealt;
    private bool combatHold;
    private PlayerMovement playerMovement;
    
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
        playerMovement = GetComponent<PlayerMovement>();
        currentShootForce = minShootForce;
        currentDamageDealt = minDamageDealt;
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
            yield return new WaitForSeconds(0);
        }
    }

    void OnCombatHold(InputAction.CallbackContext input)
    {
        combatHold = true;
        StartCoroutine(BowCharge());
    }

    void OnCombatRelease(InputAction.CallbackContext input)
    {
        combatHold = false;
        Shoot();
    }

    void Shoot()
    {
        Rigidbody2D currentarrowRB = Instantiate(arrowPrefab,firepoint.position,quaternion.identity).GetComponent<Rigidbody2D>();
        Arrow currentArrow = currentarrowRB.gameObject.GetComponent<Arrow>();
        currentArrow.SetDamage(currentDamageDealt);
        if(playerMovement.IsLookingRight())
        {
            currentarrowRB.AddForce(transform.right * currentShootForce,ForceMode2D.Force);
        }else
        {
            currentarrowRB.AddForce(-transform.right * currentShootForce,ForceMode2D.Force);
        }
        currentDamageDealt = minDamageDealt;
        currentShootForce = minShootForce;
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
}
