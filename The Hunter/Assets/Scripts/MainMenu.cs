using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
}
