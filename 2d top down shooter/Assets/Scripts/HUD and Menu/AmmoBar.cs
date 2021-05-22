using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AmmoBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxAmmo(int health)
    {
        slider.maxValue = health;
    }

    public void SetAmmo(int health)
    {
        slider.value = health;
    }
}
