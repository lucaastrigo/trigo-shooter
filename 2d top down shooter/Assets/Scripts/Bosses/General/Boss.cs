using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int health;
    public GameObject bigHealthChest;
    public GameObject bigAmmoKit;
    public GameObject deathFX;
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
        player = GameObject.Find("Player");
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        colli = GetComponent<BoxCollider2D>();
        mat = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
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
        if(health <= 150)
        {
            if (!secondStage)
            {
                Collider2D[] hitWall = Physics2D.OverlapCircleAll(transform.position, 3, wallLayer);

                foreach (Collider2D wall in hitWall)
                {
                    Destroy(wall.gameObject);
                }

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

        if (GetComponent<SpawnerEnemy>() != null)
        {
            GetComponent<SpawnerEnemy>().Spawn();
        }

        Instantiate(bigHealthChest, new Vector2(transform.position.x - 0.4f, transform.position.y + 0.3f), Quaternion.identity);
        Instantiate(bigAmmoKit, new Vector2(transform.position.x + 0.4f, transform.position.y + 0.3f), Quaternion.identity);

        Instantiate(deathFX, transform.position, Quaternion.identity);

        //change sprite to dead sprite
        sprite.sprite = deadSprite;

        //disable everything
        anim.enabled = false;
        colli.enabled = false;
    }
}
