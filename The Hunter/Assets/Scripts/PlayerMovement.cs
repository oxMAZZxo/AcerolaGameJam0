using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    private CharacterController2D characterController2D;
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField,Range(1f,100f)]private float moveSpeed = 1f;
    [SerializeField,Range(1f,200f)]private float dashSpeed = 1f;
    [SerializeField]private InputActionReference movement;
    [SerializeField]private InputActionReference jump;
    [SerializeField]private InputActionReference dash;
    private bool isJumping;
    private bool inAir;
    private bool lookingRight;

    void Start()
    {
        lookingRight = true;
        characterController2D = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float direction = movement.action.ReadValue<float>() * moveSpeed * Time.fixedDeltaTime;

        if(rb.velocity.y != 0){inAir = true;}else{inAir = false;}
        
        
        if(inAir) {animator.SetFloat("speed",0); }else {animator.SetFloat("speed",Mathf.Abs(direction)); }
        animator.SetBool("inAir",inAir);

        characterController2D.Move(direction,false,isJumping);
        isJumping = false;
    }

    private void OnMove(InputAction.CallbackContext input)
    {
        if(input.performed)
        {
            if(input.action.ReadValue<float>() < 0)
            {
                lookingRight = false;
            }
            else
            {
                lookingRight = true;
            }
        }
    }

    private void OnJump(InputAction.CallbackContext input)
    {
        if(input.performed)
        {
            isJumping = true;
        }
    }

    private void OnDash(InputAction.CallbackContext input)
    {
        if(input.performed)
        {
            float dash = dashSpeed * 1; 
            if(!lookingRight)
            {
                dash = dashSpeed * -1;
            }
            characterController2D.Move(dash,false,isJumping);
        }
    }

    void OnEnable()
    {
        movement.action.Enable();
        jump.action.Enable();
        dash.action.Enable();
        movement.action.performed += OnMove;
        jump.action.performed += OnJump;
        dash.action.performed += OnDash;
    }

    void OnDisable()
    {
        movement.action.Disable();
        jump.action.Disable();
        dash.action.Disable();
        movement.action.performed -= OnMove;
        jump.action.performed -= OnJump;
        dash.action.performed -= OnDash;
    }

    public bool IsLookingRight() {return lookingRight;}
}
