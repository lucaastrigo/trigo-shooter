using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    Button button;

    void Start()
    {
        button = GetComponent<Button>();

        button.Select();
    }
}
