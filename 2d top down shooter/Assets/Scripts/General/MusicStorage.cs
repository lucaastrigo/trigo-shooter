using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStorage : MonoBehaviour
{
    public static MusicStorage value;

    private void Awake()
    {
        if (value == null)
        {
            value = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (value != this)
        {
            Destroy(gameObject);
        }
    }
}
