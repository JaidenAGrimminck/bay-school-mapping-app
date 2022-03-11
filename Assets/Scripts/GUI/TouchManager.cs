using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchManager : MonoBehaviour
{
    public GameObject cam;

    [Range(0f,10f)]
    public float swipeSpeed = 1f;

    [Range(0f,10f)]
    public float minSwipeDistance = 1f;

    public float minZoom = 1f;
    
    public float maxZoom = 40f;

    public DirectionsController dc;

    private Vector2 lastTouchPosition = Vector2.zero;

    private Vector2[] lastDoubleTouchPositions = new Vector2[] {
        Vector2.zero,
        Vector2.zero
    };

    public List<Layer> layers = new List<Layer>();

    public static TouchManager Instance;

    void Start() {
        Instance = this;

        for (int i = 0; i < layers.Count; i++) {
            layers[i].gameObject.SetActive(true);
        }
        
        updateLayers();
    }

    void Update()
    {

        if (MenuManager.Instance.getMenu() != 1) return;

        switch (Input.touchCount) {
            case 1:
                if (Input.GetTouch(0).phase == TouchPhase.Moved) {
                    Vector2 touchPos = Input.GetTouch(0).deltaPosition;
            
                    if (lastTouchPosition == Vector2.zero) {
                        lastTouchPosition = touchPos;
                        break;
                    }

                    Vector3 movement = new Vector3(-touchPos.x, 0, -touchPos.y);

                    cam.transform.position += movement * swipeSpeed * Time.deltaTime;

                } else if (Input.GetTouch(0).phase == TouchPhase.Ended) {
                    lastTouchPosition = Vector2.zero;
                }
                break;
            
            case 2:
                if (Input.GetTouch(1).phase == TouchPhase.Began) {
                    lastDoubleTouchPositions[0] = Input.GetTouch(0).position;
                    lastDoubleTouchPositions[1] = Input.GetTouch(1).position;
                } else if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved) {
                    Vector2[] touchPos = {
                        Input.GetTouch(0).position,
                        Input.GetTouch(1).position
                    };

                    float distanceBetween = Vector2.Distance(touchPos[0], touchPos[1]);
                    float oldDistance = Vector2.Distance(lastDoubleTouchPositions[0], lastDoubleTouchPositions[1]);

                    float distanceDifference = Math.Abs(distanceBetween - oldDistance);

                    if (distanceBetween > oldDistance) {
                        cam.transform.position -= new Vector3(0, distanceDifference, 0) * Time.deltaTime;
                    } else {
                        cam.transform.position += new Vector3(0, distanceDifference, 0) * Time.deltaTime;
                    }

                    if (cam.transform.position.y > maxZoom) cam.transform.position = new Vector3(cam.transform.position.x, maxZoom, cam.transform.position.z);
                    if (cam.transform.position.y < minZoom) cam.transform.position = new Vector3(cam.transform.position.x, minZoom, cam.transform.position.z);

                    lastDoubleTouchPositions[0] = Input.GetTouch(0).position;
                    lastDoubleTouchPositions[1] = Input.GetTouch(1).position;
                } else if (Input.GetTouch(1).phase == TouchPhase.Ended) {
                    if (Input.GetTouch(0).phase != TouchPhase.Ended) lastTouchPosition = Input.GetTouch(0).deltaPosition;

                    lastDoubleTouchPositions = new Vector2[] {
                        Vector2.zero,
                        Vector2.zero
                    };
                }

                break;
            default:
                break;
        }
    }

    private int layer = 1;

    public int currentLayer() {
        return layer;
    }

    public void goDownLayer() {
        layer--;
        if (layer < 0) layer = 0;
        updateLayers();
    }

    public void goUpLayer() {
        layer++;
        if (layer >= layers.Count) layer = layers.Count - 1;
        updateLayers();
    }

    public void setLayer(int layer) {
        this.layer = layer;
        if (layer >= layers.Count) layer = layers.Count - 1;
        if (layer < 0) layer = 0;
        updateLayers();
    }

    void updateLayers() {
        foreach (Layer layer in layers) {
            layer.gameObject.SetActive(false);
        }
        layers[layer].gameObject.SetActive(true);

        dc.setCircleLayerActive(layer);
    }
}
