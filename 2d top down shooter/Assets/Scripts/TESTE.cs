using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

public class TESTE : MonoBehaviour
{
    Volume volume;

    void Start()
    {
        volume = GetComponent<Volume>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            volume.enabled = !volume.enabled;
        }
    }
}
