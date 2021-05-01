using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSkill : MonoBehaviour
{
    public bool activeSkillChest;
    public Sprite openedSprite;
    public GameObject openFX;
    public GameObject[] skillObjects;
    public Vector2 offset;

    bool open;
    SpriteRenderer sprite;
    GameObject skillStorage;
    GameObject s1, s2, s3;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
    }

    private void Update()
    {
        for (int i = 0; i < skillStorage.GetComponent<SkillStorage>().skills.Length; i++)
        {
            for (int j = 0; j < skillObjects.Length; j++)
            {
                if (skillStorage.GetComponent<SkillStorage>().skills[i].name == skillObjects[j].GetComponent<SkillObject>().skillName)
                {
                    if (!skillStorage.GetComponent<SkillStorage>().skills[i].GetComponent<Skill>().skillOn)
                    {
                        skillObjects[j].GetComponent<SkillObject>().canUse = true;
                    }
                }
            }
        }
    }

    GameObject RandomizeSkill()
    {
        GameObject randomizedSkill = skillObjects[Random.Range(0, skillObjects.Length)];

        if (!randomizedSkill.GetComponent<SkillObject>().canUse)
        {
            RandomizeSkill();
        }

        return randomizedSkill;
    }

    void CalculateSkill()
    {
        if (activeSkillChest)
        {
            s1 = RandomizeSkill();
        }
        {
            s1 = RandomizeSkill();
            s2 = RandomizeSkill();
            s3 = RandomizeSkill();

            Calculate();
        }
    }

    void Calculate()
    {
        if (!activeSkillChest)
        {
            if (s1 == s2 || s1 == s3 || s2 == s3)
            {
                CalculateSkill();
            }
        }
    }

    void Drop()
    {
        if (!activeSkillChest)
        {
            Instantiate(s1, new Vector2(transform.position.x, transform.position.y + offset.y), Quaternion.identity);
            Instantiate(s2, new Vector2(transform.position.x - offset.x, transform.position.y + offset.y), Quaternion.identity);
            Instantiate(s3, new Vector2(transform.position.x + offset.x, transform.position.y + offset.y), Quaternion.identity);
            Instantiate(openFX, transform.position, Quaternion.identity);
            open = true;
            sprite.sprite = openedSprite;
        }
        else
        {
            Instantiate(s1, new Vector2(transform.position.x, transform.position.y + offset.y), Quaternion.identity);
            Instantiate(openFX, transform.position, Quaternion.identity);
            open = true;
            sprite.sprite = openedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !open)
        {
            CalculateSkill();
            Drop();
        }
    }
}
