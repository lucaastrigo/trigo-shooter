using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    [Header("Bubble Shield")]
    public float bsCooldown;
    public float bsBubbleTime;
    public GameObject bubbleShield;


    [Header("Explosion Gift")]
    public float egCooldown;
    public GameObject explosionGift;


    [Header("Freeze")]
    public float fCooldown;
    public float fFreezeTime;


    [Header("Land Mine")]
    public float lmCooldown;
    public GameObject landMine;


    [Header("Leaf Dash")]
    public int ldDamage;
    public float ldCooldown;
    public float ldSpeed;
    public float ldRange;
    public LayerMask ldHitLayer;
    public LayerMask ldEnemiesLayer;


    [Header("Plunger")]
    public GameObject plunger;


    [Header("Speedster")]
    public float sSpeedMod;
    public float sSpeedsterTime;
    public float sCooldown;

    float sCooltime;
    float sSpeedK;

    //
    //
    //

    float cooltime;
    Vector3 mousePos;


    private void Start()
    {
        sSpeedK = GetComponent<PlayerMovement>().speed;
        sSpeedMod *= GetComponent<PlayerMovement>().speed;
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //bubble shield
        if (cooltime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //bubble shield
                if (SkillStorage.value.active[0])
                {
                    StartCoroutine(BubbleShield());
                }


                //explosion gift
                if (SkillStorage.value.active[1])
                {
                    ExplosionGift();
                }


                //freeze
                if (SkillStorage.value.active[2])
                {
                    StartCoroutine(Freeze());
                }


                //land mine
                if (SkillStorage.value.active[3])
                {
                    LandMine();
                }


                //leaf dash
                if (SkillStorage.value.active[4])
                {
                    LeafDash();
                }


                //speedster
                if (SkillStorage.value.active[6])
                {
                    StartCoroutine(Speedster());
                }
            }
        }
        else
        {
            cooltime -= Time.deltaTime;
        }

        //plunger
        plunger.SetActive(SkillStorage.value.active[5]);


        if(sCooltime > 0)
        {
            Instantiate(Resources.Load("Particle FX/SprintFX"), transform.position, Quaternion.identity);
            sCooltime -= Time.deltaTime;
        }
    }

    IEnumerator BubbleShield()
    {
        bubbleShield.SetActive(true);
        cooltime = bsCooldown;
        yield return new WaitForSeconds(bsBubbleTime);
        bubbleShield.SetActive(false);
    }

    void ExplosionGift()
    {
        Instantiate(explosionGift, transform.position, Quaternion.identity);
        cooltime = egCooldown;
    }

    IEnumerator Freeze()
    {
        Global.globalSpeed = 0;
        cooltime = fCooldown;
        yield return new WaitForSeconds(fFreezeTime);
        Global.globalSpeed = 1;
    }

    void LandMine()
    {
        Instantiate(landMine, transform.position, Quaternion.identity);
        cooltime = lmCooldown;
    }

    void LeafDash()
    {
        if (!GetComponent<PlayerMovement>().moving)
        {
            RaycastHit2D dashHit = Physics2D.Linecast(transform.position, mousePos, ldHitLayer);

            if (dashHit.collider)
            {
                DashToMouse(dashHit.point);
            }
            else
            {
                DashToMouse(mousePos);
            }
        }
        else
        {
            RaycastHit2D dashHit = Physics2D.Linecast(transform.position, GetComponent<PlayerMovement>().dashDirection, ldHitLayer);

            if (dashHit.collider)
            {
                DashToKey(dashHit.distance);
            }
            else
            {
                DashToKey(ldSpeed);
            }
        }

        Instantiate(Resources.Load("Particle FX/LeafDashFX"), transform.position, Quaternion.identity);
        cooltime = ldCooldown;
    }

    void DashToMouse(Vector2 dashPos)
    {
        Collider2D[] enemiesToDamage1 = Physics2D.OverlapBoxAll(transform.position, new Vector2(ldRange, ldRange), 0, ldEnemiesLayer);

        for (int i = 0; i < enemiesToDamage1.Length; i++)
        {
            if (enemiesToDamage1[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage1[i].GetComponent<Enemy>().TakeDamage(ldDamage, Resources.Load("Materials/Special Materials/standard MAT") as Material, 0.2f);
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, dashPos, ldSpeed);

        Collider2D[] enemiesToDamage2 = Physics2D.OverlapBoxAll(transform.position, new Vector2(ldRange, ldRange), 0, ldEnemiesLayer);

        for (int i = 0; i < enemiesToDamage2.Length; i++)
        {
            if (enemiesToDamage2[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage2[i].GetComponent<Enemy>().TakeDamage(ldDamage, Resources.Load("Materials/Special Materials/standard MAT") as Material, 0.2f);
            }
        }
    }

    void DashToKey(float dashDist)
    {
        Collider2D[] enemiesToDamage1 = Physics2D.OverlapBoxAll(transform.position, new Vector2(ldRange, ldRange), 0, ldEnemiesLayer);

        for (int i = 0; i < enemiesToDamage1.Length; i++)
        {
            if (enemiesToDamage1[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage1[i].GetComponent<Enemy>().TakeDamage(ldDamage, Resources.Load("Materials/Special Materials/standard MAT") as Material, 0.2f);
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, GetComponent<PlayerMovement>().dashDirection, dashDist);

        Collider2D[] enemiesToDamage2 = Physics2D.OverlapBoxAll(transform.position, new Vector2(ldRange, ldRange), 0, ldEnemiesLayer);

        for (int i = 0; i < enemiesToDamage2.Length; i++)
        {
            if (enemiesToDamage2[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage2[i].GetComponent<Enemy>().TakeDamage(ldDamage, Resources.Load("Materials/Special Materials/standard MAT") as Material, 0.2f);
            }
        }
    }

    IEnumerator Speedster()
    {
        GetComponent<PlayerMovement>().speed = sSpeedMod;
        cooltime = sCooldown;
        sCooltime = sSpeedsterTime;
        yield return new WaitForSeconds(sSpeedsterTime);
        GetComponent<PlayerMovement>().speed = sSpeedK;
    }
}
