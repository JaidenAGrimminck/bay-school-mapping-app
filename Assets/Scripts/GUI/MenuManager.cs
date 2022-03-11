using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject homeMenu;
    public GameObject mapMenu;
    public GameObject settingsMenu;

    int currentMenu = 1;

    void Start()
    {
        currentMenu = 1;

        MenuManager.Instance = this;

        homeMenu.SetActive(false);
        mapMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void setMenu(int n) {
        currentMenu = n;

        for (int i = 0; i < 3; i++) {
            GameObject obj = (new GameObject[] {homeMenu, mapMenu, settingsMenu})[i];
            if (i == n) {
                obj.SetActive(true);
            } else obj.SetActive(false);
        }
    }

    public int getMenu() {
        return currentMenu;
    }

    public void openURL(string url) {
        string url_ = url; //Copying it so it doesn't change the original.

        if (!(url.Contains("://") && url.StartsWith("http"))) {
            url_ = "https://" + url;
        }

        Application.OpenURL(url_);
    }
}
