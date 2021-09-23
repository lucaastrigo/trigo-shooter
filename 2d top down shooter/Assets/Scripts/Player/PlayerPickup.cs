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

    [HideInInspector] public Weapon focusedWeapon;
    [HideInInspector] public Weapon equippedWeapon;
    [HideInInspector] public Weapon secondEquippedWeapon;
    [HideInInspector] public Weapon disequippedWeapon;

    private void Start()
    {
        for (int i = 0; i < WeaponStorage.value.weapons.Length; i++)
        {
            if(ValueStorage.value.weaponValue == WeaponStorage.value.weapons[i].name || ValueStorage.value.weaponValue == WeaponStorage.value.weapons[i].name + "(Clone)")
            {
                GameObject weaponToEquip = Instantiate(WeaponStorage.value.weapons[i], transform.position, Quaternion.identity);
                EquipWeapon(weaponToEquip.GetComponent<Weapon>());
                weaponToEquip.GetComponent<Weapon>().LoadData();
            }

            if(ValueStorage.value.secondWeaponValue == WeaponStorage.value.weapons[i].name || ValueStorage.value.secondWeaponValue == WeaponStorage.value.weapons[i].name + "(Clone)")
            {
                if(!((ValueStorage.value.weaponValue == "PISTOL" || ValueStorage.value.weaponValue == "PISTOL(Clone)") && (ValueStorage.value.secondWeaponValue == "PISTOL" || ValueStorage.value.secondWeaponValue == "PISTOL(Clone)")))
                {
                    GameObject secondWeaponToEquip = Instantiate(WeaponStorage.value.weapons[i], transform.position, Quaternion.identity);
                    SecondWeapon(secondWeaponToEquip.GetComponent<Weapon>());
                    secondWeaponToEquip.GetComponent<Weapon>().LoadData();
                }
            }
        }
    }

    void Update()
    {
        if (focusedWeapon != null && Input.GetKeyDown(KeyCode.E))
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

        if (hasTwoWeapons)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                SwitchWeapon(secondEquippedWeapon, equippedWeapon);
            }
        }

        if(equippedWeapon != null)
        {
            equippedWeapon.transform.position = holster.position;
        }

        if(secondEquippedWeapon != null)
        {
            secondEquippedWeapon.transform.position = secondHolster.position;
        }
    }

    void EquipWeapon(Weapon weaponToEquip)
    {
        if(weaponToEquip.transform.parent != transform && weaponToEquip.transform.parent != null)
        {
            Destroy(weaponToEquip.transform.parent.gameObject, 0.05f);
        }

        ValueStorage.value.weaponValue = weaponToEquip.GetComponent<Weapon>().weaponName;
        hasWeapon = true;
        equippedWeapon = weaponToEquip;
        equippedWeapon.transform.position = holster.transform.position;
        equippedWeapon.transform.parent = transform; //
        equippedWeapon.transform.localRotation = Quaternion.identity;
        equippedWeapon.GetComponent<BoxCollider2D>().enabled = false;
        equippedWeapon.GetComponent<Weapon>().playerWeapon = true;
        equippedWeapon.GetComponent<Weapon>().onOff = true;
        equippedWeapon.GetComponent<Weapon>().withPlayer = true;
        equippedWeapon.GetComponent<SpriteRenderer>().sortingOrder = 6;
        //Instantiate(pickupFX, transform.position, Quaternion.identity);
    }

    void DisequipWeapon(Weapon weaponToDisequip)
    {
        disequippedWeapon = weaponToDisequip;
        disequippedWeapon.transform.parent = null;
        disequippedWeapon.transform.localRotation = Quaternion.identity;
        equippedWeapon.GetComponent<BoxCollider2D>().enabled = true;
        disequippedWeapon.GetComponent<Weapon>().playerWeapon = false;
        disequippedWeapon.GetComponent<Weapon>().onOff = false;
        equippedWeapon.GetComponent<Weapon>().withPlayer = false;
    }

    void SecondWeapon(Weapon secondWeapon)
    {
        ValueStorage.value.secondWeaponValue = secondWeapon.GetComponent<Weapon>().weaponName;
        hasTwoWeapons = true;
        secondEquippedWeapon = secondWeapon;
        secondEquippedWeapon.transform.position = secondHolster.transform.position;
        secondEquippedWeapon.transform.parent = transform; //
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
