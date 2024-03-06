using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPanel : MonoBehaviour
{

    [SerializeField]private TutorialManager tutorialManager;
    
    public void StartDivineText()
    {
        StartCoroutine(tutorialManager.DisplayDivineWords());
    }

    public void In()
    {
        TutorialManager.Instance.TakeTo();
    }

    public void Out()
    {
        TutorialManager.Instance.EnablePlayer();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
