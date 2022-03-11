using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnCampusTracker : MonoBehaviour
{   
    TMPro.TextMeshProUGUI text;

    void Start() {
        text = this.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        if (!LocationManager.Instance.locationAllowed) {
            text.text = "Unknown";
            return;
        }

        //If onCampus of LocationManager.Instance is true, then set the text to "Yes" and set the color to green.
        //If onCampus of LocationManager.Instance is false, then set the text to "No" and set the color to red.
        if (LocationManager.Instance.onCampus)
        {
            text.text = "Yes";
            text.color = Color.green;
        }
        else
        {
            text.text = "No";
            text.color = Color.red;
        }

    }
}
