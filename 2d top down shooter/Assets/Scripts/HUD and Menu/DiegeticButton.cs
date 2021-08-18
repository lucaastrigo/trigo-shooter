using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class DiegeticButton : MonoBehaviour
{
    public int code, maxCode;
    public Sprite normalImage, selectedImage;

    int currentCode;
    [SerializeField] UnityEvent _event;

    Image image;

    private void Start()
    {
        image = GetComponent<Image>();

        currentCode = maxCode;
    }

    private void Update()
    {
        if(code == currentCode)
        {
            image.sprite = selectedImage;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                _event.Invoke();
            }
        }
        else
        {
            image.sprite = normalImage;
        }

        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if(currentCode < maxCode)
            {
                currentCode++;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentCode > 0)
            {
                currentCode--;
            }
        }
    }
}
