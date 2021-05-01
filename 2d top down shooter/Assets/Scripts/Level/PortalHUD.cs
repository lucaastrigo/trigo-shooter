using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PortalHUD : MonoBehaviour
{
    public Slider slider;

    public void SetMaxTime(float time)
    {
        slider.maxValue = time;
    }

    public void SetTime(float time)
    {
        slider.value = time;
    }
}
