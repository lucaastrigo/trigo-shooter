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

    [Header("Image Setting")]
    public Sprite openedSprite;
    public GameObject openFX, minimapImage;

    bool open;
    [HideInInspector] public GameObject weaponToDrop;
    SpriteRenderer sprite;
    GameObject player;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if(player.GetComponentInChildren<Weapon>() != null)
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

        Instantiate(weaponToDrop, new Vector2(transform.position.x, transform.position.y + 0.1f), Quaternion.identity);
        Instantiate(openFX, transform.position, Quaternion.identity);

        if (minimapImage != null)
        {
            Destroy(minimapImage);
        }

        Instantiate(openFX, transform.position, Quaternion.identity);
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
