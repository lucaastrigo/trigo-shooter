using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SkillObject : MonoBehaviour
{
    public string skillName;
    public bool activeSkill;
    [HideInInspector] public bool canUse;
    public GameObject skillTitle, description, skillFX;

    bool onTrigger = false;
    GameObject[] allObjects;
    GameObject skillStorage;
    GameObject oldSkill;
    GameObject player;
    Light2D light2D;

    void Start()
    {
        allObjects = GameObject.FindGameObjectsWithTag("Skill Object");

        skillTitle.GetComponent<TextMeshProUGUI>().text = skillName;

        light2D = GetComponent<Light2D>();
        light2D.GetComponent<Light2D>().color = skillTitle.GetComponent<TextMeshProUGUI>().color;
    }

    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");

        skillTitle.SetActive(onTrigger);
        description.SetActive(onTrigger);
        light2D.enabled = onTrigger;

        if (onTrigger)
        {
            skillTitle.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ActivateSkill();
            }
        }
    }

    void ActivateSkill()
    {
        Instantiate(skillFX, transform.position, Quaternion.identity);

        if (activeSkill)
        {
            for (int i = 0; i < skillStorage.GetComponent<SkillStorage>().skills.Length; i++)
            {
                if (skillStorage.GetComponent<SkillStorage>().skills[i].GetComponent<Skill>().activeSkill && skillStorage.GetComponent<SkillStorage>().skills[i].GetComponent<Skill>().skillOn)
                {
                    GameObject oldSkill = skillStorage.GetComponent<SkillStorage>().skillObjects[i];

                    Instantiate(oldSkill, transform.position, Quaternion.identity);

                    skillStorage.GetComponent<SkillStorage>().skills[i].GetComponent<Skill>().skillOn = false;
                }
            }

            Destroy(gameObject);
        }
        else
        {
            for (int i = 0; i <allObjects.Length; i++)
            {
                if(allObjects[i].transform.parent != null && allObjects[i].transform.parent == transform.parent && allObjects[i].GetComponentInParent<ChestSkill>() != null)
                {
                    Destroy(allObjects[i]);
                }
            }

            Destroy(gameObject);
        }

        GameObject skill = GameObject.Find(skillName);

        if (skill.GetComponent<Skill>() != null)
        {
            skill.GetComponent<Skill>().skillOn = true;
        }

        /*
        player.GetComponent<Player>().aaplace[0].GetComponent<Image>().enabled = true;
        player.GetComponent<Player>().aaplace[1].GetComponent<Image>().enabled = true;
        */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onTrigger = false;
        }
    }
}
