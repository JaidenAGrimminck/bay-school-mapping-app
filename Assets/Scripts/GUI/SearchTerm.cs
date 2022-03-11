using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SearchTerm : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    public GameObject title;
    public GameObject description;
    public GameObject distance;

    public Color pressedColor;

    private Color normalColor;

    private float roomN;

    public void OnPointerDown(PointerEventData eventData){
        //Change Color
        this.GetComponent<Image>().color = pressedColor;
    }
    
    public void OnPointerUp(PointerEventData eventData){
        this.GetComponent<Image>().color = normalColor;

        SearchManager.Instance.onSearchTermClick(roomN);
    }

    public void setSearchTerm(string title_, string description_, string distance_, float roomNumber) {
        title.GetComponent<TMPro.TextMeshProUGUI>().text = title_;
        description.GetComponent<TMPro.TextMeshProUGUI>().text = description_;
        distance.GetComponent<TMPro.TextMeshProUGUI>().text = distance_;

        normalColor = this.GetComponent<Image>().color;

        roomN = roomNumber;
    }
}
