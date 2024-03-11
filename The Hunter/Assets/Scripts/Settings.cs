using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour
{
    public GameObject defaultButton;
    public TextMeshProUGUI framerateText;
    public Slider framerateSlider;
    public TMP_Dropdown dropdown;
    private int framerate;
    private int fullscreenChoice;

    public void ChangeFramerateText()
    {
        framerate = (int)framerateSlider.value;
        framerateText.text = framerate.ToString();
    }

    public void ChangeDropdownChoice()
    {
        fullscreenChoice = dropdown.value;
    }

    public void ApplyChanges()
    {
        Application.targetFrameRate = framerate;
        switch (fullscreenChoice)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
            break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            break;
        }
        Debug.Log(Screen.fullScreenMode);
        Debug.Log(Application.targetFrameRate);
    }

    public void SaveSettings()
    {   
        string data = framerate.ToString() + "," + fullscreenChoice.ToString();
        SaveSystem.SaveData(data, Application.persistentDataPath + ("/SettingsData.txt"));
    }

    public void SetFramerate(int newValue) 
    {
        framerate = newValue;
        framerateSlider.value = framerate;    
    }
    public void SetFullScreenChoice(int newValue) 
    {
        fullscreenChoice = newValue;
        dropdown.value = newValue;
    }

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }
}
