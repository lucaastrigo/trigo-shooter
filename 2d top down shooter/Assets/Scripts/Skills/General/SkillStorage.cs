using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SkillStorage : MonoBehaviour
{
    public static bool gameOn;
    public static SkillStorage value;

    public bool[] activeSkills;
    public GameObject[] skills;

    [Space]
    [Space]
    [Space]
    [Space]
    [Space]

    public bool[] active;
    public GameObject[] items;

    [Space]
    [Space]
    [Space]
    [Space]
    [Space]

    public float ammoExpert;
    public float bloodlust;
    public float exposed;
    public float fastHands;
    public float reinforcedBarrel;
    public float scavenger;
    public float sharpshooter;
    public float sixthSense = 1;
    public float widerMuzzle;


    /*
    
    >items

        bubble shield - 0
        explosion gift - 1
        freeze - 2
        land mine - 3
        leaf dash - 4
        plunger - 5
        speedster - 6

    >skills

        ghost bullets - 4

    */

    PlayerSkills player;

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

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkills>();
    }

    public void DeactivateItems()
    {
        for (int i = 0; i < activeSkills.Length; i++)
        {
            activeSkills[i] = false;
        }

        for (int i = 0; i < active.Length; i++)
        {
            active[i] = false;
        }



        ammoExpert = 0;
        bloodlust = 0;
        exposed = 0;
        fastHands = 0;
        reinforcedBarrel = 0;
        scavenger = 0;
        sharpshooter = 0;
        sixthSense = 1;
        widerMuzzle = 0;
    }
}
