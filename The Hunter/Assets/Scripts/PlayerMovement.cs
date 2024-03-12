using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController2D),typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    private CharacterController2D characterController2D;
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField,Range(1f,100f)]private float moveSpeed = 1f;
    [SerializeField,Range(1f,1000f)]private float dashSpeed = 1f;
    [SerializeField,Range(1f,10f)]private float dashCoolDownTime = 1f;
    [SerializeField,Range(1f,3f)]private float imunityTime = 1f;
    [SerializeField]private InputActionReference movement;
    [SerializeField]private InputActionReference jump;
    [SerializeField]private InputActionReference dash;
    [SerializeField]private Image dashImage;
    private bool isJumping;
    private bool inAir;
    private bool lookingRight;
    private bool isDashing;
    private bool canDash = true;

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
        isDashing = false;
        lookingRight = true;
        characterController2D = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float direction = movement.action.ReadValue<float>() * moveSpeed * Time.fixedDeltaTime;

        if(Mathf.Abs(rb.velocity.y) > 1.85f){inAir = true;}else{inAir = false;}
        
        if(inAir) {animator.SetFloat("speed",0); }else {animator.SetFloat("speed",Mathf.Abs(direction)); }
        animator.SetBool("inAir",inAir);

        if(!isDashing) {characterController2D.Move(direction,false,isJumping);}
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
        if(input.performed && GameData.Instance.CanDash() && canDash)
        {
            isDashing = true;
            float dash = dashSpeed * 1; 
            canDash = false;
            if(!lookingRight)
            {
                dash = dashSpeed * -1;
            }
            animator.SetTrigger("Dash");
            characterController2D.Move(dash,false,isJumping);
            isDashing = false;
            Color transparent = dashImage.color;
            transparent.a = 0;
            StartCoroutine(Imunity());
            StartCoroutine(DashCooldown(transparent,dashImage.color,dashCoolDownTime));
        }
    }

    private IEnumerator Imunity()
    {
        Physics2D.IgnoreLayerCollision(3,8,true);
        yield return new WaitForSeconds(imunityTime);
        Physics2D.IgnoreLayerCollision(3,8,false);
    }

    private IEnumerator DashCooldown(Color start, Color end, float duration)
    {
        for (float t=0f;t<duration;t+=Time.deltaTime) 
        {
            float normalizedTime = t/duration;
            
            dashImage.color = Color.Lerp(start, end, normalizedTime);
            yield return null;
        }
        dashImage.color = end;
        canDash = true;
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
        characterController2D.Move(0,false,false);
        animator.SetFloat("speed",0);
        if(!inAir)
        {

        rb.velocity = new Vector2(0,rb.velocity.y);
        }
    }

    public bool IsLookingRight() {return lookingRight;}
}
