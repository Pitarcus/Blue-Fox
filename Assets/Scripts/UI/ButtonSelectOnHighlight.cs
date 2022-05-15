using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectOnHighlight : MonoBehaviour//, IPointerEnterHandler
{
    /*void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }*/
    public void OnPointerEnter()
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}