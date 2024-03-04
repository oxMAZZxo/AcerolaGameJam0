using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [SerializeField]private Transform mainArea;
    [SerializeField]private Transform theWild;
    [SerializeField]private GameObject transitionPanel;
    [SerializeField]private PolygonCollider2D mainAreaBoundary;
    [SerializeField]private PolygonCollider2D wildAreaBoundary;
    [SerializeField]private CinemachineConfiner2D cinemachineConfiner;
    private bool isInMain;

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
        isInMain = true;
    }

    public void OnTreeline()
    {
        transitionPanel.SetActive(true);
        PlayerMovement.Instance.enabled = false;
        PlayerCombat.Instance.enabled = false;
    }

    public void TakeTo()
    {
        if(isInMain)
        {
            TheWild();
        }else
        {
            Main();
        }
    }

    public void Main()
    {
        PlayerCombat.Instance.transform.position = mainArea.position;
        isInMain = true;
        cinemachineConfiner.m_BoundingShape2D = mainAreaBoundary;
    }

    public void TheWild()
    {
        PlayerCombat.Instance.transform.position = theWild.position;
        isInMain = false;
        cinemachineConfiner.m_BoundingShape2D = wildAreaBoundary;
    }

    public void EnablePlayer()
    {
        PlayerCombat.Instance.enabled = true;
        PlayerMovement.Instance.enabled = true;
        wildAreaBoundary.enabled = false;
    }
}
