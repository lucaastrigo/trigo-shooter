using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnemy : MonoBehaviour
{
    public float fireRange;
    public float fireRate;
    public float secondStageFireRate;
    public GameObject bullet;
    public GameObject laser;
    public AudioClip fireSound; 
    public Transform muzzle;

    float fireTime;
    float angle;
    bool firing;
    GameObject player;
    GameObject parent;
    AudioSource audioSource;
    Animator laserAnim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        parent = transform.parent.gameObject;
        audioSource = GetComponent<AudioSource>();

        if(laser != null)
        {
            laserAnim = laser.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if(parent.GetComponent<Enemy>() != null)
        {
            if (!parent.GetComponent<Enemy>().dead)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (distanceToPlayer <= fireRange && parent.GetComponent<MovementEnemy>().canFollow())
                {
                    //it points towards the player
                    Vector3 difference = player.transform.position - transform.position;
                    angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle - 90);

                    if (Global.globalSpeed > 0)
                    {
                        if (canShoot())
                        {
                            if (laser != null)
                            {
                                laserAnim.SetTrigger("shoot");

                                GetComponentInParent<Enemy>().speed = 0;
                            }
                            else
                            {
                                Shoot();
                            }
                        }
                    }
                }
                else
                {
                    //it points towards the next patrol point
                    Vector3 difference = parent.GetComponent<MovementEnemy>().nextPatrolPoint - transform.position;
                    angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    bool canShoot()
    {
        if (fireTime >= fireRate)
        {
            firing = true;
        }
        else
        {
            firing = false;
            fireTime += Time.deltaTime;
        }

        return firing;
    }

    public bool canShootSecondStage()
    {
        if (fireTime >= secondStageFireRate)
        {
            firing = true;
        }
        else
        {
            firing = false;
            fireTime += Time.deltaTime;
        }

        return firing;
    }

    public void Shoot()
    {
        Instantiate(bullet, muzzle.position, muzzle.rotation);
        fireTime = 0;
        firing = false;
        audioSource.PlayOneShot(fireSound);
    }
}
