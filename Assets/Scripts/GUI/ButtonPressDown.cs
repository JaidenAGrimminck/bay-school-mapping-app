using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    public int buttonNumber = 0;
    public MenuButtonManager buttonManager;
    
    public void OnPointerDown(PointerEventData eventData){
        buttonManager.onPress(buttonNumber);
    }
    
    public void OnPointerUp(PointerEventData eventData){
        buttonManager.onRelease(buttonNumber);
    }
}