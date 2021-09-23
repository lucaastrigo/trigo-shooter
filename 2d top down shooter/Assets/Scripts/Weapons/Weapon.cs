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
    public bool onOff;
    [HideInInspector] public bool withPlayer;
    [HideInInspector] public bool ammo, cantDrop;
    public Transform muzzle;
    [HideInInspector] public GameObject ammoBar;
    public GameObject bullet;
    public TextMeshProUGUI weaponDescription;
    public AudioClip fireSound;

    public int weaponIndex;
    int shots;
    float fireTime;
    float angle;
    float _accuracy, _fireRate;
    Vector2 mousePos;
    GameObject ammoText;
    Player player;
    AudioSource audioSource;

    void Start()
    {
        ammoBar = GameObject.Find("Ammo Bar");

        ammoText = GameObject.Find("Ammo Text");
        shots = shotsPerBurst;
        fireTime = fireRate;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();

        weaponDescription.GetComponent<TMP_Text>().enabled = false;

        if (bullet.GetComponentInChildren<BulletScript>() != null)
        {
            weaponDescription.GetComponent<TMP_Text>().text = weaponName + ": damage " + bullet.GetComponentInChildren<BulletScript>().damage.ToString() + " - fire rate " + fireRate.ToString() + " - accuracy " + accuracy.ToString();
        }
        else if(bullet.GetComponent<BulletScript>() != null)
        {
            weaponDescription.GetComponent<TMP_Text>().text = weaponName + ": damage " + bullet.GetComponent<BulletScript>().damage.ToString() + " - fire rate " + fireRate.ToString() + " - accuracy " + accuracy.ToString();
        }


        _accuracy = accuracy;
        _fireRate = fireRate;


        //skills
        if(SkillStorage.value.sharpshooter > 0)
        {
            accuracy += SkillStorage.value.sharpshooter * _accuracy;
        }

        if(SkillStorage.value.fastHands > 0)
        {
            fireRate -= SkillStorage.value.fastHands * _fireRate;
        }
    }

    void Update()
    {
        if (!PauseMenu.paused)
        {
            if (playerWeapon && gameObject.activeInHierarchy)
            {
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

            //nameWeapon.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (onOff)
            {
                ammoBar.GetComponent<AmmoBar>().SetAmmo(currentAmmo);
                ammoBar.GetComponent<AmmoBar>().SetMaxAmmo(maxAmmo);

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

                if (currentAmmo > 0 && !Minimap.mapped)
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

        if (accuracy >= 1)
        {
            accuracy = 1;
        }

        if (withPlayer)
        {
            if (onOff)
            {
                //ValueStorage.value.weaponValue = weaponName;
            }
            else
            {
                //ValueStorage.value.secondWeaponValue = weaponName;
            }
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

        if(shots == 0)
        {
            ResetBurst();
        }
    }

    void ResetBurst()
    {
        shots = shotsPerBurst;
        fireTime = fireRate;
    }

    public void MoreAmmo(int ammo)
    {
        Vector3 indicatorPos = Camera.main.WorldToScreenPoint(player.transform.position + new Vector3(0, 1, 0));
        GameObject ind;

        ind = Instantiate(player.indicator, indicatorPos, Quaternion.identity);
        ind.transform.SetParent(player.canvas);
        ind.GetComponent<Animator>().SetTrigger("active");
        ind.GetComponent<TMP_Text>().color = Color.white;

        if(currentAmmo + ammo >= maxAmmo)
        {
            ind.GetComponent<TMP_Text>().text = "full ammo";
        }
        else
        {
            ind.GetComponent<TMP_Text>().text = ammo.ToString();
        }

        currentAmmo += ammo;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            weaponDescription.enabled = !onOff;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            weaponDescription.enabled = false;
        }
    }

    void OnDestroy()
    {
        SaveData();
    }

    void SaveData()
    {
        //save the current ammo value of this weapon

        if(player.currentHealth > 0)
        {
            if (withPlayer)
            {
                if (onOff)
                {
                    ValueStorage.value.firstAmmo = currentAmmo;
                    ValueStorage.value.weaponValue = weaponName;
                }
                else
                {
                    ValueStorage.value.secondAmmo = currentAmmo;
                    ValueStorage.value.secondWeaponValue = weaponName;
                }
            }
        }
        else
        {
            ValueStorage.value.weaponValue = "PISTOL";
            ValueStorage.value.firstAmmo = 10000;

            ValueStorage.value.secondWeaponValue = null;
            ValueStorage.value.secondAmmo = 10000;
        }
    }

    public void LoadData()
    {
        //load the ammo value to this weapon
        if(weaponName == ValueStorage.value.weaponValue)
        {
            currentAmmo = ValueStorage.value.firstAmmo;
        }
        else if(weaponName == ValueStorage.value.secondWeaponValue)
        {
            currentAmmo = ValueStorage.value.secondAmmo;
        }
    }
}
