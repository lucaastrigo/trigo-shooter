using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public enum bulletType
    {
        bullet, arrow
    }

    public bulletType _bulletType;

    public bool explosive, flame, electric, ice;

    public int damage, fireDamage;
    public float speed, speedMod, lifetime, colorTime, explosionRange, shakeMag, shakeDur;

    [Range(0, 1)]
    public float freezePercent;

    public bool changeSpeed;
    public GameObject hitFX, explosionFX;
    public Material mat;
    public Color color;

    GameObject flameHit, iceHit;
    GameObject skillStorage;
    SpriteRenderer sprite;

    bool reinforced, ghosted, wided;

    public float _damage;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        if(Global.globalSpeed > 0)
        {
            if (explosive)
            {
                Invoke("Explode", lifetime);
            }
            else
            {
                Destroy(gameObject, lifetime);
            }
        }

        skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");

        if (skillStorage.GetComponentInChildren<ReinforcedBarrelSkill>().skill.GetComponent<Skill>().skillOn)
        {
            if (!reinforced)
            {
                _damage *= skillStorage.GetComponentInChildren<ReinforcedBarrelSkill>()._damageIncreased;
                damage = Mathf.RoundToInt(_damage);
                reinforced = true;
            }
        }

        if (skillStorage.GetComponentInChildren<WiderMuzzleSkill>().skill.GetComponent<Skill>().skillOn)
        {
            if (!wided)
            {
                transform.localScale *= skillStorage.GetComponentInChildren<WiderMuzzleSkill>()._sizeIncrease;
                wided = true;
            }
        }
    }

    void Update()
    {
        if (Global.globalSpeed > 0)
        {
            Invoke("Explode", lifetime);
        }

        transform.Translate(Vector2.up * speed * Time.deltaTime * Global.globalSpeed);

        if (changeSpeed)
        {
            if(speed >= 1)
            {
                speed += Time.deltaTime * speedMod;
            }
        }

        if(skillStorage != null)
        {
            skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
        }
    }

    void Explode()
    {
        if (GetComponent<Explosive>() != null)
        {
            Instantiate(GetComponent<Explosive>().explosionArea, transform.position, Quaternion.identity);
        }

        Camera.main.GetComponent<CameraScript>().Shake(shakeMag, shakeDur);
        speed = 0;

        Collider2D[] hitExplosion = Physics2D.OverlapCircleAll(transform.position, explosionRange);

        foreach(Collider2D thing in hitExplosion)
        {
            if(thing.GetComponent<Enemy>() != null)
            {
                thing.GetComponent<Enemy>().TakeDamage(damage, mat, colorTime);
            }

            if(thing.GetComponent<Boss>() != null)
            {
                thing.GetComponent<Boss>().TakeDamage(damage, mat, colorTime);
            }

            if (ice)
            {
                iceHit = thing.gameObject;
                Freeze();
            }

            if (thing.gameObject.CompareTag("Wall"))
            {
                Destroy(thing.gameObject);
            }
        }

        Instantiate(explosionFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    IEnumerator Burn()
    {
        yield return new WaitForSeconds(colorTime);

        if(flameHit != null)
        {
            if(flameHit.GetComponent<Enemy>() != null)
            {
                flameHit.GetComponent<Enemy>().TakeDamage(fireDamage, mat, colorTime / 2);
            }

            if(flameHit.GetComponent<Boss>() != null)
            {
                flameHit.GetComponent<Boss>().TakeDamage(fireDamage, mat, colorTime / 2);
            }
        }

        Instantiate(hitFX, flameHit.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void Freeze()
    {
        if (iceHit != null)
        {
            if (iceHit.GetComponent<Enemy>() != null)
            {
                iceHit.GetComponent<Enemy>().Frost(colorTime, freezePercent);
            }

            if (iceHit.GetComponent<MovementBoss>() != null)
            {
                iceHit.GetComponent<MovementBoss>().Frost(colorTime, freezePercent);
            }
        }

        if(_bulletType == bulletType.bullet)
        {
            Destroy(gameObject);

        }else if(_bulletType == bulletType.arrow)
        {
            Destroy(gameObject, lifetime / 2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.gameObject;

        if(_bulletType == bulletType.arrow)
        {
            if (!flame && !ice)
            {
                if (other.GetComponent<Enemy>() != null)
                {
                    if (!explosive)
                    {
                        other.GetComponent<Enemy>().TakeDamage(damage, mat, colorTime);

                        if (!skillStorage.GetComponentInChildren<GhostBulletsSkill>().skill.GetComponent<Skill>().skillOn)
                        {
                            Destroy(gameObject, lifetime);
                        }
                    }
                    else
                    {
                        Invoke("Explode", lifetime);
                    }
                }

                if (other.GetComponent<Boss>() != null)
                {
                    if (!explosive)
                    {
                        other.GetComponent<Boss>().TakeDamage(damage, mat, colorTime);

                        if (!skillStorage.GetComponentInChildren<GhostBulletsSkill>().skill.GetComponent<Skill>().skillOn)
                        {
                            Destroy(gameObject, lifetime);
                        }
                    }
                    else
                    {
                        Invoke("Explode", lifetime);
                    }
                }
            }
            else
            {
                if (other.GetComponent<Enemy>() != null)
                {
                    if (!explosive)
                    {
                        other.GetComponent<Enemy>().TakeColor(damage, color, colorTime);
                    }
                    else
                    {
                        Invoke("Explode", lifetime);
                    }
                }

                if (other.GetComponent<Boss>() != null)
                {
                    if (!explosive)
                    {
                        other.GetComponent<Boss>().TakeColor(damage, color, colorTime);
                    }
                    else
                    {
                        Invoke("Explode", lifetime);
                    }
                }
            }

            transform.parent = other.transform;
            speed = 0;
            Instantiate(hitFX, transform.position, Quaternion.identity);
        }
        else if(_bulletType == bulletType.bullet)
        {
            if (!flame && !ice)
            {
                if (explosive)
                {
                    Explode();
                }
                else
                {
                    if (other.GetComponent<Enemy>() != null)
                    {
                        other.GetComponent<Enemy>().TakeDamage(damage, mat, colorTime);
                    }

                    if (other.GetComponent<Boss>() != null)
                    {
                        other.GetComponent<Boss>().TakeDamage(damage, mat, colorTime);
                    }

                    Instantiate(hitFX, transform.position, Quaternion.identity);

                    if (!skillStorage.GetComponentInChildren<GhostBulletsSkill>().skill.GetComponent<Skill>().skillOn)
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                if (explosive)
                {
                    Explode();
                }
                else
                {
                    if (other.GetComponent<Enemy>() != null)
                    {
                        other.GetComponent<Enemy>().TakeColor(damage, color, colorTime);
                    }

                    if (other.GetComponent<Boss>() != null)
                    {
                        other.GetComponent<Boss>().TakeColor(damage, color, colorTime);
                    }

                    Instantiate(hitFX, transform.position, Quaternion.identity);
                }
            }
        }

        if (flame)
        {
            flameHit = collision.gameObject;

            StartCoroutine(Burn());

            if (!skillStorage.GetComponentInChildren<GhostBulletsSkill>().skill.GetComponent<Skill>().skillOn)
            {
                speed = 0;

                if (_bulletType == bulletType.bullet)
                {
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }

                gameObject.GetComponent<Collider2D>().enabled = false;

                if (GetComponent<TrailRenderer>() != null)
                {
                    gameObject.GetComponent<TrailRenderer>().enabled = false;
                }
            }
        }

        if (ice)
        {
            iceHit = collision.gameObject;

            Freeze();

            if (!skillStorage.GetComponentInChildren<GhostBulletsSkill>().skill.GetComponent<Skill>().skillOn)
            {
                speed = 0;

                if(_bulletType == bulletType.bullet)
                {
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }

                gameObject.GetComponent<Collider2D>().enabled = false;

                if(GetComponent<TrailRenderer>() != null)
                {
                    gameObject.GetComponent<TrailRenderer>().enabled = false;
                }
            }
        }

        if(other.CompareTag("Indestructible Wall") || other.CompareTag("Door"))
        {
            if(_bulletType == bulletType.bullet)
            {
                Destroy(gameObject);
            }

            if(_bulletType == bulletType.arrow)
            {
                Destroy(gameObject, lifetime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
