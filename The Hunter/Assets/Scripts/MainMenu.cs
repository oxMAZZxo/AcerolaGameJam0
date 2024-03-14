using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuDefaultButton;
    public GameObject playPanelDefaultButton;
    public GameObject settingsDefaultButton;
    public GameObject playPanel;
    public Settings settings;

    void Start()
    {
        string data = SaveSystem.LoadData(Application.persistentDataPath + ("/SettingsData.txt"));
        if(data != null)
        {
            string[] splitData = data.Split(",");
            // Debug.Log("Data before conversion: ");
            // Debug.Log(splitData[0]);
            // Debug.Log(splitData[1]);
            // Debug.Log("Data after conversion: ");
            // Debug.Log(Convert.ToInt32(splitData[0]));
            // Debug.Log(Convert.ToInt32(splitData[1]));
            settings.SetFramerate(Convert.ToInt32(splitData[0]));
            settings.SetFullScreenChoice(Convert.ToInt32(splitData[1]));
            settings.ApplyChanges();
        }else
        {
            settings.SetFramerate(60);
            settings.SetFullScreenChoice(0);
            settings.ApplyChanges();
        }
    }

    public void Play()
    {
        if(GameData.Instance.IsDataLoaded())
        {
            playPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(playPanelDefaultButton);
        }else{
            LoadMap();
        }
        
    }

    public void LoadMap()
    {
        if(GameData.Instance.IsTutorialCompleted())
        {
            SceneManager.LoadScene("MainMap");
        }else
        {
            SceneManager.LoadScene("TutorialScene");
        }
    }

    public void StartNew()
    {
        GameData.Instance.LoadDefaultValues();
        LoadMap();
    }

    public void Exit()
    {
        Application.Quit();
    }

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuDefaultButton);
    }

    public void PlayClickSound()
    {
        AudioManager.Global.Play("ButtonClick");
    }
}
