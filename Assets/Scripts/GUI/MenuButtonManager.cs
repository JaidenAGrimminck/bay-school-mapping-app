using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{
    public RectTransform buttonBackground;

    public Color pressedColor;
    public Color restingColor;

    public void onPress(int buttonInMenu) {
        int[] xValues = {160, 540, 920};

        if (buttonInMenu < 0 || buttonInMenu > 2) return;

        buttonBackground.anchoredPosition = new Vector2(xValues[buttonInMenu], buttonBackground.anchoredPosition.y);
        
        buttonBackground.gameObject.GetComponent<Image>().color = pressedColor;
    }

    public void onRelease(int buttonInMenu) {
        buttonBackground.gameObject.GetComponent<Image>().color = restingColor;

        MenuManager.Instance.setMenu(buttonInMenu);

        if (buttonInMenu == 2) {
            SettingsManager.Instance.Load();
        }
    }
}
