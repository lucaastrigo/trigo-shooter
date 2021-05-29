using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holster;
    public Transform secondHolster;
    public GameObject pickupFX;

    [HideInInspector] public bool hasWeapon;
    [HideInInspector] public bool hasTwoWeapons;
    [HideInInspector] public string equippedWeaponName;
    [HideInInspector] public string secondEquippedWeaponName;
    [HideInInspector] public string disequippedWeaponName;
    [HideInInspector] public Weapon focusedWeapon;
    [HideInInspector] public Weapon equippedWeapon;
    [HideInInspector] public Weapon secondEquippedWeapon;
    [HideInInspector] public Weapon disequippedWeapon;

    GameObject skillStorage, valueStorage;

    public int gunIndex;
    bool equip, secEquip, subEquip;

    private void Start()
    {
        //
    }

    void Update()
    {
        if (skillStorage == null)
        {
            skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
        }

        if (valueStorage == null)
        {
            valueStorage = GameObject.FindGameObjectWithTag("Value Storage");
        }

        if (focusedWeapon != null && Input.GetKeyDown(KeyCode.Q))
        {
            if (skillStorage.GetComponentInChildren<ExtraHolsterSkill>().skill.GetComponent<Skill>().skillOn)
            {
                if (hasTwoWeapons)
                {
                    DisequipWeapon(equippedWeapon);
                    SecondWeapon(secondEquippedWeapon); //maybe this line is not necessary
                }

                if (hasWeapon && !hasTwoWeapons)
                {
                    SecondWeapon(equippedWeapon);
                }

                EquipWeapon(focusedWeapon);
            }
            else
            {
                if (hasWeapon)
                {
                    DisequipWeapon(equippedWeapon);
                }

                EquipWeapon(focusedWeapon);
            }            
        }

        if (equippedWeapon != null)
        {
            ValueStorage.value.firstIndex = equippedWeapon.GetComponent<Weapon>().weaponIndex;

            equippedWeapon.transform.position = holster.transform.position;

            if (skillStorage.GetComponentInChildren<ExtraHolsterSkill>().skill.GetComponent<Skill>().skillOn)
            {
                if (subEquip)
                {
                    equippedWeapon.LoadData();
                    subEquip = false;
                }
            }
            else
            {
                if (!equip)
                {
                    equippedWeapon.LoadData();
                    equip = true;
                }
            }
        }

        if (secondEquippedWeapon != null)
        {
            ValueStorage.value.secondIndex = secondEquippedWeapon.GetComponent<Weapon>().weaponIndex;

            secondEquippedWeapon.transform.position = secondHolster.transform.position;

            if (!secEquip)
            {
                secondEquippedWeapon.LoadData();
                secEquip = true;
                subEquip = true;
            }
        }

        if (hasTwoWeapons)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                SwitchWeapon(secondEquippedWeapon, equippedWeapon);
            }
        }

    }

    void EquipWeapon(Weapon weaponToEquip)
    {
        if(weaponToEquip.transform.parent != transform && weaponToEquip.transform.parent != null)
        {
            Destroy(weaponToEquip.transform.parent.gameObject, 0.05f);
        }

        ValueStorage.value.weaponValue = weaponToEquip.name;
        hasWeapon = true;
        equippedWeaponName = weaponToEquip.name;
        equippedWeapon = weaponToEquip;
        equippedWeapon.transform.parent = transform;
        equippedWeapon.transform.localRotation = Quaternion.identity;
        equippedWeapon.GetComponent<BoxCollider2D>().enabled = false;
        equippedWeapon.GetComponent<Weapon>().playerWeapon = true;
        equippedWeapon.GetComponent<Weapon>().onOff = true;
        equippedWeapon.GetComponent<SpriteRenderer>().sortingOrder = 6;
        Instantiate(pickupFX, transform.position, Quaternion.identity);

        equippedWeapon.GetComponent<Weapon>().weaponIndex = gunIndex;
        valueStorage.GetComponent<ValueStorage>().WeaponAmmo.Insert(gunIndex, 1000);
        gunIndex++;

        /*
        if (equippedWeapon.GetComponent<Weapon>().weaponIndex == -1)
        {
            equippedWeapon.GetComponent<Weapon>().weaponIndex = gunIndex;
            valueStorage.GetComponent<ValueStorage>().WeaponAmmo.Insert(gunIndex, 1000);
            gunIndex++;
        }
        */
    }

    void DisequipWeapon(Weapon weaponToDisequip)
    {
        disequippedWeaponName = weaponToDisequip.name;
        disequippedWeapon = weaponToDisequip;
        disequippedWeapon.transform.parent = null;
        disequippedWeapon.transform.localRotation = Quaternion.identity;
        equippedWeapon.GetComponent<BoxCollider2D>().enabled = true;
        disequippedWeapon.GetComponent<Weapon>().playerWeapon = false;
        disequippedWeapon.GetComponent<Weapon>().onOff = false;

        disequippedWeapon.GetComponent<Weapon>().weaponIndex = -1;
    }

    void SecondWeapon(Weapon secondWeapon)
    {
        ValueStorage.value.secondWeaponValue = secondWeapon.name;
        hasTwoWeapons = true;
        secondEquippedWeaponName = secondWeapon.name;
        secondEquippedWeapon = secondWeapon;
        secondEquippedWeapon.transform.parent = transform;
        secondEquippedWeapon.transform.localRotation = Quaternion.identity;
        secondEquippedWeapon.GetComponent<BoxCollider2D>().enabled = true;
        secondEquippedWeapon.GetComponent<Weapon>().playerWeapon = false;
        secondEquippedWeapon.GetComponent<Weapon>().onOff = false;
        secondEquippedWeapon.GetComponent<SpriteRenderer>().sortingOrder = 4;
    }

    void SwitchWeapon(Weapon toEquip, Weapon toDisequip)
    {
        EquipWeapon(toEquip);
        SecondWeapon(toDisequip);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            focusedWeapon = collision.GetComponent<Weapon>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            focusedWeapon = null;
        }
    }
}
