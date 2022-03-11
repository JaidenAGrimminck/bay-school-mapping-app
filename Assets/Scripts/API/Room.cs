using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    private float roomNumber = 0;
    private RoomType type;
    private string name;
    private List<string> owners;

    public Room(float roomNumber, RoomType type, string name, string[] owners) {
        this.roomNumber = roomNumber;
        this.type = type;
        this.name = name;
        this.owners = new List<string>(owners);
    }

    public float getNumber() {
        return this.roomNumber;
    }
    
    public RoomType getType() {
        return this.type;
    }

    public string getName() {
        return name;
    }

    public void addTeacher(Teacher teacher) {
        this.owners.Add(teacher.full_name);
        
        if (this.name.EndsWith("Office")) {
            string names = "";

            for (int i = 0; i < this.owners.Count; i++) {
                if (i == this.owners.Count - 1) names += "and ";
                names += this.owners[i];
                if (i != this.owners.Count - 1) {
                    names += ", ";
                }
            }

            this.name = names + "'s Office";

            if (this.owners.Count == 2) {
                this.name = this.owners[0] + " and " + this.owners[1] + "'s Office";
            } else if (this.owners.Count == 1) {
                this.name = this.owners[0] + "'s Office";
            }
        }
    }

    public enum RoomType {
        OFFICE,
        CLASSROOM,
        LIBRARY,
        STORAGE,
        LOUNGE,
        CAFETERIA,
        NONE
    }

    public static RoomType toRoomType(string room) {
        room = room.ToLower();
        if (room == "office") {
            return RoomType.OFFICE;
        } else if (room == "classroom") {
            return RoomType.CLASSROOM;
        } else if (room == "library") {
            return RoomType.LIBRARY;
        } else if (room == "storage") {
            return RoomType.STORAGE;
        } else if (room == "cafeteria") {
            return RoomType.CAFETERIA;
        }
        
        return RoomType.NONE;
    }

    public static int getFloorFromRoomNumber(float roomNumber) {
        string room = roomNumber.ToString();

        if (room.StartsWith("50")) {
            return 50; //50 will be project center;
        }

        if (room.Contains(".")) {
            room = room.Split('.')[0];
        }

        if (room.Length == 2) {
            return 0;
        }

        return int.Parse(room.Substring(0, 1));
    }
}
