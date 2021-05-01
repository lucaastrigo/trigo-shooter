﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public int health; 
    public int currentHealth;
    [HideInInspector] public GameObject healthBar;
    public TextMeshProUGUI playerIndicator;

    [HideInInspector] public float time;
    [HideInInspector] public float indicationTime = 1f;
    GameObject healthText;
    GameObject valueStorage, skillStorage;
    [HideInInspector] public GameObject aaplace, aatimer;
    Rigidbody2D rb;
    Animator anim;

    private void Start()
    {
        currentHealth = ValueStorage.value.healthValue;
        health = ValueStorage.value.maxHealthValue;

        healthBar = GameObject.Find("Health Bar");
        healthBar.GetComponent<HealthBar>().SetHealth(ValueStorage.value.healthValue);
        healthBar.GetComponent<HealthBar>().SetMaxHealth(ValueStorage.value.maxHealthValue);

        healthText = GameObject.Find("Health Text");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        aaplace = GameObject.FindGameObjectWithTag("AA");
        aatimer = GameObject.FindGameObjectWithTag("AAT");
    }

    private void Update()
    {
        if(valueStorage == null)
        {
            valueStorage = GameObject.FindGameObjectWithTag("Value Storage");
        }

        if(skillStorage == null)
        {
            skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
        }

        ValueStorage.value.healthValue = currentHealth;
        ValueStorage.value.maxHealthValue = health;

        healthText.GetComponent<TMP_Text>().text = currentHealth.ToString() + "/" + health.ToString();

        if(currentHealth >= health)
        {
            currentHealth = health;
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        playerIndicator.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));

        if (time <= 0)
        {
            playerIndicator.enabled = false;
        }
        else
        {
            playerIndicator.enabled = true;
            time -= Time.deltaTime;
        }

        for (int i = 0; i < skillStorage.GetComponent<SkillStorage>().skills.Length; i++)
        {
            if (skillStorage.GetComponent<SkillStorage>().skills[i].GetComponent<Skill>().activeSkill && skillStorage.GetComponent<SkillStorage>().skills[i].GetComponent<Skill>().skillOn)
            {
                aaplace.GetComponent<Image>().sprite = skillStorage.GetComponent<SkillStorage>().skillObjects[i].GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    public void MoreHealth(int healthAmount)
    {
        time = indicationTime;

        currentHealth += healthAmount;

        healthBar.GetComponent<HealthBar>().SetHealth(currentHealth);

        if(currentHealth >= health)
        {
            playerIndicator.GetComponent<TMP_Text>().text = "full health";
        }
        else
        {
            playerIndicator.GetComponent<TMP_Text>().text = "+ " + healthAmount.ToString() + " health";
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.GetComponent<HealthBar>().SetHealth(currentHealth);

        anim.SetTrigger("hurt");
    }

    public void Die()
    {
        ValueStorage.value.healthValue = 10;
        ValueStorage.value.maxHealthValue = 10;
        ValueStorage.value.weaponValue = "PISTOL";

        for (int i = 0; i <= valueStorage.GetComponent<ValueStorage>().WeaponAmmo.Count - 1; i++)
        {
            ValueStorage.value.WeaponAmmo[i] = 1000;
        }

        for (int i = valueStorage.GetComponent<ValueStorage>().WeaponAmmo.Count - 1; i > 0; i--)
        {
            ValueStorage.value.WeaponAmmo.RemoveAt(i);
        }

        //reset skills
        for (int i = 0; i < SkillStorage.value.skills.Length; i++)
        {
            SkillStorage.value.skills[i].skillOn = false;
            SkillStorage.value.skills[i].skilled = false;
            SkillStorage.value.skills[i].unskilled = false;
        }

        SceneManager.LoadScene("Lobby");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Room"))
        {
            if(collision.GetComponent<Room>().thisRoom == Room.roomType.wave && !collision.GetComponent<Room>().spawned)
            {
                collision.GetComponent<Room>().numOfWaves = Random.Range(1, 4);

                if (collision.GetComponent<Room>().numOfWaves > 0)
                {
                    for (int i = 0; i < collision.GetComponent<Room>().door.Length; i++)
                    {
                        collision.GetComponent<Room>().door[i].SetActive(true);
                    }

                    collision.GetComponent<Room>().SpawnWave();
                }
            }

            collision.GetComponent<Room>().opened = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Portal"))
        {
            time = 0;
            playerIndicator.GetComponent<TMP_Text>().text = null;
        }
    }
}