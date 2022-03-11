using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System;
using TMPro;

public class DirectionsController : MonoBehaviour
{
    private string[] currentDirections = new string[0];
    private float[] distances = new float[0];
    private int onDirection = 0;

    private bool currentlyNavigating = false;

    public Image stairs;
    public Image walk;
    public Image left;
    public Image right;
    public Image forward;

    public GameObject circlePrefab;

    public GameObject toObj;
    public GameObject timeObj;

    public GameObject upDownObj;

    List<List<GameObject>> circles = new List<List<GameObject>>();

    public GameObject fullDirections;

    // Start is called before the first frame update
    void Start()
    {
        currentDirections = new string[0]; //maybe load in directions from file?
        gameObject.SetActive(false);
    }

    void Update() {
        if (currentlyNavigating) {
            float left = 0;
            for (int i = 0; i < distances.Length; i++) {
                if (onDirection > i) continue;
                left += distances[i];
            }

            double distance;
            LocationManager.metersToFeet(left, out distance);

            DateTime localDate = DateTime.Now;
            localDate.AddSeconds(left / LocationManager.Instance.minutePerMeter * 60f);

            timeObj.GetComponent<TMPro.TextMeshProUGUI>().text = Mathf.Floor((float) distance) + " ft • " + (localDate.Hour > 12 ? localDate.Hour - 12 : localDate.Hour) + ":" + localDate.Minute + " " + (localDate.Hour <= 11 ? "A" : "P") + "M";
            int t = (int) Mathf.Floor(left / LocationManager.Instance.minutePerMeter);
            toObj.GetComponent<TMPro.TextMeshProUGUI>().text = t + " min" + (t == 1 ? "" : "s");
        }
    }

    public string[] getCurrentDirections() {
        return currentDirections;
    }

    public string getDirection() {
        return currentDirections[onDirection];
    }
    
    public void nextDirection() {
        onDirection++;
    }

    public void previousDirection() {
        onDirection--;
    }

    public void showFullDirections() {
        if (!currentlyNavigating) return;

        fullDirections.SetActive(true);
        fullDirections.GetComponent<Fade>().fadeIn();
        fullDirections.GetComponent<DirectionListController>().showDirections();
    }

    public void startDirections(List<string> directions, List<float> distances, int toFloor, int currentFloor, LocationNode from, LocationNode to) {
        
        currentDirections = directions.ToArray();
        this.distances = distances.ToArray();
        onDirection = 0;
        currentlyNavigating = true;

        TouchManager.Instance.setLayer(currentFloor);
        //Set camera position above the from locationNode

        upDownObj.GetComponent<UpDownController>().setY(true);

        circles = new List<List<GameObject>>();

        LocationNode lastStaircaseTaken = null;

        for (int layer_ = 0; layer_ < TouchManager.Instance.layers.Count; layer_++) {
            circles.Add(new List<GameObject>());

            Layer layer = TouchManager.Instance.layers[layer_];
            if (layer_ == toFloor && toFloor == currentFloor) {
                List<LocationNode> pathNodes = layer.getShortestDistancePathFromLocToLoc(from.transform.position, to.transform.position);

                //Generate circles between the from and to the first path node
                List<GameObject> firstCircleNodes = generateCirclesBetweenTwoPathNodes(from.transform, pathNodes[0].transform);
                circles[layer_].AddRange(firstCircleNodes);

                for (int i = 0; i < pathNodes.Count - 1; i++) {
                    List<GameObject> circleNodes = generateCirclesBetweenTwoPathNodes(pathNodes[i].transform, pathNodes[i + 1].transform);
                    circles[layer_].AddRange(circleNodes);
                }

                //Generate circles between the last node and the destination
                List<GameObject> lastCircleNodes = generateCirclesBetweenTwoPathNodes(pathNodes[pathNodes.Count - 1].transform, to.transform);
                circles[layer_].AddRange(lastCircleNodes);

                break;
            } else {
                LocationNode fromNode = null, toNode = null;

                Debug.Log("Checking floor " + layer_ + ", " + (toFloor > currentFloor));

                if (toFloor > currentFloor) {
                    if (toFloor == layer_) {
                        //At the to floor, set as staircase and destination
                        fromNode = layer.getClosestDownStaircase(lastStaircaseTaken.transform.position);
                        toNode = to;
                    } else if (layer_ == currentFloor) {
                        //At the current floor, set as from and closest staircase
                        fromNode = from;
                        toNode = layer.getClosestUpStaircase(to.transform.position);
                        lastStaircaseTaken = toNode;
                    } else if (layer_ > currentFloor && layer_ < toFloor) { 
                        //In between the floors, set as staircases
                        fromNode = layer.getClosestDownStaircase(lastStaircaseTaken.transform.position);
                        toNode = layer.getClosestUpStaircase(fromNode.transform.position);
                        lastStaircaseTaken = toNode;
                    }
                } else {
                    //same below
                    if (toFloor == layer_) {
                        //At the to floor, set as destination and closest staircase
                        toNode = to;
                        fromNode = layer.getClosestUpStaircase(toNode.transform.position);

                        lastStaircaseTaken = fromNode;

                    } else if (layer_ == currentFloor) {
                        //At the current floor, set as closest staircase and from
                        toNode = layer.getClosestDownStaircase(lastStaircaseTaken.transform.position);
                        fromNode = from;
                    } else if (layer_ < currentFloor && layer_ > toFloor) {
                        //In between the floors, set as staircases
                        fromNode = layer.getClosestDownStaircase(lastStaircaseTaken.transform.position);
                        toNode = layer.getClosestUpStaircase(fromNode.transform.position);
                        lastStaircaseTaken = toNode;
                    }
                }

                if (fromNode == null || toNode == null) continue;


                //Generate a path between the fromNode and the toNode
                List<LocationNode> pathNodes = layer.getShortestDistancePathFromLocToLoc(fromNode.transform.position, toNode.transform.position);

                //Generate circles between the fromNode and to the first path node
                List<GameObject> firstCircleNodes = generateCirclesBetweenTwoPathNodes(fromNode.transform, pathNodes[0].transform);
                circles[layer_].AddRange(firstCircleNodes);

                for (int i = 0; i < pathNodes.Count - 1; i++) {
                    List<GameObject> circleNodes = generateCirclesBetweenTwoPathNodes(pathNodes[i].transform, pathNodes[i + 1].transform);
                    circles[layer_].AddRange(circleNodes);
                }

                //Generate circles between the last node and the destination
                List<GameObject> lastCircleNodes = generateCirclesBetweenTwoPathNodes(pathNodes[pathNodes.Count - 1].transform, toNode.transform);
                circles[layer_].AddRange(lastCircleNodes);
            }
        }

        setCircleLayerActive(TouchManager.Instance.currentLayer());
    }

    public void setCircleLayerActive(int layer) {
        //Can't do circles[layer] since circles may be empty.
        for (int i = 0; i < circles.Count; i++) {
            if (i == layer) {
                for (int j = 0; j < circles[i].Count; j++) {
                    circles[i][j].SetActive(true);
                }
            } else {
                for (int j = 0; j < circles[i].Count; j++) {
                    circles[i][j].SetActive(false);
                }
            }
        }
    }

    public void stopNavigating() {
        onDirection = -1;
        currentlyNavigating = false;
        currentDirections = new string[0];
        distances = new float[0];

        //Destory all circles
        for (int i = 0; i < circles.Count; i++) {
            for (int j = 0; j < circles[i].Count; j++) {
                Destroy(circles[i][j]);
            }
        }
        circles = new List<List<GameObject>>();

        SearchManager.Instance.OnPointerDown(null);

        StartCoroutine(setInactiveAndMoveLevelControllerDown(0.2f));
    }

    IEnumerator setInactiveAndMoveLevelControllerDown(float time) {
        yield return new WaitForSeconds(time);
        upDownObj.GetComponent<UpDownController>().setY(false);
        this.gameObject.SetActive(false);
    }
    
    [Header("Used to figure out how many circles should be generated per distance. 0.1f is very thick, 4 is very sparce.")]
    [Range(0.1f, 4f)]
    public float circleMultiplier = 2f;

    public List<GameObject> generateCirclesBetweenTwoPathNodes(Transform pathParent, Transform to) {
        float xDifference = to.position.x - pathParent.position.x;
        float zDifference = to.position.z - pathParent.position.z;

        int count = (int) Mathf.Floor(Vector3.Distance(pathParent.position, to.position) / (circlePrefab.transform.localScale.x * circleMultiplier));

        if (count < 2) count = 2;

        float xIncrement = xDifference / count;
        float zIncrement = zDifference / count;

        List<GameObject> circles = new List<GameObject>();
        
        for (int i = 0; i < count; i++) {
            GameObject circle = Instantiate(circlePrefab);
            circle.transform.position = new Vector3(pathParent.position.x + xIncrement * i, pathParent.position.y, pathParent.position.z + zIncrement * i);
            circle.transform.SetParent(pathParent);
            circles.Add(circle);
        }

        return circles;
    }
}
