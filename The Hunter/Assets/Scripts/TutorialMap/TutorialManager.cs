using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [SerializeField]private Transform mainArea;
    [SerializeField]private Transform theWild;
    [SerializeField]private GameObject transitionPanel;
    [SerializeField]private GameObject cameraTransitionPanel;
    [SerializeField]private PolygonCollider2D mainAreaBoundary;
    [SerializeField]private PolygonCollider2D wildAreaBoundary;
    [SerializeField]private CinemachineVirtualCamera mainCamera;
    [SerializeField]private CinemachineVirtualCamera tutorialCamera;
    private CinemachineConfiner2D cinemachineConfiner;
    [SerializeField]private TextMeshProUGUI tutorialText;
    [SerializeField]private Farmer farmer;
    [SerializeField]private Transform farmerDeathTransform;
    [SerializeField]private GameObject troll;
    [SerializeField,Range(1,10)]private int cameraTransitionTime = 1;
    private bool isInMain;
    private TutorialState state;

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
        isInMain = true;
        state = TutorialState.Hunting;
        cinemachineConfiner = mainCamera.gameObject.GetComponent<CinemachineConfiner2D>();
    }

    void FixedUpdate()
    {
        switch(state)
        {
            case TutorialState.Hunting:
                tutorialText.text = "You need to hunt for food!";
            break;
            case TutorialState.ShootBunny:
                tutorialText.text = "Use the Left Mouse button to shoot an arrow at the bunny.";
            break;
            case TutorialState.PickUp:
                tutorialText.text = "Pick up the bunny";
            break;
            case TutorialState.GoBack:
                tutorialText.text = "Go back to the campfire to cook the catch!";
            break;
            case TutorialState.DefendYourself:
                tutorialText.text = "Defend yourself";
            break;
            case TutorialState.Ressurection:

            case TutorialState.Cook:
                tutorialText.text = "Press Q when standing on a campfire to cook the catch.";
            break;
            case TutorialState.Eat:
                tutorialText.text = "Press E to eat food.";
            break;
            
        }
    }

    public void OnTreeline()
    {
        transitionPanel.SetActive(true);
        PlayerMovement.Instance.enabled = false;
        PlayerCombat.Instance.enabled = false;
    }

    public void TakeTo()
    {
        if(isInMain)
        {
            TheWild();
            
        }else
        {
            Main();
            state = TutorialState.DefendYourself;
            farmer.SetDeath(true);
            farmer.transform.position = farmerDeathTransform.position;
            troll.SetActive(true);
            StartCoroutine(CameraTransitions());
        }
    }

    public void Main()
    {
        PlayerCombat.Instance.transform.position = mainArea.position;
        isInMain = true;
        cinemachineConfiner.m_BoundingShape2D = mainAreaBoundary;
    }

    public void TheWild()
    {
        PlayerCombat.Instance.transform.position = theWild.position;
        isInMain = false;
        cinemachineConfiner.m_BoundingShape2D = wildAreaBoundary;
    }

    public void EnablePlayer()
    {
        PlayerCombat.Instance.enabled = true;
        PlayerMovement.Instance.enabled = true;
        wildAreaBoundary.enabled = false;
    }

    IEnumerator CameraTransitions()
    {
        mainCamera.Priority = 0;
        tutorialCamera.Priority = 1;
        yield return new WaitForSeconds(cameraTransitionTime);
        cameraTransitionPanel.SetActive(true);
        mainCamera.Priority = 1;
        tutorialCamera.Priority = 0;
    }

    public void SetState(TutorialState newState) {state = newState;}

    public TutorialState GetState() {return state;}
}

public enum TutorialState{
    Hunting,
    ShootBunny,
    PickUp,
    GoBack,
    DefendYourself,
    Death,
    Ressurection,
    FinishedRessurection,
    Cook,
    Eat,
}