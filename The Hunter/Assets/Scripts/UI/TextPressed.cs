using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TextPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform textToMove;
    private Vector2 vector2;
    public float UnitsToMove;

    void Start()
    {
        vector2 = textToMove.GetComponent<RectTransform>().anchoredPosition;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        textToMove.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, UnitsToMove);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        textToMove.GetComponent<RectTransform>().anchoredPosition = new Vector2(vector2.x, vector2.y);
    }
}
