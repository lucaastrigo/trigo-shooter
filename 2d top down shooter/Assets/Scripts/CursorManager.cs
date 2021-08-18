using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour 
{
    public bool noCursor;

    public static CursorManager Instance { get; private set; }

    public float frameRate;
    public Texture2D[] sprite;
    public Vector2 offset;

    int frameCount;
    int currentFrame;
    float frameTimer;

    private void Start()
    {
        if (noCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Awake() 
    {
        Instance = this;
    }

    void Update() 
    {
        frameCount = sprite.Length;

        frameTimer -= Time.deltaTime;
        if (frameTimer <= 0f) {
            frameTimer += frameRate;
            currentFrame = (currentFrame + 1) % frameCount;
            Cursor.SetCursor(sprite[currentFrame], offset, CursorMode.Auto);
        }
    }
}
