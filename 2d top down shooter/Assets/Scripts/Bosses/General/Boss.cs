using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    public int health, secondStageHP;
    public GameObject healthBar;
    public GameObject bigHealthChest;
    public GameObject bigAmmoKit;
    public GameObject deathFX;
    public GameObject stairs;
    public Sprite deadSprite;
    public LayerMask wallLayer;

    [HideInInspector] public bool dead;
    [HideInInspector] public bool secondStage;
    [HideInInspector] public bool canBeHurt;
    GameObject player;
    SpriteRenderer sprite;
    Animator anim;
    Collider2D colli;
    GameObject skillStorage;
    Material mat;

    [Header("Skill Related")]
    public float _health;

    bool exposed;

    void Start()
    {
        dead = false;
        canBeHurt = true;
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        colli = GetComponent<BoxCollider2D>();
        mat = GetComponent<SpriteRenderer>().material;

        healthBar.GetComponent<HealthBar>().SetMaxHealth(health);
    }

    void Update()
    {
        healthBar.GetComponent<HealthBar>().SetHealth(health);

        if (skillStorage == null)
        {
            skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
        }

        if (skillStorage.GetComponentInChildren<ExposedSkill>().skill.GetComponent<Skill>().skillOn)
        {
            if (!exposed)
            {
                _health *= skillStorage.GetComponentInChildren<ExposedSkill>()._healthDecrease;
                health = Mathf.RoundToInt(_health);
                exposed = true;
            }
        }

        //STATE MACHINE
        if(health <= secondStageHP)
        {
            if (!secondStage)
            {
                secondStage = true;
                anim.SetTrigger("second stage");
            }
        }

        if (health <= 0 && !dead)
        {
            //anim.SetTrigger("death");
            Die();
        }


        GameObject[] enemyBullet = GameObject.FindGameObjectsWithTag("Enemy Bullet");
        for (int i = 0; i < enemyBullet.Length; i++)
        {
            Physics2D.IgnoreCollision(enemyBullet[i].GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    public void TakeDamage(int damage, Material color, float colorT)
    {
        if (canBeHurt)
        {
            //anim.SetTrigger("hurt");
            health -= damage;
            sprite.material = color;
            StartCoroutine(Vanish(colorT));
        }
    }

    IEnumerator Vanish(float time)
    {
        yield return new WaitForSeconds(time);
        sprite.material = mat;
    }

    public void TakeColor(int damage, Color color, float colorT)
    {
        health -= damage;
        sprite.color = color;
        StartCoroutine(ColorBack(colorT));
    }

    IEnumerator ColorBack(float time)
    {
        yield return new WaitForSeconds(time);
        sprite.color = Color.white;
    }

    void Die()
    {
        //spawn portal

        dead = true;

        Instantiate(bigHealthChest, new Vector2(transform.position.x - 0.4f, transform.position.y + 0.3f), Quaternion.identity);
        Instantiate(bigAmmoKit, new Vector2(transform.position.x + 0.4f, transform.position.y + 0.3f), Quaternion.identity);

        Instantiate(deathFX, transform.position, Quaternion.identity);
        Instantiate(stairs, new Vector2(transform.position.x, transform.position.y + 2.5f), Quaternion.identity);

        //change sprite to dead sprite
        sprite.sprite = deadSprite;

        //disable everything
        if(anim != null)
        {
            anim.enabled = false;
        }

        if (colli != null)
        {
            colli.enabled = false;
        }
    }
}
