using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]private GameObject arrowPrefab;
    [SerializeField]private InputActionReference combat;
    [SerializeField]private Transform firepoint;
    [SerializeField,Range(1f,1000f)]private float minShootForce = 100f;
    [SerializeField,Range(40f,10000f)]private float maxShootForce = 1000f;
    [SerializeField,Range(1f,1000f)]private float forceIncrementation = 100f;
    private float currentShootForce;
    private bool combatHold;
    private PlayerMovement playerMovement;
    
    void Start()
    {
        Physics2D.IgnoreLayerCollision(3,7);
        playerMovement = GetComponent<PlayerMovement>();
        currentShootForce = minShootForce;
    }

    private IEnumerator BowCharge()
    {
        while(combatHold)
        {
            currentShootForce += forceIncrementation * Time.deltaTime;
            if(currentShootForce >= maxShootForce)
            {
                currentShootForce = maxShootForce;
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
        if(playerMovement.IsLookingRight())
        {
            currentarrowRB.AddForce(transform.right * currentShootForce,ForceMode2D.Force);
        }else
        {
            currentarrowRB.AddForce(-transform.right * currentShootForce,ForceMode2D.Force);
        }
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
}
