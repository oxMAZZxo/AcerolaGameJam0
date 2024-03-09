using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Camera")]
    [SerializeField]private GameObject waterRender;
    [SerializeField]private PolygonCollider2D mainAreaCameraBoundary;
    [SerializeField]private PolygonCollider2D caveAreaCameraBoundary;
    [SerializeField]private CinemachineConfiner2D cameraConfiner;
    [Header("Lighting")]
    [SerializeField]private Light2D globalLight;
    [SerializeField,Range(0,1f)]private float overworldLighting;
    [SerializeField,Range(0,1f)]private float caveLighting;
    private bool isInOverworld = true;
    private Location playerLocation = Location.Overworld;
    [SerializeField]private Portal[] portals;
    private bool finishedLoading = false;

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
        }
        finishedLoading = true;

    }

    public void ChangePlayerLocation()
    {
        if(isInOverworld)
        {
            waterRender.SetActive(false);
            cameraConfiner.m_BoundingShape2D = caveAreaCameraBoundary;
            isInOverworld = false;
            globalLight.intensity = caveLighting;
        }else
        {
            waterRender.SetActive(true);
            cameraConfiner.m_BoundingShape2D = mainAreaCameraBoundary;
            isInOverworld = true;
            globalLight.intensity = overworldLighting;
        }
    }

    public Location GetPlayerLocation() {return playerLocation;}

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
}

public enum Location{
    Overworld,
    Cave
}
