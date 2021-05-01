using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SkillStorage : MonoBehaviour
{
    public static bool gameOn;
    public static SkillStorage value;

    public Skill[] skills;
    public GameObject[] skillObjects;

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
