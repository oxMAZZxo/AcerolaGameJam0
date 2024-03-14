using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Pausing")]
    [SerializeField]private InputActionReference pause;
    [SerializeField]private GameObject pausePanel;
    [SerializeField]private PauseMenu pauseMenu;
    private bool isPaused;
    [Header("Camera")]
    [SerializeField]private GameObject waterRender;
    [SerializeField]private PolygonCollider2D mainAreaCameraBoundary;
    [SerializeField]private PolygonCollider2D caveAreaCameraBoundary;
    [SerializeField]private CinemachineConfiner2D cameraConfiner;
    [Header("Lighting")]
    [SerializeField]private Light2D globalLight;
    [SerializeField,Range(0,1f)]private float overworldLighting;
    [SerializeField,Range(0,1f)]private float caveLighting;
    private bool inOverworld = true;
    private Location playerLocation = Location.Overworld;
    [SerializeField]private Portal[] portals;
    private bool finishedLoading = false;
    [SerializeField]private Color overworldLightColour;
    [SerializeField]private Color caveLightColour;
    [Header("Restart")]
    [SerializeField,Range(1,5)]private int maxResurrections = 1;
    [SerializeField]private GameObject ressurectionPanel;
    [SerializeField]private TextMeshProUGUI ressurectionText;
    [SerializeField,Range(1f,5f)]private float restartWaitTime = 1f;
    [SerializeField]private GameObject deathPanel;

    [Header("End Game")]
    [SerializeField]private GameObject endGamePanel;
    [SerializeField]private GameObject endButtonSelected;
    [Header("Other")]
    [SerializeField]private GameObject inPanel;
    [SerializeField]private GameObject savingSymbol;
    [SerializeField]private TextMeshProUGUI log;
    private bool endGame;
    public int resurectionsLeft;

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
        Loading();
    }

    void Loading()
    {
        finishedLoading = false;
        if(GameData.Instance.IsDataLoaded())
        {
            Load();
        }else
        {
            resurectionsLeft = maxResurrections;
        }
        finishedLoading = true;
    }

    void FixedUpdate()
    {
        if(!AudioManager.Global.IsSoundPlaying())
        {
            int musicIndex = UnityEngine.Random.Range(1,6);
            AudioManager.Global.Play("Music" + musicIndex);
        }
    }

    public void ChangePlayerLocation()
    {
        inPanel.SetActive(true);
        if(inOverworld)
        {
            waterRender.SetActive(false);
            cameraConfiner.m_BoundingShape2D = caveAreaCameraBoundary;
            inOverworld = false;
            globalLight.intensity = caveLighting;
            globalLight.color = caveLightColour;
            playerLocation = Location.Cave;
        }else
        {
            waterRender.SetActive(true);
            cameraConfiner.m_BoundingShape2D = mainAreaCameraBoundary;
            inOverworld = true;
            globalLight.intensity = overworldLighting;
            globalLight.color = overworldLightColour;
            playerLocation = Location.Overworld;
        }
    }

    public void ChangeAesthetic()
    {
        if(inOverworld)
        {
            waterRender.SetActive(true);
            cameraConfiner.m_BoundingShape2D = mainAreaCameraBoundary;
            globalLight.intensity = overworldLighting;
            playerLocation = Location.Overworld;
            globalLight.color = overworldLightColour;
        }else
        {
            waterRender.SetActive(false);
            cameraConfiner.m_BoundingShape2D = caveAreaCameraBoundary;
            globalLight.intensity = caveLighting;
            playerLocation = Location.Cave;
            globalLight.color = caveLightColour;

        }
    }

    public Location GetPlayerLocation() {return playerLocation;}
    public bool IsInOverworld(){return inOverworld;}
    public Portal[] GetPortals() {return portals;}

    public void LoadPortals(bool[] portalsDestroyed)
    {
        for(int i = 0; i < portalsDestroyed.Length; i++)
        {
            if(portalsDestroyed[i])
            {
                Destroy(portals[i].gameObject);
            }
        }
    }
    
    public bool IsFinishedLoading(){return finishedLoading;}

    void OnPauseInput(InputAction.CallbackContext input)
    {
        if(isPaused)
        {
            Resume();
        }else
        {
            Pause();
        }
        
    }

    public void Pause()
    {
        Time.timeScale = 0;
        PlayerCombat.Instance.enabled = false;
        PlayerMovement.Instance.enabled = false;
        Inventory.Instance.enabled = false;
        pausePanel.SetActive(true);
        isPaused = true;
        pauseMenu.Pause(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        PlayerCombat.Instance.enabled = true;
        PlayerMovement.Instance.enabled = true;
        Inventory.Instance.enabled = true;
        pausePanel.SetActive(false);
        isPaused = false;
        pauseMenu.Pause(false);

    }

    public void EnableSaveSymbol()
    {
        savingSymbol.SetActive(true);
    }

    public void SaveAndExit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    void OnEnable()
    {
        pause.action.Enable();
        pause.action.performed += OnPauseInput;
        Portal.onDestroy += CheckEndGame;
    }

    void OnDisable()
    {
        pause.action.Disable();
        pause.action.performed -= OnPauseInput;
        Portal.onDestroy -= CheckEndGame;
    }

    public TextMeshProUGUI GetLogText() { return log;}
    public void SetLogText(string text){log.text = text;}

    private void CheckEndGame()
    {
        StartCoroutine(CheckPortals());
    }

    public IEnumerator CheckPortals()
    {
        yield return new WaitForSeconds(1f);
        bool tempEndGame = true;
        foreach(Portal portal in portals)
        {
            if(portal != null){               
                tempEndGame = false;
            }
        }
        if(tempEndGame)
        {
            endGame = true;
            PlayerCombat.Instance.enabled = false;
            PlayerMovement.Instance.enabled = false;
            Inventory.Instance.enabled = false;
            endGamePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(endButtonSelected);
            Save(PlayerCombat.Instance.transform.position);
        }
    }

    public IEnumerator Restart()
    {
        if(resurectionsLeft >= 0)
        {
            resurectionsLeft -=1;
            ressurectionPanel.SetActive(true);
            ressurectionText.text = "You have " + resurectionsLeft + " resurections left!";
            yield return new WaitForSeconds(restartWaitTime);
            PlayerCombat.Instance.enabled = true;
            PlayerCombat.Instance.Ressurect();
        }else
        {
            deathPanel.SetActive(true);
        }
        
    }

    public bool HasGameEnded(){return endGame;}

    public int GetResurectionsLeft(){return resurectionsLeft;}

    public void Save(Vector2 position)
    {
        EnableSaveSymbol();
        GameData.Instance.SetLastSavedPlayerLocationX(position.x);
        GameData.Instance.SetLastSavedPlayerLocationY(position.y);
        GameData.Instance.SetPortalDestroyed(GetPortals());
        GameData.Instance.SetCurrentHealth(PlayerCombat.Instance.GetCurrentHealth());
        GameData.Instance.SetCookedBunnies(Inventory.Instance.GetNoOfCookedBunnies());
        GameData.Instance.SetRawBunnies(Inventory.Instance.GetNoOfRawBunnies());
        GameData.Instance.SetInOverworld(IsInOverworld());
        GameData.Instance.SetMaxHealth(PlayerCombat.Instance.GetMaxHealth());
        GameData.Instance.SetResurectionsLeft(GetResurectionsLeft());
        log.text = GameData.Instance.SaveGame();
    }

    public void Load()
    {
        Debug.ClearDeveloperConsole();
        Debug.Log("Game manager pulling data..");
        PlayerCombat.Instance.transform.position = new Vector3(GameData.Instance.GetLastSavedPlayerLocationX(),GameData.Instance.GetLastSavedPlayerLocationY(),0f);
        PlayerCombat.Instance.SetMaxHealth(GameData.Instance.GetMaxHealth());
        PlayerCombat.Instance.SetCurrentHealth(GameData.Instance.GetCurrentHealth());
        Inventory.Instance.SetRawBunnies(GameData.Instance.GetNoOfRawBunnies());
        Inventory.Instance.SetCookedBunnies(GameData.Instance.GetNoOfCookedBunnies());
        resurectionsLeft = GameData.Instance.GetResurectionsLeft();
        LoadPortals(GameData.Instance.GetPortalsDestroyed());
        inOverworld = GameData.Instance.IsInOverworld();
        Debug.Log("Gamemanager finished data pull");
        ChangeAesthetic();
        inPanel.SetActive(true);
    }
}

public enum Location{
    Overworld,
    Cave
}
