using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance;

    public string API_LINK = "https://test-bay-api.jaidenwithani.repl.co/api";

    private List<Room> rooms = new List<Room>();
    private List<Teacher> teachers = new List<Teacher>();

    IEnumerator Start()
    {
        Instance = this;

        CoroutineData<UnityWebRequestAsyncOperation> request = new CoroutineData<UnityWebRequestAsyncOperation>(
            this, 
            GetRequest( getPath("/rooms") )
        );

        yield return request.coroutine;
        
        UnityWebRequestAsyncOperation result = request.result;

        if (result.webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + result.webRequest.error);
        }

        string json = result.webRequest.downloadHandler.text;

        JSONRoom.RoomContainer roomContainer = JsonUtility.FromJson<JSONRoom.RoomContainer>(json);

        foreach (JSONRoom j_room in roomContainer.rooms) {
            rooms.Add(j_room.toRoom(Room.toRoomType(j_room.type)));
        }

        Debug.Log("Finished loading rooms - loaded " + rooms.Count + " rooms.");

        request = new CoroutineData<UnityWebRequestAsyncOperation>(
            this, 
            GetRequest( getPath("/teachers") )
        );

        yield return request.coroutine;
        
        result = request.result;

        if (result.webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + result.webRequest.error);
        }

        json = result.webRequest.downloadHandler.text;

        Teacher.TeacherContainer teacherContainer = JsonUtility.FromJson<Teacher.TeacherContainer>(json);

        foreach (Teacher teacher in teacherContainer.teachers) {
            teachers.Add(teacher);
        }

        Debug.Log("Finished loading teachers - loaded " + teachers.Count + " teachers.");

        Debug.Log("Loading offices....");

        foreach (Teacher teacher in teachers) {
            if (teacher.office != null) {
                Room room = GetRoomByOffice(teacher.office);
                if (false /*room != null*/) {
                    room.addTeacher(teacher);
                } else {
                    try {
                        string[] owners = new string[] { "" };
                        owners[0] = teacher.full_name;
                        Room newRoom = new Room(teacher.office, Room.RoomType.OFFICE, teacher.full_name + "'s Office", owners);
                        rooms.Add(newRoom);
                    } catch (System.Exception e) {
                        Debug.LogWarning(e);
                        Debug.Log(teacher.full_name + " has an invalid office error thing *shrug*");
                    }
                }
            }
        }

        Debug.Log("Final Room List has a count of " + rooms.Count + " rooms.");
    }

    public List<Room> GetRooms() {
        return this.rooms;
    }

    public List<Teacher> GetTeachers() {
        return this.teachers;
    }

    public Room GetRoomByName(string name) {
        for (int i = 0; i < rooms.Count; i++) {
            if (rooms[i].getName() == name) return rooms[i];
        }
        return null;
    }

    public Room GetRoomByOffice(float office) {
        for (int i = 0; i < rooms.Count; i++) {
            if (rooms[i].getNumber() == office) return rooms[i];
        }
        return null;
    }

    public List<Room> GetRoomsOnFloor(int floor) {
        List<Room> roomOnFloor = new List<Room>();

        for (int i = 0; i < rooms.Count; i++) {
            if (Mathf.Floor(rooms[i].getNumber() / 100) == floor) roomOnFloor.Add(rooms[i]);
        }
        
        return roomOnFloor;
    }

    public string getPath(string path) {
        return API_LINK + (path.StartsWith("/") ? path : "/" + path);
    }

    IEnumerator GetRequest(string uri) {
        UnityWebRequest request = UnityWebRequest.Get(uri);

        yield return request.SendWebRequest();
    }
    
}
