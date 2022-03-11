using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpDownController : MonoBehaviour
{
    float initialY;

    public float yAdj = 0;

    RectTransform rect;

    void Start() {
        rect = this.GetComponent<RectTransform>();
        initialY = rect.anchoredPosition.y;
    }


    public void setY(bool up) {
        if (up) {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, initialY + yAdj);
        } else {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, initialY);
        }
    }
}
