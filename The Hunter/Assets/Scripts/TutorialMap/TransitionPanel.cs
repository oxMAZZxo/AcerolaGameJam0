using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPanel : MonoBehaviour
{ 
    public void StartDivineText()
    {
        StartCoroutine(TutorialManager.Instance.DisplayDivineWords());
    }

    public void ResetPlayer()
    {
        TutorialManager.Instance.RessurectPlayer();
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
