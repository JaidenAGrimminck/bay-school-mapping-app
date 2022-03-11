using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    public bool isGUIElement = false;

    private FadeType fading = FadeType.NONE;

    private Color targetColor;

    private float t;

    private bool inactiveAfter = false;

    void FixedUpdate()
    {
        if (fading != FadeType.NONE) {
            if (!isGUIElement) {
                GetComponent<Renderer>().material.color = Color.Lerp(
                    GetComponent<Renderer>().material.color, 
                    targetColor,
                    (fading == FadeType.IN ? fadeInTime : fadeOutTime) * Time.deltaTime
                );
            } else {
                GetComponent<CanvasGroup>().alpha = Mathf.Lerp(
                    fading == FadeType.OUT ? 1 : 0, 
                    fading == FadeType.OUT ? 0 : 1,
                    t
                );

                t += 0.02f / (fading == FadeType.OUT ? fadeOutTime : fadeInTime);
            }
            
            if (!isGUIElement ? GetComponent<Renderer>().material.color.a == targetColor.a : (fading == FadeType.IN ? GetComponent<CanvasGroup>().alpha >= 1 : GetComponent<CanvasGroup>().alpha <= 0)) {
                if (fading == FadeType.OUT) {
                    fading = FadeType.NONE;

                    this.gameObject.SetActive(!inactiveAfter);

                    return;
                }
                fading = FadeType.NONE;
            }
        }
    }

    public void fadeOut(bool setInactiveAfter) {
        setTargetColor(0);
        fading = FadeType.OUT;
        inactiveAfter = setInactiveAfter;
        t = 0;
    }

    public void fadeIn() {
        setTargetColor(isGUIElement ? 1:255);
        fading = FadeType.IN;
        t = 0;
    }

    public void setAlpha(int alpha) {
        setTargetColor(alpha);

        if (isGUIElement) {
            GetComponent<CanvasGroup>().alpha = alpha;

            return;
        }

        GetComponent<Renderer>().material.color = targetColor;
    }

    private void setTargetColor(int alpha) {
        Color colorCopy = isGUIElement ? new Color(0,0,0,alpha) : GetComponent<Renderer>().material.color;
        //Creating a new one just in case to not change the original.
        targetColor = new Color(colorCopy.r, colorCopy.g, colorCopy.b, alpha);
    }

    private enum FadeType
    {
        NONE,
        IN,
        OUT
    }
}
