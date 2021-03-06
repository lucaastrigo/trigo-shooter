using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public enum bulletType
    {
        bullet, arrow, mine
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

    GameObject flameHit, iceHit, electricHit;
    SpriteRenderer sprite;
    Rigidbody2D rb;
    Vector2 moveDir;

    bool reinforced, ghosted, wided;
    float iSpeed;
    float _damage;

    void Start()
    {
        moveDir = Vector2.up;

        sprite = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        if(Global.globalSpeed > 0)
        {
            if(_bulletType != bulletType.mine)
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
        }


        _damage = damage;


        if (_bulletType != bulletType.mine)
        {
            if(SkillStorage.value.reinforcedBarrel > 0)
            {
                damage += Mathf.RoundToInt(SkillStorage.value.reinforcedBarrel * _damage);
            }

            if(SkillStorage.value.widerMuzzle > 0)
            {
                transform.localScale *= SkillStorage.value.widerMuzzle + 1;
            }
        }

        iSpeed = -speed;
    }

    void Update()
    {
        if (Global.globalSpeed > 0)
        {
            if (_bulletType != bulletType.mine)
            {
                Invoke("Explode", lifetime);
            }
        }

        transform.Translate(moveDir * speed * Time.deltaTime * Global.globalSpeed);

        if (changeSpeed)
        {
            if(speed >= 1)
            {
                speed += Time.deltaTime * speedMod;
            }
            else if(speed <= -1)
            {
                speed -= Time.deltaTime * speedMod;
            }
        }
    }

    void Explode()
    {
        if (GetComponent<Explosive>() != null)
        {
            GameObject explo = GetComponent<Explosive>().explosionArea;
            GameObject explos = Instantiate(explo, transform.position, Quaternion.identity);
            explos.transform.localScale = new Vector3(explosionRange, explosionRange, explosionRange);
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

            if (ice)
            {
                iceHit = thing.gameObject;
                Freeze();
            }

            if (electric)
            {
                electricHit = thing.gameObject;
                Shock();
            }

            if (thing.gameObject.CompareTag("Wall"))
            {
                Destroy(thing.gameObject);
            }

            if(thing.gameObject.GetComponent<Prop>() != null)
            {
                thing.gameObject.GetComponent<Prop>().DestroyProp();
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
                flameHit.GetComponent<Enemy>().TakeDamage(fireDamage, Resources.Load("Materials/Special Materials/standard MAT") as Material, colorTime / 2);
                Instantiate(hitFX, flameHit.transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }

    void Freeze()
    {
        if (iceHit != null)
        {
            if (iceHit.GetComponent<Enemy>() != null)
            {
                iceHit.GetComponent<Enemy>().Frost(colorTime, freezePercent);

                if (iceHit.GetComponentInChildren<WeaponEnemy>() != null)
                {
                    iceHit.GetComponentInChildren<WeaponEnemy>().enabled = false;

                    GameObject enemyWeapon = iceHit.GetComponentInChildren<WeaponEnemy>().gameObject;

                    if(enemyWeapon.transform.GetChild(0).GetComponentInChildren<Laser>() != null)
                    {
                        enemyWeapon.transform.GetChild(0).GetComponentInChildren<Laser>().enabled = false;
                    }
                }
            }
        }

        if(_bulletType == bulletType.bullet)
        {
            Destroy(gameObject);
        }
        else if(_bulletType == bulletType.arrow)
        {
            Destroy(gameObject, lifetime / 2);
        }
    }

    void Shock()
    {
        if (electricHit != null)
        {
            if (electricHit.GetComponent<Enemy>() != null)
            {
                electricHit.GetComponent<Enemy>().Frost(colorTime, freezePercent);
            }
        }

        if (_bulletType == bulletType.bullet)
        {
            Destroy(gameObject);
        }
        else if (_bulletType == bulletType.arrow)
        {
            Destroy(gameObject, lifetime / 2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.gameObject;

        if(_bulletType == bulletType.arrow)
        {
            if (!flame && !ice & !electric)
            {
                if (other.GetComponent<Enemy>() != null)
                {
                    if (!explosive)
                    {
                        other.GetComponent<Enemy>().TakeDamage(damage, mat, colorTime);

                        if (!SkillStorage.value.activeSkills[GhostBullets.ID]) //ghost bullets
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
                        if(other.GetComponent<BossHealth>() == null)
                        {
                            other.GetComponent<Enemy>().TakeDamage(damage, mat, colorTime);
                        }
                        else
                        {
                            other.GetComponent<Enemy>().TakeDamage(damage, Resources.Load("Materials/Special Materials/standard MAT") as Material, 0.2f);
                        }
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
            if (!flame && !ice && !electric)
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

                    Instantiate(hitFX, transform.position, Quaternion.identity);

                    if (!SkillStorage.value.activeSkills[GhostBullets.ID]) //ghost bullets
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
                        if (other.GetComponent<BossHealth>() == null)
                        {
                            other.GetComponent<Enemy>().TakeDamage(damage, mat, colorTime);
                        }
                        else
                        {
                            other.GetComponent<Enemy>().TakeDamage(damage, Resources.Load("Materials/Special Materials/standard MAT") as Material, 0.2f);
                        }
                    }

                    Instantiate(hitFX, transform.position, Quaternion.identity);
                }
            }
        }
        else if(_bulletType == bulletType.mine)
        {
            if (explosive)
            {
                Explode();
            }
        }

        if (flame)
        {
            flameHit = collision.gameObject;

            if(collision.GetComponent<BossHealth>() == null)
            {
                StartCoroutine(Burn());
            }
            else
            {
                Destroy(gameObject);
            }

            if (!SkillStorage.value.activeSkills[GhostBullets.ID]) //ghost bullets
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

            if(collision.GetComponent<BossHealth>() == null)
            {
                Freeze();
            }
            else
            {
                Destroy(gameObject);
            }

            if (!SkillStorage.value.activeSkills[GhostBullets.ID]) //ghost bullets
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

        if (electric)
        {
            electricHit = collision.gameObject;

            if(collision.GetComponent<BossHealth>() == null)
            {
                Shock();
            }
            else
            {
                Destroy(gameObject);
            }

            if (!SkillStorage.value.activeSkills[GhostBullets.ID]) //ghost bullets
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

        if (other.CompareTag("Indestructible Wall") || other.CompareTag("Door"))
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
