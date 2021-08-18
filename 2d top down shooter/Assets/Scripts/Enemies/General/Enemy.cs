using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public float speed;
    public float healthChestDropRate, coinDropRate, ammoDropRate;
    public GameObject deadSprite;

    float time, _frostTime;
    [HideInInspector] public float speedK;
    [HideInInspector] public bool dead, frost;
    [HideInInspector] public bool canBeHurt = true;
    GameObject player;
    SpriteRenderer sprite;
    [HideInInspector] public Animator anim;
    Collider2D colli;
    GameObject skillStorage;
    Material mat;

    [Header("Skill Related")]
    public float _health;
    public float _dropRate;

    bool exposed;
    bool scavenger;

    void Start()
    {
        dead = false;
        player = GameObject.FindGameObjectWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        colli = GetComponent<Collider2D>();
        mat = GetComponent<SpriteRenderer>().material;

        speedK = speed;
    }

    void Update()
    {
        if(skillStorage == null)
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

        if (skillStorage.GetComponentInChildren<ScavengerSkill>().skill.GetComponent<Skill>().skillOn)
        {
            if (!scavenger)
            {
                skillStorage.GetComponentInChildren<ScavengerSkill>().increase = healthChestDropRate * skillStorage.GetComponentInChildren<ScavengerSkill>().dropRateIncrease;
                healthChestDropRate += skillStorage.GetComponentInChildren<ScavengerSkill>().increase;
                scavenger = true;
            }
        }
        else
        {
            healthChestDropRate = _dropRate;
        }

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

    //public void TakeColor(int damage, Color color, float colorT)
    //{
    //    if (canBeHurt)
    //    {
    //        health -= damage;
    //        sprite.color = color;
    //        StartCoroutine(ColorBack(colorT));
    //    }
    //}

    //IEnumerator ColorBack(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    sprite.color = Color.white;
    //}

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
        if (Random.Range(0, 100) <= ammoDropRate)
        {
            if (scavenger)
            {
                Instantiate(Resources.Load("Items/MINI HEALTH"), new Vector2(transform.position.x + 0.3f, transform.position.y + 0.3f), Quaternion.identity);
                Instantiate(Resources.Load("Items/AMMO CHEST"), new Vector2(transform.position.x - 0.3f, transform.position.y - 0.3f), Quaternion.identity);
            }
            else
            {
                Instantiate(Resources.Load("Items/MINI AMMO"), transform.position, Quaternion.identity);
            }
        }
        else if (Random.Range(0, 100) <= healthChestDropRate)
        {
            if (scavenger)
            {
                Instantiate(Resources.Load("Items/HEALTH CHEST"), new Vector2(transform.position.x + 0.3f, transform.position.y + 0.3f), Quaternion.identity);
                Instantiate(Resources.Load("Items/MINI AMMO"), new Vector2(transform.position.x - 0.3f, transform.position.y - 0.3f), Quaternion.identity);
            }
            else
            {
                Instantiate(Resources.Load("Items/MINI HEALTH"), transform.position, Quaternion.identity);
            }
        }
        else if (Random.Range(0, 100) <= coinDropRate)
        {
            if (scavenger)
            {
                Instantiate(Resources.Load("Items/COIN CHEST"), transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(Resources.Load("Items/MINI COIN"), transform.position, Quaternion.identity);
            }
        }

        Instantiate(Resources.Load("Particle FX/DeathFX"), transform.position, Quaternion.identity);

        //disable everything
        anim.enabled = false;
        colli.enabled = false;


        //bloodlust skill
        if (skillStorage.GetComponentInChildren<BloodlustSkill>().skill.GetComponent<Skill>().skillOn)
        {
            if (Random.Range(0, 100) <= skillStorage.GetComponentInChildren<BloodlustSkill>()._healthUpRate)
            {
                player.GetComponent<Player>().MoreHealth(1);
            }
        }

        //ammo expert skill
        if (skillStorage.GetComponentInChildren<AmmoExpertSkill>().skill.GetComponent<Skill>().skillOn)
        {
            if (Random.Range(0, 100) <= skillStorage.GetComponentInChildren<AmmoExpertSkill>()._ammoUpRate)
            {
                player.GetComponentInChildren<Weapon>().MoreAmmo(player.GetComponentInChildren<Weapon>().maxAmmo / 10);
            }
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
