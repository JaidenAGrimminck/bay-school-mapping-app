using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionListController : MonoBehaviour
{
    public GameObject direction;
    public GameObject searchBar;

    public GameObject stopButton;

    public GameObject listButton;

    public GameObject upDown;

    public Transform listObj;
    
    public DirectionsController dc;

    public int spacing = 150;

    public void Start() {
        gameObject.SetActive(false);
    }

    public void showDirections() {
        string[] directions = dc.getCurrentDirections();

        stopButton.SetActive(false);
        listButton.SetActive(false);
        searchBar.SetActive(false);
        upDown.SetActive(false);

        destroyDirections();

        for (int i = 0; i < directions.Length; i++) {
            GameObject newDirection = Instantiate(direction, listObj);
            //Get the first child of newDirection
            Transform child = newDirection.transform.GetChild(0);
            child.gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = (i + 1) + ". " + directions[i];
            //Set anchor position of newDirection to the top left
            newDirection.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(i * spacing) -75);
        }
    }

    public void hideDirections() {
        StartCoroutine(setEverythingActive());

        GetComponent<Fade>().fadeOut(true);
    }

    IEnumerator setEverythingActive() {
        yield return new WaitForSeconds(GetComponent<Fade>().fadeOutTime / 2);

        destroyDirections();

        stopButton.SetActive(true);
        listButton.SetActive(true);
        searchBar.SetActive(true);
        upDown.SetActive(true);
    }

    private void destroyDirections() {
        //destroy all of listObj's children
        foreach (Transform child in listObj) {
            Destroy(child.gameObject);
        }
    }
}
