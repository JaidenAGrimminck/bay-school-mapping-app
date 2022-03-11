using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public GameObject cam;
    void Update()
    {
        transform.position = cam.transform.position;
    }
}
