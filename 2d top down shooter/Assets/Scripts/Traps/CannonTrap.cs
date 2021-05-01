using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTrap : MonoBehaviour
{
    public int shotsPerBurst;
    public float fireRate;
    public float burstRate;
    public GameObject bullet;
    public AudioClip fireSound;
    public Transform muzzle;

    public int shots;
    float fireTime;
    bool firing;
    GameObject player;
    AudioSource audioSource;

    void Start()
    {
        player = GameObject.Find("Player");
        audioSource = GetComponent<AudioSource>();

        shots = shotsPerBurst;
    }

    void Update()
    {
        if (Global.globalSpeed > 0)
        {
            if (canShoot())
            {
                if(shots > 0)
                {
                    Shoot();
                }
                else
                {
                    firing = false;
                    Invoke("ResetBurst", burstRate);
                }
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

    void Shoot()
    {
        Instantiate(bullet, muzzle.position, muzzle.rotation);
        shots--;
        fireTime = burstRate;
        audioSource.PlayOneShot(fireSound);

        if (shots > 0)
        {
            Invoke("Shoot", burstRate);
        }
    }

    void ResetBurst()
    {
        shots = shotsPerBurst - 1;
    }
}
