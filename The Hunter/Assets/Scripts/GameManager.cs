using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Pausing")]
    [SerializeField]private InputActionReference pause;
    [SerializeField]private GameObject pausePanel;
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
    
    [Header("Other")]
    [SerializeField]private GameObject transitionPanel;
    [SerializeField]private TextMeshProUGUI log;


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
        finishedLoading = false;
        if(GameData.Instance.IsDateLoaded())
        {
            PlayerCombat.Instance.transform.position = new Vector3(GameData.Instance.GetLastSavedPlayerLocationX(),GameData.Instance.GetLastSavedPlayerLocationY(),0f);
            PlayerCombat.Instance.SetCurrentHealth(GameData.Instance.GetCurrentHealth());
            Inventory.Instance.SetRawBunnies(GameData.Instance.GetNoOfRawBunnies());
            Inventory.Instance.SetCookedBunnies(GameData.Instance.GetNoOfCookedBunnies());
            LoadPortals(GameData.Instance.GetPortalsDestroyed());
            inOverworld = GameData.Instance.IsInOverworld();
            ChangeAesthetic();
        }
        finishedLoading = true;
    }

    public void ChangePlayerLocation()
    {
        transitionPanel.SetActive(true);
        if(inOverworld)
        {
            waterRender.SetActive(false);
            cameraConfiner.m_BoundingShape2D = caveAreaCameraBoundary;
            inOverworld = false;
            globalLight.intensity = caveLighting;
        }else
        {
            waterRender.SetActive(true);
            cameraConfiner.m_BoundingShape2D = mainAreaCameraBoundary;
            inOverworld = true;
            globalLight.intensity = overworldLighting;
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
        }else
        {
            waterRender.SetActive(false);
            cameraConfiner.m_BoundingShape2D = caveAreaCameraBoundary;
            globalLight.intensity = caveLighting;
            playerLocation = Location.Cave;

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
    }

    public void Resume()
    {
        Time.timeScale = 1;
        PlayerCombat.Instance.enabled = true;
        PlayerMovement.Instance.enabled = true;
        Inventory.Instance.enabled = true;
        pausePanel.SetActive(false);
        isPaused = false;
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
    }

    void OnDisable()
    {
        pause.action.Disable();
        pause.action.performed -= OnPauseInput;
    }

    public TextMeshProUGUI GetLogText() { return log;}
    public void SetLogText(string text){log.text = text;}
}

public enum Location{
    Overworld,
    Cave
}
