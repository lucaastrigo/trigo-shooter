 using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public int maxAmmo;
    public int currentAmmo;
    public int shotsPerBurst;
    public float burstRate;
    public float fireRate;

    [Range(0f, 1f)]
    public float accuracy;

    public float offset;
    public float shakeDur;
    public float shakeMag;
    public bool playerWeapon;
    public bool singleShot;
    public bool burst;
    [HideInInspector] public bool onOff = true;
    [HideInInspector] public bool ammo;
    public Transform muzzle;
    public GameObject bullet;
    public TextMeshProUGUI nameWeapon;
    public AudioClip fireSound;

    public int weaponIndex;
    int shots;
    float fireTime;
    float angle;
    Vector2 mousePos;
    GameObject ammoText;
    GameObject valueStorage;
    GameObject skillStorage;
    Player player;
    AudioSource audioSource;

    [Header("Skill Related")]
    public float _fireRate;
    public float _accuracy;

    bool sharpshooterOn;
    bool fastHandsOn;

    void Start()
    {
        ammoText = GameObject.Find("Ammo Text");
        shots = shotsPerBurst;
        fireTime = fireRate;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();

        if(weaponIndex < 0)
        {
            currentAmmo = ValueStorage.value.WeaponAmmo[weaponIndex + 1];
        }
        else
        {
            currentAmmo = ValueStorage.value.WeaponAmmo[weaponIndex];
        }
    }

    void Update()
    {
        valueStorage = GameObject.FindGameObjectWithTag("Value Storage");
        skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");

        //here
        if(transform.parent != null)
        {
            if (transform.parent.GetComponent<PlayerPickup>().equippedWeapon.name == weaponName || transform.parent.GetComponent<PlayerPickup>().equippedWeapon.name == weaponName + "(Clone)")
            {
                valueStorage.GetComponent<ValueStorage>().WeaponAmmo[weaponIndex] = currentAmmo;
            }
        }

        if (!PauseMenu.paused)
        {
            if (playerWeapon && gameObject.activeInHierarchy)
            {
                //sharpshooter
                if (skillStorage.GetComponentInChildren<SharpshooterSkill>().skill.GetComponent<Skill>().skillOn)
                {
                    if (!sharpshooterOn)
                    {
                        if (skillStorage.GetComponentInChildren<SharpshooterSkill>() != null)
                        {
                            skillStorage.GetComponentInChildren<SharpshooterSkill>().weapon = this.gameObject;
                            skillStorage.GetComponentInChildren<SharpshooterSkill>().Activate();
                            sharpshooterOn = true;
                        }
                    }
                }
                else
                {
                    skillStorage.GetComponentInChildren<SharpshooterSkill>().Deactivate();
                }

                //fast hands
                if (skillStorage.GetComponentInChildren<FastHandsSkill>().skill.GetComponent<Skill>().skillOn)
                {
                    if (!fastHandsOn)
                    {
                        if(skillStorage.GetComponentInChildren<FastHandsSkill>() != null)
                        {
                            skillStorage.GetComponentInChildren<FastHandsSkill>().weapon = this.gameObject;
                            skillStorage.GetComponentInChildren<FastHandsSkill>().Activate();
                            fastHandsOn = true;
                        }
                    }
                }
                else
                {
                    skillStorage.GetComponentInChildren<FastHandsSkill>().Deactivate();
                }

                if (currentAmmo > 0)
                {
                    ammoText.GetComponent<TMP_Text>().text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
                    ammoText.GetComponent<TMP_Text>().color = Color.white;
                }
                else
                {
                    ammoText.GetComponent<TMP_Text>().text = "no ammo";
                    ammoText.GetComponent<TMP_Text>().color = Color.red;
                }
            }

            nameWeapon.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (onOff)
            {
                if (player.currentHealth <= 0)
                {
                    ValueStorage.value.weaponValue = "PISTOL";
                }
                else
                {
                    ValueStorage.value.weaponValue = weaponName;
                }

                if (mousePos.x < transform.position.x)
                {
                    transform.GetComponent<SpriteRenderer>().flipX = true;
                }

                if (mousePos.x >= transform.position.x)
                {
                    transform.GetComponent<SpriteRenderer>().flipX = false;
                }

                Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + offset);

                if (currentAmmo > 0)
                {
                    if (fireTime <= 0)
                    {
                        if (singleShot)
                        {
                            if (burst)
                            {
                                if (Input.GetKeyDown(KeyCode.Mouse0))
                                {
                                    if (shots > 0)
                                    {
                                        ShootBurst();
                                    }
                                    else
                                    {
                                        fireTime = fireRate;
                                        Invoke("ResetBurst", fireRate);
                                    }
                                }
                            }
                            else
                            {
                                if (Input.GetKeyDown(KeyCode.Mouse0))
                                {
                                    Shoot();
                                }
                            }
                        }
                        else
                        {
                            if (Input.GetKey(KeyCode.Mouse0))
                            {
                                Shoot();
                            }
                        }
                    }
                    else
                    {
                        fireTime -= Time.deltaTime;
                    }
                }
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (currentAmmo > maxAmmo)
            {
                currentAmmo = maxAmmo;
            }
        }

        if(accuracy >= 1)
        {
            accuracy = 1;
        }
    }

    void Shoot()
    {
        Quaternion spread = new Quaternion(0, 0, muzzle.rotation.z + Random.Range(-1f, 1f) * (1 - accuracy), muzzle.rotation.w);
        Camera.main.GetComponent<CameraScript>().Shake(shakeMag, shakeDur);
        currentAmmo--;
        Instantiate(bullet, muzzle.position, spread);
        fireTime = fireRate;
        audioSource.PlayOneShot(fireSound);
    }

    void ShootBurst()
    {
        Quaternion spread = new Quaternion(0, 0, muzzle.rotation.z + Random.Range(-1f, 1f) * (1 - accuracy), muzzle.rotation.w);
        Camera.main.GetComponent<CameraScript>().Shake(shakeMag, shakeDur);
        currentAmmo--;
        shots--;
        Instantiate(bullet, muzzle.position, spread);
        fireTime = burstRate;
        audioSource.PlayOneShot(fireSound);

        if (shots > 0 && currentAmmo > 0)
        {
            Invoke("ShootBurst", burstRate);
        }
    }

    void ResetBurst()
    {
        shots = shotsPerBurst;
    }

    public void MoreAmmo(int ammo)
    {
        player.time = player.indicationTime;

        currentAmmo += ammo;

        if (currentAmmo >= maxAmmo)
        {
            player.playerIndicator.GetComponent<TMP_Text>().text = "full ammo";
        }
        else
        {
            player.playerIndicator.GetComponent<TMP_Text>().text = "+ " + ammo.ToString() + " ammo";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            nameWeapon.enabled = !onOff;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            nameWeapon.enabled = false;
        }
    }

    public void LoadData()
    {
        if(transform.parent.GetComponent<PlayerPickup>().equippedWeaponName == weaponName)
        {
            currentAmmo = valueStorage.GetComponent<ValueStorage>().WeaponAmmo[weaponIndex];
        }
    }
}
