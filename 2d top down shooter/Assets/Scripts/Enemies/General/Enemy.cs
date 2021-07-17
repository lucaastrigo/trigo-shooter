using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public float speed;
    public float healthChestDropRate;
    public GameObject healthChest;
    public GameObject bigHealthChest;
    public GameObject ammoKit;
    public GameObject deathFX;
    public Sprite deadSprite;

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
            if(speed <= 0)
            {
                speed = 0;
            }
        }
    }

    public void Frost(float frostT, float frostP) //color time and freeze percent
    {
        _frostTime = frostT;

        frostP *= speedK;
        speed -= frostP;

        frost = true;
    }

    void Unfrost()
    {
        speed = speedK;
        frost = false;
    }

    public void TakeDamage(int damage, Material material, float colorT)
    {
        if (canBeHurt)
        {
            health -= damage;
            sprite.material = material;
            StartCoroutine(MaterialBack(colorT));
        }
    }

    IEnumerator MaterialBack(float time)
    {
        yield return new WaitForSeconds(time);
        sprite.material = mat;
    }

    public void TakeColor(int damage, Color color, float colorT)
    {
        if (canBeHurt)
        {
            health -= damage;
            sprite.color = color;
            StartCoroutine(ColorBack(colorT));
        }
    }

    IEnumerator ColorBack(float time)
    {
        yield return new WaitForSeconds(time);
        sprite.color = Color.white;
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

        dead = true;

        if(GetComponent<SpawnerEnemy>() != null)
        {
            GetComponent<SpawnerEnemy>().Spawn();
        }

        //scavenger skill
        if (Random.Range(0, 100) < healthChestDropRate)
        {
            if (scavenger)
            {
                Instantiate(bigHealthChest, new Vector2(transform.position.x + 0.3f, transform.position.y + 0.3f), Quaternion.identity);
                Instantiate(ammoKit, new Vector2(transform.position.x - 0.3f, transform.position.y - 0.3f), Quaternion.identity);
            }
            else
            {
                Instantiate(healthChest, transform.position, Quaternion.identity);
            }
        }

        Instantiate(deathFX, transform.position, Quaternion.identity);

        //change sprite to dead sprite
        sprite.sprite = deadSprite;

        //disable everything
        anim.enabled = false;
        colli.enabled = false;


        //bloodlust skill
        if (skillStorage.GetComponentInChildren<BloodlustSkill>().skill.GetComponent<Skill>().skillOn)
        {
            if (Random.Range(0, 100) <= skillStorage.GetComponentInChildren<BloodlustSkill>()._healthUpRate)
            {
                print("ganhou vida do inimigo");
                player.GetComponent<Player>().MoreHealth(1);
            }
        }

        //ammo expert skill
        if (skillStorage.GetComponentInChildren<AmmoExpertSkill>().skill.GetComponent<Skill>().skillOn)
        {
            if (Random.Range(0, 100) <= skillStorage.GetComponentInChildren<AmmoExpertSkill>()._ammoUpRate)
            {
                print("ganhou munição do inimigo");
                player.GetComponentInChildren<Weapon>().MoreAmmo(player.GetComponentInChildren<Weapon>().maxAmmo / 10);
            }
        }
    }

    IEnumerator Destroyed()
    {
        yield return new WaitForSeconds(1);
        Instantiate(deathFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
