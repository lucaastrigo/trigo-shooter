using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSkill : MonoBehaviour
{
    public bool activeSkillChest;
    public Sprite openedSprite;
    public GameObject[] skillObjects;
    public Vector2 offset;

    bool open;
    SpriteRenderer sprite;
    GameObject skillStorage;

    [HideInInspector] public GameObject randomizedSkill, s1, s2, s3;

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
        randomizedSkill = skillObjects[Random.Range(0, skillObjects.Length)];

        if (!randomizedSkill.GetComponent<SkillObject>().canUse)
        {
             RandomizeSkill();
        }
        else
        {
            return randomizedSkill;
        }

        return randomizedSkill;
    }

    public void CalculateSkill()
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
            GameObject child1 = Instantiate(s1, new Vector2(transform.position.x, transform.position.y + offset.y), Quaternion.identity);
            GameObject child2 = Instantiate(s2, new Vector2(transform.position.x - offset.x, transform.position.y + offset.y), Quaternion.identity);
            GameObject child3 = Instantiate(s3, new Vector2(transform.position.x + offset.x, transform.position.y + offset.y), Quaternion.identity);

            child1.transform.parent = transform;
            child2.transform.parent = transform;
            child3.transform.parent = transform;

            Instantiate(Resources.Load("Particle FX/SkillChestFX"), transform.position, Quaternion.identity);
            open = true;
            sprite.sprite = openedSprite;
        }
        else
        {
            GameObject child1 = Instantiate(s1, new Vector2(transform.position.x, transform.position.y + offset.y), Quaternion.identity);

            child1.transform.parent = transform;

            Instantiate(Resources.Load("Particle FX/SkillChestFX"), transform.position, Quaternion.identity);
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
