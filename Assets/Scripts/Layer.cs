using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    public static List<Layer> layers = new List<Layer>();

    [Header("The general outline of the buildings, each one being a corner.")]
    public LocationManager.Location[] outlineNodes = new LocationManager.Location[4] { LocationManager.Location.Empty(), LocationManager.Location.Empty(), LocationManager.Location.Empty(), LocationManager.Location.Empty() };

    [Header("Just a general list of all the nodes in the floor.")]
    public List<LocationNode> nodes = new List<LocationNode>();

    [Header("All children of this object will be added to the list above.")]
    public GameObject nodesParent;

    [Header("Parent of the pathfinding nodes.")]
    public GameObject pathParent;

    [Header("This are all of the staircase beginning, all of the staircases that lead out of this floor, up.")]
    public List<LocationNode> staircaseBeginnings = new List<LocationNode>();

    [Header("This are all of the staircase endings, all of the staircases that lead up to this floor.")]
    public List<LocationNode> staircaseEndings = new List<LocationNode>();

    public string floorName = "nth Floor";

    public int floorNumber = 0;
    int savedNumber = 0;


    public void OnEnable() {
        savedNumber = floorNumber;

        if (layers.Contains(this)) {
            return;
        }

        layers.Add(this);

        foreach (Transform child in nodesParent.transform) {
            nodes.Add(child.GetComponent<LocationNode>());
        }
    }

    void Update() {
        if (this.floorNumber == 69) {
            Debug.Log("testing shortest distance.");
            this.floorNumber = savedNumber;

            LocationNode dwnsc = getClosestDownStaircase(getNodeByNumber(335).transform.position);

            Debug.Log(dwnsc.gameObject.name);
            
            // List<LocationNode> nodes = getShortestDistancePathFromLocToLoc(getNodeByNumber(146).transform.position, getNodeByNumber(107).transform.position);
            // Debug.Log("len: " + nodes.Count);
            // foreach (LocationNode node in nodes) {
            //     Debug.Log(node.transform.position + " - " + node.gameObject.name);
            // }
        }
    }

    public static Layer getLayerFromFloor(int level) {
        for (int i = 0; i < TouchManager.Instance.layers.Count; i++) {
            if (TouchManager.Instance.layers[i].floorNumber == level) {
                return TouchManager.Instance.layers[i];
            }
        }
        return null;
    }

    public LocationNode getClosestUpStaircase(Vector3 location) {
        int idx = -1;
        float lowestDistance = 999999999;
        for (int i = 0; i < this.staircaseBeginnings.Count; i++) {
            if (Vector3.Distance(location, this.staircaseBeginnings[i].transform.position) < lowestDistance) {
                idx = i;
                lowestDistance = Vector3.Distance(location, this.staircaseBeginnings[i].transform.position);
            }
        }
        return this.staircaseBeginnings[idx];
    }

    public LocationNode getClosestDownStaircase(Vector3 location) {
        int idx = -1;
        float lowestDistance = 999999999;
        for (int i = 0; i < this.staircaseEndings.Count; i++) {
            if (Vector3.Distance(location, this.staircaseEndings[i].transform.position) < lowestDistance) {
                idx = i;
                lowestDistance = Vector3.Distance(location, this.staircaseEndings[i].transform.position);
            }
        }
        return this.staircaseEndings[idx];
    }

    LocationNode getClosestNode(Vector3 location) {
        int idx = -1;
        float lowestDistance = 999999999;
        for (int i = 0; i < this.nodesParent.transform.childCount; i++) {
            if (Vector3.Distance(location, this.nodesParent.transform.GetChild(i).position) < lowestDistance) { 
                idx = i;
                lowestDistance = Vector3.Distance(location, this.nodesParent.transform.GetChild(i).position);
            }
        }
        return this.nodes[idx];
    }

    LocationNode getClosestPathNode(Vector3 location) {
        int idx = -1;
        float lowestDistance = 999999999;
        for (int i = 0; i < this.pathParent.transform.childCount; i++) {
            if (Vector3.Distance(location, this.pathParent.transform.GetChild(i).position) < lowestDistance) { 
                idx = i;
                lowestDistance = Vector3.Distance(location, this.pathParent.transform.GetChild(i).position);
            }
        }
        return this.pathParent.transform.GetChild(idx).GetComponent<LocationNode>();
    }

    public List<LocationNode> getShortestDistancePathFromLocToLoc(Vector3 from, Vector3 to) {
        List<LocationNode> path = new List<LocationNode>();

        LocationNode closestFromNode = getClosestNode(from);
        LocationNode closestToNode = getClosestNode(to);
        LocationNode closestPathNode = getClosestPathNode(from);
        LocationNode closestPathNodeTo = getClosestPathNode(to);

        path.Add(closestPathNode);

        LocationNode lastNode = closestPathNode;

        int count = 0;
        
        while (path[path.Count - 1] != closestPathNodeTo && count < 100) {
            int indx = -1;
            float lowestDistance = 9999999999;
            for (int i = 0; i < lastNode.adjacentPathNodes.Count; i++) {
                LocationNode current = lastNode.adjacentPathNodes[i];

                if (path.Contains(current)) continue;

                if (Vector3.Distance(closestPathNodeTo.transform.position, current.transform.position) < lowestDistance) {
                    indx = i;
                    lowestDistance = Vector3.Distance(closestPathNodeTo.transform.position, current.transform.position);
                }
            }

            if (indx == -1) {
                Debug.Log("No path found");
                break;
            }

            lastNode = lastNode.adjacentPathNodes[indx];
            path.Add(lastNode);

            count++;
        }

        return path;
    }

    public LocationNode getNodeByNumber(float roomN) {
        for (int i = 0; i < this.nodesParent.transform.childCount; i++) {
            LocationNode node = this.nodesParent.transform.GetChild(i).GetComponent<LocationNode>();

            if (node.isStaircase || node.isPathNode) continue;
            
            if (node.roomNumber == roomN) {
                return node;
            }
        }
        return null;
    }
}
