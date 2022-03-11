using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance = null;

    public Location currentLocation = Location.Empty();

    public double meterToXAndZ = 0.0f;

    public float minutePerMeter = 0;

    public List<double> scales = new List<double>();

    [HideInInspector]
    public double scale;

    public double getScaleForFloor(int floor) {
        if (floor == 50) return scales[5];
        return scales[floor];
    }


    bool isRemote = false;

    [HideInInspector]
    public bool locationAllowed = false;

    [HideInInspector]
    public bool onCampus = false;

    public List<LocationNode> anchors = new List<LocationNode>();

    public void Awake() {
        scale = meterToXAndZ;
        LocationManager.Instance = GetComponent<LocationManager>();
    }

    //TODO: Temporary file thing in order to figure out location? idk

    IEnumerator Start()
    {
        yield return new WaitForSeconds(5);

        #if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isRemoteConnected) {
                isRemote = true;
            }
        #endif

        Debug.Log("Is remote: " + isRemote);

        Debug.Log("Size of location nodes: " + LocationNode.Locations.Count);

        if (isRemote) {
            yield return new WaitForSeconds(2);
        }

        if (!Input.location.isEnabledByUser) {
            Debug.Log("Location Services not Enabled.");

            if (SystemInfo.deviceType != DeviceType.Handheld && !isRemote) {
                Debug.Log("Yielded due to running on non-handheld device. TYPE: " + SystemInfo.deviceType);
                yield break;
            }
        }

        Input.location.Start(1, 1);

        locationAllowed = true;

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed) {
            Debug.Log("Unable to determine device location");
            locationAllowed = false;
            tracking = false;
            yield break;
        } else {
            locationAllowed = true;
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }
    }

    bool tracking = true;

    Location BaySchoolLocation = new Location(37.8006476, -122.4560279, 0,0);
    const double radius = 200;

    public void Update() {
        if (!tracking) return;

        if (Input.location.status == LocationServiceStatus.Failed) {
            Debug.Log("Unable to determine device location, location failed.");
            tracking = false;
            locationAllowed = false;
            return;
        }

        if (Input.location.status == LocationServiceStatus.Running) {
            double latitude = Input.location.lastData.latitude;
            double longitude = Input.location.lastData.longitude;

            double distance;

            distanceInMeters(new Location(latitude, longitude, 0, 0), BaySchoolLocation, out distance);

            onCampus = distance < radius;

            // Debug.Log("Current Accuracy: " + Input.location.lastData.horizontalAccuracy);

            // //Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            // double latitude = Input.location.lastData.latitude;
            // double longitude = Input.location.lastData.longitude;
            // double altitude = Input.location.lastData.altitude;
            // currentLocation = new Location(latitude, longitude, altitude, 0);

            // int closestAnchorIndex = 0;
            // double lowestValue = 999999999999999999D;
            // int i = 0;
            // foreach (LocationNode loc in LocationNode.Locations)
            // {
            //     double v;

            //     distanceInMeters(currentLocation, loc.location, out v);

            //     if (v < lowestValue) {
            //         lowestValue = v;
            //         closestAnchorIndex = i;
            //     }

            //     i++;
            // }

            // Debug.Log("Closest anchor: " + LocationNode.Locations[closestAnchorIndex].gameObject.name + ", Distance: ~" + lowestValue);
        }
    }

    [System.Serializable]
    public struct Location {
        public double latitude;
        public double longitude;
        
        public double elevation;

        public int timestamp;

        public Location(double latitude, double longitude, double elevation, int timestamp) {
            this.latitude = latitude;
            this.longitude = longitude;
            this.elevation = elevation;
            this.timestamp = timestamp;
        }

        public static Location Empty() {
            return new Location(0,0,0,0);
        }
    }

    // Get distance in meters between two latitudes and longitudes
    public static void distanceInMeters(LocationManager.Location loc1, LocationManager.Location loc2, out double distance)
    {
        double latitude = loc1.latitude;
        double longitude = loc1.longitude;
        double latitude2 = loc2.latitude;
        double longitude2 = loc2.longitude;

        // LMAOOO thank you github copilot for getting the answer from here https://stackoverflow.com/questions/365826/calculate-distance-between-2-gps-coordinates
        double R = 6371000; // metres
        double φ1 = latitude * Mathf.Deg2Rad;
        double φ2 = latitude2 * Mathf.Deg2Rad;
        double Δφ = (latitude2 - latitude) * Mathf.Deg2Rad;
        double Δλ = (longitude2 - longitude) * Mathf.Deg2Rad;

        double a = Math.Sin(Δφ / 2f) * Math.Sin(Δφ / 2f) +
            Math.Cos(φ1) * Math.Cos(φ2) *
            Math.Sin(Δλ / 2f) * Math.Sin(Δλ / 2f);
        double c = 2f * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        distance = R * c;
    }

    public static void metersToFeet(double meters, out double feet) {
        feet = meters * 3.28084;
    }

    public static void feetToMeters(double feet, out double meters) {
        meters = feet / 3.28084;
    }
}
