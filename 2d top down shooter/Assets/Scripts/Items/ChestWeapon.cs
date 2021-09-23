using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestWeapon : MonoBehaviour
{
    [System.Serializable]
    public class DropItem
    {
        public int dropRate;
        public GameObject weapon;
    }

    public List<DropItem> DropTable = new List<DropItem>();

    public bool mergedChest;
    public Sprite openedSprite;
    public AudioClip clip;

    bool open;
    [HideInInspector] public GameObject weaponToDrop;
    SpriteRenderer sprite;
    GameObject player;
    AudioSource aud;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        aud = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!mergedChest)
        {
            if (player.GetComponentInChildren<Weapon>() != null)
            {
                for (int i = 0; i < DropTable.Count; i++)
                {
                    if (DropTable[i].weapon.GetComponent<Weapon>().weaponName == player.GetComponentInChildren<Weapon>().weaponName)
                    {
                        DropTable[i].weapon.GetComponent<Weapon>().cantDrop = true;
                    }
                    else
                    {
                        DropTable[i].weapon.GetComponent<Weapon>().cantDrop = false;
                    }
                }
            }
        }
    }

    public void CalculateDrop()
    {
        int itemWeight = 0;

        for (int i = 0; i < DropTable.Count; i++)
        {
            itemWeight += DropTable[i].dropRate;
        }

        int randomValue = Random.Range(0, itemWeight);

        for (int j = 0; j < DropTable.Count; j++)
        {
            if (!mergedChest)
            {
                if (randomValue <= DropTable[j].dropRate && !DropTable[j].weapon.GetComponent<Weapon>().cantDrop)
                {
                    weaponToDrop = DropTable[j].weapon;
                    break;
                }
                else
                {
                    randomValue -= DropTable[j].dropRate;
                }
            }
            else
            {
                if (randomValue <= DropTable[j].dropRate) // && !DropTable[j].weapon.GetComponent<Weapon>().cantDrop
                {
                    weaponToDrop = DropTable[j].weapon;
                    break;
                }
                else
                {
                    randomValue -= DropTable[j].dropRate;
                }
            }
        }
    }

    void Drop()
    {
        if(weaponToDrop.GetComponent<SpriteRenderer>() != null)
        {
            weaponToDrop.GetComponent<SpriteRenderer>().sortingOrder = 6;
        }
        else if(weaponToDrop.GetComponentInChildren<SpriteRenderer>() != null)
        {
            weaponToDrop.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6;
        }

        Instantiate(weaponToDrop, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
        Instantiate(Resources.Load("Particle FX/WeaponChestFX"), transform.position, Quaternion.identity);
        aud.PlayOneShot(clip);

        open = true;
        sprite.sprite = openedSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !open)
        {
            CalculateDrop();
            Drop();
        }
    }
}
