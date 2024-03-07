using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField]private Animator tutorialPanel;
    private bool isInMain;
    [SerializeField,Multiline]private string divineWords;
    [SerializeField]private TextMeshProUGUI divineText;
    private TutorialState state;
    [SerializeField]private GameObject bowCharge;
    [SerializeField]private TutorialTroll tutorialTroll;
    [SerializeField,Range(1,50f)]private int playerMinDamageDealt = 1;
    [SerializeField,Range(1,200f)]private int playerMaxDamageDealt = 1;
    [SerializeField,Range(1f,1000f)]private int playerMinShootForce = 100;
    [SerializeField,Range(40f,10000f)]private int playerMaxShootForce = 1000;
    [SerializeField,Range(1f,100f)]private int playerMaxHealth = 1;
    [SerializeField]private Color killColour;
    [SerializeField]private Color regularColour;
    [SerializeField]private GameObject playerHUD;
    [SerializeField]private Trigger treeline;
    bool eatShown;
    bool dashShown;

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
        state = TutorialState.NotStarted;
        cinemachineConfiner = mainCamera.gameObject.GetComponent<CinemachineConfiner2D>();
    }

    void FixedUpdate()
    {
        switch(state)
        {
            case TutorialState.Hunting:
                tutorialText.text = "You need to hunt for food!";
                tutorialText.color = regularColour;
            break;
            case TutorialState.ShootBunny:
                tutorialText.text = "Use the Left Mouse button to shoot an arrow at the bunny.";
                tutorialText.color = regularColour;
            break;
            case TutorialState.PickUp:
                tutorialText.text = "Pick up the bunny";
                tutorialText.color = regularColour;
            break;
            case TutorialState.GoBack:
                tutorialText.text = "Go back to the campfire to cook the catch!";
                tutorialText.color = regularColour;
            break;
            case TutorialState.DefendYourself:
                tutorialText.text = "Defend yourself";
                tutorialText.color = killColour;
            break;
            case TutorialState.Ressurection:
                tutorialText.text = "";
            break;
            case TutorialState.FinishedRessurection:
                tutorialText.text = "Kill the trol!";
                tutorialText.color = killColour;
            break;
            case TutorialState.Cook:
                tutorialText.text = "Press Q when standing on a campfire to cook the catch.";
                tutorialText.color = regularColour;
            break;
            case TutorialState.Eat:
                tutorialText.text = "Press E to eat food when you need to replenish your health.";
                tutorialText.color = regularColour;
            break;
            case TutorialState.Dash:
                tutorialText.text = "Press the Left Shift button to Dash";
                tutorialText.color = regularColour;
            break;
            case TutorialState.GoBackToWild:
                tutorialText.text = "Now go back to the wild to embark on a journey to KILL all trolls";
                tutorialText.color = killColour;
            break;
        }
        if(state == TutorialState.Eat && !eatShown)
        {
            eatShown = true;
            StartCoroutine(CountDownToNextState(TutorialState.Dash,3f));
        }
        if(state == TutorialState.Dash && !dashShown)
        {
            dashShown = true;
            StartCoroutine(CountDownToNextState(TutorialState.GoBackToWild,3f));
            treeline.Reset();
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
        if(state == TutorialState.GoBackToWild)
        {
            SceneManager.LoadScene("MainMap");
        }else
        {
            PlayerCombat.Instance.transform.position = theWild.position;
            isInMain = false;
            cinemachineConfiner.m_BoundingShape2D = wildAreaBoundary;
        }
    }

    public void EnablePlayer()
    {
        PlayerCombat.Instance.enabled = true;
        PlayerMovement.Instance.enabled = true;
        wildAreaBoundary.enabled = false;
        playerHUD.SetActive(true);
    }

    IEnumerator CameraTransitions()
    {
        mainCamera.Priority = 0;
        tutorialCamera.Priority = 1;
        playerHUD.SetActive(false);
        yield return new WaitForSeconds(cameraTransitionTime);
        cameraTransitionPanel.SetActive(true);
        mainCamera.Priority = 1;
        tutorialCamera.Priority = 0;
    }

    public IEnumerator DisplayDivineWords()
    {
        int previousCharacterValue = 0;
        for(int currentLetter = 0; currentLetter < divineWords.Length; currentLetter ++)
        {
            divineText.text += divineWords[currentLetter];
            int characterValue = (int)divineText.text[currentLetter];
            if(characterValue == 10 && characterValue != previousCharacterValue)
            {
                yield return new WaitForSeconds(0.4f);
            }else
            {
                yield return new WaitForSeconds(0.05f);
            }
            previousCharacterValue = characterValue;
        }
        yield return new WaitForSeconds(2f);
        bowCharge.SetActive(true);
        tutorialPanel.SetTrigger("Out");
    }

    IEnumerator CountDownToNextState(TutorialState newState,float time)
    {
        yield return new WaitForSeconds(time);
        SetState(newState);
    }
    public void SetState(TutorialState newState) {state = newState;}

    public TutorialState GetState() {return state;}

    public Animator GetTutorialPanelAnimator() {return tutorialPanel;}

    public void RessurectPlayer()
    {
        SetState(TutorialState.FinishedRessurection);
        tutorialTroll.DecreaseStats(20,10);
        PlayerCombat.Instance.Ressurect(playerMaxHealth,playerMaxShootForce,playerMinShootForce,playerMaxDamageDealt,playerMinDamageDealt);
    }
}

public enum TutorialState{
    NotStarted,
    Hunting,
    ShootBunny,
    PickUp,
    GoBack,
    DefendYourself,
    Ressurection,
    FinishedRessurection,
    Cook,
    Eat,
    Dash,
    GoBackToWild
}