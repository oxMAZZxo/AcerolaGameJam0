using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class MainMenu : MonoBehaviour
{
    public GameObject DefaultButton;
    public GameObject settingsDefaultButton;
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
        if(GameData.Instance.isTutorialCompleted())
        {
            SceneManager.LoadScene("MainMap");
        }else
        {
            SceneManager.LoadScene("TutorialScene");
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(DefaultButton);
    }
}
