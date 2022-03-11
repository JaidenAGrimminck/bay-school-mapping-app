using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Teacher
{
    public string full_name;
    public string[] subjects;
    public float office;

    public class TeacherContainer {
        public List<Teacher> teachers;
    }
}
