using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    float currentSize, minSize, maxSize;
    Camera cam;
    Vector3 dragOrigin;

    void Start()
    {
        cam = GetComponent<Camera>();

        minSize = cam.orthographicSize - 30;
        maxSize = cam.orthographicSize + 40;
    }

    void Update()
    {
        PanCamera();
        Zoom();
    }

    void PanCamera()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position += difference;
        }
    }

    void Zoom()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && cam.orthographicSize > minSize)
        {
            cam.orthographicSize -= 5f;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0 && cam.orthographicSize < maxSize)
        {
            cam.orthographicSize += 5f;
        }
    }
}
