using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SearchManager : MonoBehaviour, IPointerDownHandler
{
    public static SearchManager Instance;

    public GameObject searchTermsBackground;
    public GameObject placeholder;

    public GameObject searchTermHolders;

    public GameObject searchTermPrefab;

    public GameObject exitButton;

    public GameObject textInp;

    public GameObject roomNumberMenu;
    public GameObject roomNumberInput;

    public GameObject circleInstance;

    public DirectionsController dc;

    private List<Room> lastResults = new List<Room>();
    private LocationNode navigatingTo;
    private LocationNode navigatingFrom;

    public void Start() {
        Instance = this;

        navigatingTo = null;

        searchTermsBackground.GetComponent<Fade>().setAlpha(0);

        searchTermsBackground.SetActive(false);
        roomNumberMenu.SetActive(false);
        exitButton.SetActive(false);
    }

    public void OnPointerDown (PointerEventData eventData) {
        if (searchTermsBackground.activeSelf) return;

        searchTermsBackground.SetActive(true);
        searchTermsBackground.GetComponent<Fade>().fadeIn();

        placeholder.GetComponent<Text>().text = "Search";
        exitButton.SetActive(true);

        onEdit();
    }

    public void onSearchTermClick(float roomN) {
        navigatingTo = null;

        foreach (LocationNode node in LocationNode.Locations) {
            if (node.roomNumber == roomN) {
                navigatingTo = node;
            }
        }

        if (navigatingTo == null) Debug.Log("Room Not Found");

        roomNumberMenu.SetActive(true);
    }

    public void leaveSearchTermMenu() {
        roomNumberMenu.SetActive(false);
    }

    public void getRoomNumber() {
        string roomNumber = roomNumberInput.GetComponent<InputField>().text.ToLower();

        if (roomNumber == "") return;

        float add = 0;

        if (roomNumber.EndsWith("a") || roomNumber.EndsWith("b")) {
            add = roomNumber.EndsWith("a") ? 0.1f : 0.5f;
            roomNumber.Remove(roomNumber.Length - 1);
        }

        bool isNumeric = int.TryParse(roomNumber, out _);

        if (isNumeric) {
            int n = int.Parse(roomNumber);
            float finalN = n + add;
            LocationNode node = null;

            foreach (LocationNode v in LocationNode.Locations) {
                if (v.roomNumber == n) {
                    node = v;
                    break;
                }
            }

            if (node == null) {
                Debug.Log("Room doesn't exist. TODO: create prompt for this.");
                return;
            }

            navigatingFrom = node;
        } else if (roomNumber.Contains("project center") || roomNumber.Contains("pc")) {
            LocationNode pc = null;

            foreach (LocationNode v in LocationNode.Locations) {
                if (v.roomNumber.ToString().StartsWith("50")) {
                    pc = v;
                    break;
                }
            }

            navigatingFrom = pc;
        }

        Debug.Log("Navigating to Room " + navigatingTo.roomNumber);

        roomNumberMenu.SetActive(false);

        int currentFloor = Room.getFloorFromRoomNumber(navigatingFrom.roomNumber);
        int toFloor = Room.getFloorFromRoomNumber(navigatingTo.roomNumber);

        

        if (currentFloor == 50) {
            //Project center
            if (toFloor == 50) {
                Debug.Log("You have arrived at your location.");
            }
        } else if (toFloor == 50) {
            //To project center
        } else if (currentFloor > toFloor) {
            Layer layer = Layer.getLayerFromFloor(currentFloor);
            float distance = 9999999999;
            int index = -1;
            int i = 0;
            foreach (LocationNode staircase in layer.staircaseEndings) {
                if (Vector3.Distance(staircase.transform.position, navigatingFrom.transform.position) < distance) {
                    distance = Vector3.Distance(staircase.transform.position, navigatingFrom.transform.position);
                    index = i;
                }
                i++;
            }
            
            Debug.Log("Closest Staircase to go down is " + (distance * LocationManager.Instance.getScaleForFloor(currentFloor)) + " meters away.");

            if (index == -1) {
                Debug.Log("No Staircase to go up.");
                return;
            }

            List<string> directions = new List<string>();
            List<float> distances = new List<float>();

            LocationNode closestStaircase = layer.staircaseEndings[index];

            // directions.Add("Go down staircase.");
            // distances.Add(distance * ((float) LocationManager.Instance.getScaleForFloor(currentFloor)));

            Vector3 lastStaircasePos = closestStaircase.transform.position;

            for (int j = currentFloor; j < toFloor; j++) {
                directions.Add("Go down staircase.");
                LocationNode closeStaircaseBeginning = TouchManager.Instance.layers[j].getClosestUpStaircase(lastStaircasePos);
                LocationNode closeStaircaseEnding = TouchManager.Instance.layers[j].getClosestDownStaircase(closeStaircaseBeginning.transform.position);
                distances.Add(Vector3.Distance(closeStaircaseBeginning.transform.position, closeStaircaseEnding.transform.position));
                lastStaircasePos = closeStaircaseEnding.transform.position;
            }

            directions.Add("Walk to room " + navigatingTo.roomNumber + ".");
            distances.Add(Vector3.Distance(lastStaircasePos, navigatingTo.transform.position));

            dc.gameObject.SetActive(true);
            dc.startDirections(directions, distances, toFloor, currentFloor, navigatingFrom, navigatingTo);
        } else if (currentFloor < toFloor) {
            Layer layer = Layer.getLayerFromFloor(currentFloor);

            float distance = 9999999999;
            int index = -1;
            int i = 0;
            foreach (LocationNode staircase in layer.staircaseBeginnings) {
                if (Vector3.Distance(staircase.transform.position, navigatingFrom.transform.position) < distance) {
                    distance = Vector3.Distance(staircase.transform.position, navigatingFrom.transform.position);
                    index = i;
                }
                i++;
            }

            if (index == -1) {
                Debug.Log("No Staircase to go up.");
                return;
            }

            List<string> directions = new List<string>();
            List<float> distances = new List<float>();

            LocationNode closestStaircase = layer.staircaseBeginnings[index];

            // directions.Add("Go up staircase");
            // distances.Add(distance * ((float) LocationManager.Instance.getScaleForFloor(currentFloor)));

            Vector3 lastStaircasePos = closestStaircase.transform.position;

            for (int j = currentFloor; j < toFloor; j++) {
                directions.Add("Go up staircase.");
                LocationNode closeStaircaseEnding = TouchManager.Instance.layers[j].getClosestDownStaircase(lastStaircasePos);
                LocationNode closeStaircaseBeginning = TouchManager.Instance.layers[j].getClosestUpStaircase(closeStaircaseEnding.transform.position);
                distances.Add(Vector3.Distance(closeStaircaseEnding.transform.position, closeStaircaseBeginning.transform.position));
                lastStaircasePos = closeStaircaseBeginning.transform.position;
            }

            directions.Add("Walk to room " + navigatingTo.roomNumber + ".");
            distances.Add(Vector3.Distance(lastStaircasePos, navigatingTo.transform.position));

            dc.gameObject.SetActive(true);
            dc.startDirections(directions, distances, toFloor, currentFloor, navigatingFrom, navigatingTo);

            foreach (string direction in directions) {
                Debug.Log(direction);
            }

        } else if (currentFloor == toFloor) {
            float distance = Vector3.Distance(navigatingFrom.transform.position, navigatingTo.transform.position);

            Debug.Log("You are " + (distance * LocationManager.Instance.getScaleForFloor(currentFloor)) + " meters away.");

            List<string> directions = new List<string>();
            List<float> distances = new List<float>();

            directions.Add("Walk to room " + navigatingTo.roomNumber + ".");
            distances.Add(distance * ((float) LocationManager.Instance.getScaleForFloor(currentFloor)));

            dc.gameObject.SetActive(true);
            dc.startDirections(directions, distances, toFloor, currentFloor, navigatingFrom, navigatingTo);

            //EditorUtility.DisplayDialog("Distance", "You are " + (distance * LocationManager.Instance.getScaleForFloor(currentFloor)) + " meters away.", "OK", "Cancel");
        }

        exitSearch();

    }

    public void onEdit() {
        string currentValue = this.GetComponent<InputField>().text;
        string[] keywords = currentValue.Split(' ');

        for (int ii = 0; ii < keywords.Length; ii++) {
            if (keywords[ii].Length < 1) keywords[ii] = "NO_STRING_WILL_CONTAIN_THIS_KEYWORD.";
        }

        IDictionary<string, int> results = new Dictionary<string, int>();

        List<Room> rooms = ApiManager.Instance.GetRooms();

        if (currentValue.Length < 2) {
            for (int i = 0; i < 7; i++) {
                if (i >= rooms.Count) break;

                results.Add(rooms[i].getName(), 7 - i);
            }
        } else {
            for (int i = 0; i < rooms.Count; i++) {
                for (int j = 0; j < keywords.Length; j++) {
                    if (rooms[i].getName().ToLower().Contains(keywords[j].ToLower())) {
                        if (results.ContainsKey(rooms[i].getName())) {
                            results[rooms[i].getName()] += 1;
                        } else results.Add(rooms[i].getName(), 1);
                    }
                }
            }
        }

        List<Room> finalResults = new List<Room>();

        int max = 0;
        
        //Find the highest int from the results dictionary.
        foreach (int calledN in results.Values) if (calledN > max) max = calledN;

        for (int i = max; i > 0; i--) {
            if (results.Values.Contains(i)) {
                foreach (string key in results.Keys) {
                    if (results[key] == i) {
                        finalResults.Add(ApiManager.Instance.GetRoomByName(key));
                    }
                }
            }
        }

        int keptAmount = 0;

        foreach (Transform child in searchTermHolders.transform) {
            if (keptAmount < results.Count) {
                keptAmount++; 
                continue;
            }

            GameObject.Destroy(child.gameObject);
            keptAmount++;
        }

        int currentGameObjectsMade = searchTermHolders.transform.childCount;

        //-2 = front desk, -3 = admin desk
        //-4 = library
        //x.1 = a
        //x.5 = b
        //starts w/ 50, ex: 50301, 50 is PC, then 301 is the room number

        //Display it ig
        for (int k = 0; k < 7; k++) {
            if (finalResults.Count <= k) break;
            
            GameObject searchTerm = currentGameObjectsMade <= k ? Instantiate(searchTermPrefab, searchTermHolders.transform) : searchTermHolders.transform.GetChild(k).gameObject;

            Room room = finalResults[k];

            string roomNumber = "";

            if (Mathf.Floor(room.getNumber()) < room.getNumber()) {
                roomNumber = Mathf.Floor(room.getNumber()).ToString() + (room.getNumber().ToString().EndsWith("1") ? "A" : "B");
            } else if (roomNumber.ToString().StartsWith("50")) {
                roomNumber = "PC - " + roomNumber.ToString().Substring(2);
            } else {
                roomNumber = room.getNumber().ToString();
            }

            if (room.getNumber() == -4) {
                roomNumber = "Library";
            } else if (room.getNumber() == -2) {
                roomNumber = "Front Desk";
            } else if (room.getNumber() == -3) {
                roomNumber = "Admin Desk";
            }

            string roomN = "Room Number: " + roomNumber;

            int rn = Room.getFloorFromRoomNumber(room.getNumber());

            string floor = (rn == 50 ? "PC" : (rn == 0 ? "Basement" : "Floor " + rn.ToString()));

            searchTerm.GetComponent<SearchTerm>().setSearchTerm(room.getName(), roomN, floor, room.getNumber());

            searchTerm.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75 - (150 * k));
        }
    }

    public void exitSearch() {
        searchTermsBackground.GetComponent<Fade>().fadeOut(true);

        placeholder.GetComponent<Text>().text = "Search here";
        exitButton.SetActive(false);
    }
}
