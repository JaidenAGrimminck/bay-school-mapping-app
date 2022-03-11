using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float speed = 5f;

    int xMovement = 0;
    int yMovement = 0;

    void Update() {
        Vector3 Movement = new Vector3(xMovement, 0, yMovement);

        transform.position += Movement * speed * Time.deltaTime;
    }

    public void setMoving(int x, int y) {
        xMovement += x;
        xMovement += y;
    }
}
