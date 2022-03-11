using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JSONRoom 
{
    public float room;
    public string[] owners;

    public string name;

    public string type;

    public Room toRoom(Room.RoomType type) {
        return new Room(room, type, name == null ? "Room " + room : name, owners);
    }

    public class RoomContainer {
        public JSONRoom[] rooms;
    }
}