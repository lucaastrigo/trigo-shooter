using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float cameraSize;

    float shakeTime;
    float smoothTime = 0.2f;
    Vector3 target, mousePos, refVel, orgPos;
    GameObject player;

    void Start()
    {
        GetComponent<Camera>().orthographicSize = cameraSize;
    }

    private void FixedUpdate()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            mousePos = CaptureMousePos();
            target = UpdateTargetPos();
            UpdateCameraPosition();
        }
    }

    private void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    Vector3 CaptureMousePos()
    {
        Vector2 ret = Camera.main.ScreenToViewportPoint(Input.mousePosition); 
        ret *= 2;
        ret -= Vector2.one; 
        float max = 0.9f;
        if (Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max)
        {
            ret = ret.normalized;
        }
        return ret;
    }

    Vector3 UpdateTargetPos()
    {
        Vector3 mouseOffset = mousePos; //multiply with 'cameraDist' to increase sensibility
        Vector3 ret = player.transform.position + mouseOffset; 
        ret.z = transform.position.z;  
        return ret;
    }

    void UpdateCameraPosition()
    {
        Vector3 tempPos;
        tempPos = Vector3.SmoothDamp(transform.position, target, ref refVel, smoothTime);     
        transform.position = tempPos; 
    }

    public void Shake(float magnitude, float time)
    {
        shakeTime = time;
        orgPos = transform.localPosition;

        do
        {
            float offset = Random.Range(-1.0f, 1.0f) * magnitude;
            orgPos.x += offset;
            transform.localPosition = orgPos;
            shakeTime -= Time.deltaTime;

        } while (shakeTime > 0);

        transform.localPosition = orgPos;
    }

    public void StopShake()
    {
        shakeTime = 0;
    }
}
