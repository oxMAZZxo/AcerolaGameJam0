using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

public class TutorialCampfire : Campfire
{
    public bool cooked = false;
    [SerializeField]private InputActionReference cookInput;
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            if(TutorialManager.Instance.GetState() == TutorialState.GoBack)
            {
                TutorialManager.Instance.SetState(TutorialState.Cook);

            }
            inRange = true;
            animator.SetBool("inRange",inRange);
        }
    }

    void OnCookInput(InputAction.CallbackContext input)
    {
        if(!cooked && input.performed && TutorialManager.Instance.GetState() == TutorialState.Cook)
        {
            cooked = true;
            TutorialManager.Instance.SetState(TutorialState.Eat);
        }
    }

    void OnEnable()
    {
        cookInput.action.Enable();
        cookInput.action.performed += OnCookInput;
    }

    void OnDisable()
    {
        cookInput.action.Enable();
        cookInput.action.performed -= OnCookInput;
    }
}
