using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSkill : MonoBehaviour
{
    public bool itemChest;
    public Sprite openedSprite;
    public Vector2 offset;
    public AudioClip clip;

    bool open;
    SpriteRenderer sprite;
    AudioSource aud;

    [HideInInspector] public GameObject randomizedSkill, s1, s2, s3;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        aud = GetComponent<AudioSource>();
    }

    GameObject RandomizeSkill()
    {
        if (itemChest)
        {
            int randomized = Random.Range(0, SkillStorage.value.items.Length);
            randomizedSkill = SkillStorage.value.items[randomized];

            if (SkillStorage.value.active[randomized])
            {
                RandomizeSkill();
            }
            else
            {
                return randomizedSkill;
            }
        }
        else
        {
            int randomized = Random.Range(0, SkillStorage.value.skills.Length);
            randomizedSkill = SkillStorage.value.skills[randomized];

            if (SkillStorage.value.activeSkills[randomized])
            {
                RandomizeSkill();
            }
            else
            {
                return randomizedSkill;
            }
        }

        return randomizedSkill;
    }

    public void CalculateSkill()
    {
        if (itemChest)
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
        if (!itemChest)
        {
            if (s1 == s2 || s1 == s3 || s2 == s3)
            {
                CalculateSkill();
            }
        }
    }

    void Drop()
    {
        if (!itemChest)
        {
            GameObject child1 = Instantiate(s1, new Vector2(transform.position.x, transform.position.y + offset.y), Quaternion.identity);
            GameObject child2 = Instantiate(s2, new Vector2(transform.position.x - offset.x, transform.position.y + offset.y), Quaternion.identity);
            GameObject child3 = Instantiate(s3, new Vector2(transform.position.x + offset.x, transform.position.y + offset.y), Quaternion.identity);

            child1.transform.parent = transform;
            child2.transform.parent = transform;
            child3.transform.parent = transform;

            Instantiate(Resources.Load("Particle FX/SkillChestFX"), transform.position, Quaternion.identity);
            aud.PlayOneShot(clip);
            open = true;
            sprite.sprite = openedSprite;
        }
        else
        {
            GameObject child1 = Instantiate(s1, new Vector2(transform.position.x, transform.position.y + offset.y), Quaternion.identity);

            child1.transform.parent = transform;

            Instantiate(Resources.Load("Particle FX/SkillChestFX"), transform.position, Quaternion.identity);
            aud.PlayOneShot(clip);
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
