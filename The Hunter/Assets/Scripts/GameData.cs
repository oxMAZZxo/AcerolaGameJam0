using System;
using TMPro;
using UnityEngine;

public class GameData : MonoBehaviour
{   
    public static GameData Instance;
    private bool tutorialCompleted;
    private float lastSavedPlayerLocationX;
    private float lastSavedPlayerLocationY;
    private int rawBunnies;
    private int cookedBunnies;
    private int currentHealth;
    private bool[] portalsDestroyed = new bool[2];
    private bool inOverworld = true;
    private bool dataLoaded;
    [SerializeField]private TextMeshProUGUI log;
    [SerializeField]private bool loadGame;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }else
        {
            Instance = this;
            if(loadGame) {LoadGame();}
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadGame()
    {
        string data = SaveSystem.LoadData(Application.persistentDataPath + ("/GameData.txt"));
        if(data != null)
        {
            string[] tempArray = data.Split(",");
            tutorialCompleted = Convert.ToBoolean(tempArray[0]);
            lastSavedPlayerLocationX = Convert.ToSingle(tempArray[1]);
            lastSavedPlayerLocationY = Convert.ToSingle(tempArray[2]);
            currentHealth = Convert.ToInt32(tempArray[3]);
            rawBunnies = Convert.ToInt32(tempArray[4]);
            cookedBunnies = Convert.ToInt32(tempArray[5]);
            inOverworld = Convert.ToBoolean(tempArray[6]);
            string[] tempPortalsDestroyed = tempArray[7].Split("-");
            for(int i = 0; i < tempPortalsDestroyed.Length; i++)
            {
                portalsDestroyed[i] = Convert.ToBoolean(tempPortalsDestroyed[i]);
            }
            dataLoaded = true;
            if(GameManager.Instance != null)
            {
                GameManager.Instance.SetLogText(data);
            }
            if(log != null)
            {
                log.text = data;
            }
        }else
        {
            dataLoaded = false;
            if(GameManager.Instance != null)
            {
                GameManager.Instance.SetLogText("No data was loaded");
            }
            if(log != null)
            {
                log.text = "no data was loaded";
            }
        }        
        
    }

    public string SaveGame()
    {
        return SaveSystem.SaveData(DataToString(),Application.persistentDataPath + ("/GameData.txt"));
    }

    public string DataToString()
    {
        string data = tutorialCompleted.ToString() + "," + lastSavedPlayerLocationX + "," + lastSavedPlayerLocationY + "," 
        + currentHealth.ToString() + "," + rawBunnies.ToString() + "," + cookedBunnies.ToString() + "," + inOverworld.ToString() + ",";
        
        for(int i = 0; i < portalsDestroyed.Length; i++)
        {
            if(i == portalsDestroyed.Length -1)
            {
                data += portalsDestroyed[i].ToString();
            }else
            {
                data += portalsDestroyed[i].ToString() + "-";
            }
        }
        
        return data;
    }
    
    public bool isTutorialCompleted(){return tutorialCompleted;}
    public void SetTutorialCompleted(bool newValue){tutorialCompleted = newValue;}
    public float GetLastSavedPlayerLocationX(){return lastSavedPlayerLocationX;}
    public void SetLastSavedPlayerLocationX(float newValue){lastSavedPlayerLocationX = newValue;}
    public float GetLastSavedPlayerLocationY(){return lastSavedPlayerLocationY;}
    public void SetLastSavedPlayerLocationY(float newValue){lastSavedPlayerLocationY = newValue;}
    public void SetPortalDestroyed(Portal[] portals)
    {
        for(int i = 0; i < portals.Length; i++)
        {
            if(portals[i] == null)
            {
                portalsDestroyed[i] = true;
            }else
            {
                portalsDestroyed[i] = false;
            }
        }
    }
    public bool[] GetPortalsDestroyed(){return portalsDestroyed;}
    public void SetCurrentHealth(int newValue) {currentHealth = newValue;}
    public int GetCurrentHealth() {return currentHealth;}
    public void SetRawBunnies(int newValue) {rawBunnies = newValue;}
    public int GetNoOfRawBunnies() {return rawBunnies;}
    public void SetCookedBunnies(int newValue) {cookedBunnies = newValue;}
    public int GetNoOfCookedBunnies() {return cookedBunnies;}
    public bool IsDataLoaded(){return dataLoaded;}
    public bool IsInOverworld(){return inOverworld;}
    public void SetInOverworld(bool newValue){ inOverworld = newValue;}
}
