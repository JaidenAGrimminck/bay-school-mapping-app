using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.iOS;

public class NotificationsManager : MonoBehaviour
{
    public string auth;

    void Start() {
        Debug.Log("Notification manager loaded.");
        Debug.Log("NOT REQUESTING AUTHORIZATION");
        //StartCoroutine(RequestAuthorization());
    }

    void afterAuthorization() {
        Debug.Log("Authorization status: " + auth);

        createiOSLocationNotification(
            new Location(37.8007107f, -122.4557998f), // Latitude, Longitude
            250f, //Meter radius
            true, //On entry = true. On exit = false.
            new Notification(
                "You've entered school grounds!", 
                "You're in the school grounds now!", 
                "You are within 250m school grounds."
            )
        );

        createiOSCalendarNotification(
            10, 30,
            new Notification("It is 10:30.", "The notification says it's 10:30", "yea it's 10:30.")
        );

        Debug.Log("Created notifications");
    }

    IEnumerator RequestAuthorization() {
        Debug.Log("Requesting Notifications Access...");

        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;

        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            }

            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            auth = res;
        }

        afterAuthorization();

        yield return auth;
    }

    string createiOSCalendarNotification(int hour, int minute, Notification notif) {
        var timeTrigger = new iOSNotificationCalendarTrigger() 
        {
            Hour = hour,
            Minute = minute,
            Repeats = false
        };

        string id = "_timed_notification_01";

        var notification = new iOSNotification() {
            Identifier = id,
            Title = notif.Title,
            Body = notif.Body,
            Subtitle = notif.Subtitle,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);

        return id;
    }

    //Bay school location:
    //37.8007107,-122.4557998

    string createiOSLocationNotification(Location loc, float radius, bool onEntry, Notification notif) {
        var locationTrigger = new iOSNotificationLocationTrigger()
        {
            Center = new Vector2(loc.longitude, loc.latitude),
            Radius = radius,
            NotifyOnEntry = onEntry,
            NotifyOnExit = !onEntry,
        };

        string id = "_location_notification_02";

        var notification = new iOSNotification()
        {
            Identifier = id,
            Title = notif.Title,
            Body = notif.Body,
            Subtitle = notif.Subtitle,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = locationTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);

        return id;
    }

    struct Location {
        public float latitude;
        public float longitude;

        public Location(float longitude, float latitude) {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }

    struct Notification {
        public string Title;

        public string Body;
        public string Subtitle;

        public Notification(string title, string subtitle, string body) {
            this.Title = title;
            this.Body = body;
            this.Subtitle = subtitle;
        }
    }

}
