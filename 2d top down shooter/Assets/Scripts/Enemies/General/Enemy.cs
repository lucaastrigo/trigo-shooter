using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public float speed;
    public float dropRate;
    public GameObject deadSprite;

    float time, _frostTime;
    [HideInInspector] public float speedK;
    [HideInInspector] public bool dead, frost;
    [HideInInspector] public bool canBeHurt = true;
    GameObject player;
    SpriteRenderer sprite;
    [HideInInspector] public Animator anim;
    Collider2D colli;
    Material mat;

    [Header("Skill Related")]
    public float _health;

    void Start()
    {
        dead = false;
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        colli = GetComponent<Collider2D>();
        mat = GetComponent<SpriteRenderer>().material;

        speedK = speed;


        //skills
        if(SkillStorage.value.exposed > 0 && SkillStorage.value.exposed < 1)
        {
            _health *= 1 - SkillStorage.value.exposed;
            health = Mathf.RoundToInt(_health);
        }
        else if(SkillStorage.value.exposed >= 1)
        {
            _health *= 0.95f;
            health = Mathf.RoundToInt(_health);
        }

        if (SkillStorage.value.scavenger > 0)
        {
            dropRate += SkillStorage.value.scavenger;
        }
    }

    void Update()
    {
        if (health <= 0 && !dead)
        {
            Die();
        }


        GameObject[] enemyBullet = GameObject.FindGameObjectsWithTag("Enemy Bullet");
        for (int i = 0; i < enemyBullet.Length; i++)
        {
            Physics2D.IgnoreCollision(enemyBullet[i].GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        //frost time
        if(_frostTime <= 0)
        {
            _frostTime = 0;
            if (frost)
            {
                Unfrost();
            }
        }
        else
        {
            _frostTime -= Time.deltaTime;
        }

        if (frost)
        {
            speed = 0;
        }
    }

    public void Frost(float frostT, float frostP) //color time and freeze percent
    {
        anim.speed = 0;

        if(_frostTime <= 7.5f)
        {
            _frostTime += frostT;
        }

        speed = 0;

        frost = true;

        if(GetComponentInChildren<WeaponEnemy>() != null)
        {
            GetComponentInChildren<WeaponEnemy>().enabled = false;
        }
    }

    void Unfrost()
    {
        anim.speed = 1;

        speed = speedK;
        frost = false;

        if (GetComponentInChildren<WeaponEnemy>() != null)
        {
            GetComponentInChildren<WeaponEnemy>().enabled = true;

            GameObject enemyWeapon = GetComponentInChildren<WeaponEnemy>().gameObject;

            if (enemyWeapon.transform.GetChild(0).GetComponentInChildren<Laser>() != null)
            {
                enemyWeapon.transform.GetChild(0).GetComponentInChildren<Laser>().enabled = true;
            }
        }

        MaterialBack(0);
    }

    public void TakeDamage(int damage, Material material, float colorT)
    {
        if (canBeHurt)
        {
            health -= damage;
            sprite.material = material;

            if (GetComponentInChildren<WeaponEnemy>() != null)
            {
                GameObject enemyWeapon = GetComponentInChildren<WeaponEnemy>().gameObject;
                enemyWeapon.GetComponent<SpriteRenderer>().material = material;
            }

            if (!frost)
            {
                StartCoroutine(MaterialBack(colorT));
            }
        }
    }

    IEnumerator MaterialBack(float time)
    {
        yield return new WaitForSeconds(time);
        sprite.material = mat;

        if (GetComponentInChildren<WeaponEnemy>() != null)
        {
            GameObject enemyWeapon = GetComponentInChildren<WeaponEnemy>().gameObject;
            enemyWeapon.GetComponent<SpriteRenderer>().material = mat;
        }
    }

    public void Shock(float dazedTime)
    {
        time = dazedTime;
    }

    void Die()
    {
        if(GetComponent<TwoStages>() == null)
        {
            StartCoroutine(Destroyed());
        }
        else
        {
            //change sprite to dead sprite
            sprite.sprite = deadSprite.GetComponent<SpriteRenderer>().sprite;
        }

        dead = true;

        if(GetComponent<SpawnerEnemy>() != null)
        {
            GetComponent<SpawnerEnemy>().Spawn();
        }


        //rewards
        if (Random.Range(0, 100) <= dropRate)
        {
            switch(Random.Range(0, 3))
            {
                case 0:
                    //ammo

                    if (SkillStorage.value.scavenger > 0)
                    {
                        Instantiate(Resources.Load("Items/MINI HEALTH"), new Vector2(transform.position.x + 0.3f, transform.position.y - 0.3f), Quaternion.identity);
                        Instantiate(Resources.Load("Items/AMMO CHEST"), new Vector2(transform.position.x - 0.3f, transform.position.y - 0.3f), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(Resources.Load("Items/MINI AMMO"), new Vector2(transform.position.x, transform.position.y - 0.3f), Quaternion.identity);
                    }

                    break;
                case 1:
                    //health

                    if (SkillStorage.value.scavenger > 0)
                    {
                        Instantiate(Resources.Load("Items/MINI AMMO"), new Vector2(transform.position.x + 0.3f, transform.position.y - 0.3f), Quaternion.identity);
                        Instantiate(Resources.Load("Items/HEALTH CHEST"), new Vector2(transform.position.x - 0.3f, transform.position.y - 0.3f), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(Resources.Load("Items/MINI HEALTH"), new Vector2(transform.position.x, transform.position.y - 0.3f), Quaternion.identity);
                    }

                    break;
                case 2:
                    //coin

                    if (SkillStorage.value.scavenger > 0)
                    {
                        Instantiate(Resources.Load("Items/COIN CHEST"), new Vector2(transform.position.x, transform.position.y - 0.3f), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(Resources.Load("Items/MINI COIN"), new Vector2(transform.position.x, transform.position.y - 0.3f), Quaternion.identity);
                    }

                    break;
            }
        }

        Instantiate(Resources.Load("Particle FX/DeathFX"), transform.position, Quaternion.identity);

        //disable everything
        anim.enabled = false;
        colli.enabled = false;


        //bloodlust skill
        if(Random.Range(0, 100) <= SkillStorage.value.bloodlust)
        {
            player.GetComponent<Player>().MoreHealth(1);
        }

        //ammo expert skill
        if(Random.Range(0, 100) <= SkillStorage.value.ammoExpert)
        {
            player.GetComponentInChildren<Weapon>().MoreAmmo(player.GetComponentInChildren<Weapon>().maxAmmo / 10);
        }
    }

    IEnumerator Destroyed()
    {
        sprite.sprite = null;
        Instantiate(deadSprite, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
