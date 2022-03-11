using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationNode : MonoBehaviour
{
    public static List<LocationNode> Locations = new List<LocationNode>();

    public LocationManager.Location location = LocationManager.Location.Empty();

    public float roomNumber;

    public bool isAnchor = false;

    public bool isStaircase = false;

    public bool isPathNode = false;

    public List<LocationNode> adjacentPathNodes = new List<LocationNode>();

    public float metersFromStaircase = 0;

    public void OnEnable() {
        bool inList = false;

        for (int i = 0; i < Locations.Count; i++) {
            if (Locations[i].roomNumber == this.roomNumber) {
                inList = true;
            }
        }

        if (!inList) {
            Locations.Add(this);
        }
        
    }

    public void Update() {
        LocationManager.Location currentLoc = LocationManager.Instance.currentLocation;
    }

    
}
