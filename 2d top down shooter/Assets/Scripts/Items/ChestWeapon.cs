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
    public GameObject openFX;

    bool open;
    [HideInInspector] public GameObject weaponToDrop;
    SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void CalculateDrop()
    {
        int itemWeight = 0;

        for (int i = 0; i < DropTable.Count; i++)
        {
            itemWeight += DropTable[i].dropRate; //item weight equal to number of weapons in chest
        }

        int randomValue = Random.Range(0, itemWeight); //generates a random value between 0 and number of weapons

        for (int j = 0; j < DropTable.Count; j++)
        {
            if (randomValue <= DropTable[j].dropRate) //check if random value is equal or less than weapon 'j' drop rate 
            {
                weaponToDrop = DropTable[j].weapon; //set weaponToDrop to the weapon 'j'
                break; //stops random value from subtracting
            }
            else //but if random value is bigger
            {
                randomValue -= DropTable[j].dropRate; //subtract the previous weapon drop rate from random value 
            }
        }
    }

    void Drop()
    {
        Instantiate(weaponToDrop, new Vector2(transform.position.x, transform.position.y + 0.1f), Quaternion.identity);
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
