using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEditor.Rendering;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    [SerializeField,Range(1,40)]private int maxItems = 1;
    [SerializeField]private InputActionReference eatInput;
    [SerializeField]private InputActionReference cookInput;
    [SerializeField,Range(1,5)]private int healthAddAmount = 1;
    [SerializeField]private TextMeshProUGUI rawBunnies;
    [SerializeField]private TextMeshProUGUI cookedBunnies;
    private int noOfCookedBunnies;
    private int noOfRawBunnies;
    private bool onCampfire;
    private Cave currentCave;
    
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

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Campfire"))
        {
            onCampfire = true;
        }
        if(collider.CompareTag("AI") && collider.GetComponent<Bunny>() && collider.GetComponent<Bunny>().IsDead())
        {
            AmendItems(1,Collectible.RawBunny);
            Destroy(collider.gameObject); 
        }
        if(collider.CompareTag("Cave"))
        {
            currentCave = collider.GetComponent<Cave>();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag("Campfire"))
        {
            onCampfire = false;
        }
        if(collider.CompareTag("Cave"))
        {
            currentCave = null;
        }
    }

    public void AmendItems(int amount, Collectible type)
    {
        if(type == Collectible.RawBunny)
        {
            noOfRawBunnies += amount;
            rawBunnies.text = noOfRawBunnies.ToString();
            return;
        }
        noOfCookedBunnies += amount;
        cookedBunnies.text = noOfCookedBunnies.ToString();
    }

    public void OnEatInput(InputAction.CallbackContext input)
    {
        if(input.performed && noOfCookedBunnies != 0)
        {
            PlayerCombat.Instance.AddHealth(healthAddAmount);
            AmendItems(-1,Collectible.CookedBunny);
        }
    }

    public void OnCookInput(InputAction.CallbackContext input)
    {
        if(input.performed && noOfRawBunnies != 0 && onCampfire)
        {
            AmendItems(1,Collectible.CookedBunny);
            AmendItems(-1,Collectible.RawBunny);
        }
        if(currentCave != null)
        {
            currentCave.Send(transform);
        }
    }

    void OnEnable()
    {
        eatInput.action.Enable();
        cookInput.action.Enable();
        eatInput.action.performed += OnEatInput;
        cookInput.action.performed += OnCookInput;
    }

    void OnDisable()
    {
        eatInput.action.Disable();
        cookInput.action.Disable();
        eatInput.action.performed -= OnEatInput;
        cookInput.action.performed -= OnCookInput;
    }

    public void SetRawBunnies(int newValue) 
    {
        noOfRawBunnies = newValue;
        rawBunnies.text = noOfRawBunnies.ToString();
    }
    public int GetNoOfRawBunnies() {return noOfRawBunnies;}
    public void SetCookedBunnies(int newValue) 
    {
        noOfCookedBunnies = newValue;
        cookedBunnies.text = noOfCookedBunnies.ToString();
    }
    public int GetNoOfCookedBunnies() {return noOfCookedBunnies;}
}

public enum Collectible
{
    RawBunny,
    CookedBunny
}
