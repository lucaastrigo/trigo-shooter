using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public int health; 
    public int currentHealth;
    [HideInInspector] public GameObject healthBar;
    public GameObject indicator;
    public Transform canvas;
    public Color green, red, yellow;

    [HideInInspector] public int coins;
    [HideInInspector] public float time;
    [HideInInspector] public float indicationTime = 1f;
    GameObject healthText;
    GameObject valueStorage, skillStorage;
    [HideInInspector] public GameObject[] aaplace;
    Rigidbody2D rb;
    Animator anim;
    TextMeshProUGUI text;

    private void Start()
    {
        currentHealth = ValueStorage.value.healthValue;
        health = ValueStorage.value.maxHealthValue;

        healthBar = GameObject.Find("Health Bar");
        healthBar.GetComponent<HealthBar>().SetHealth(ValueStorage.value.healthValue);
        healthBar.GetComponent<HealthBar>().SetMaxHealth(ValueStorage.value.maxHealthValue);

        healthText = GameObject.Find("Health Text");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        coins = ValueStorage.value.coinValue;

        text = GameObject.Find("text money").GetComponent<TextMeshProUGUI>();

        aaplace = GameObject.FindGameObjectsWithTag("AA");
    }

    private void Update()
    {
        if(valueStorage == null)
        {
            valueStorage = GameObject.FindGameObjectWithTag("Value Storage");
        }

        if(skillStorage == null)
        {
            skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
        }

        ValueStorage.value.healthValue = currentHealth;
        ValueStorage.value.maxHealthValue = health;
        ValueStorage.value.coinValue = coins;

        text.text = "$" + coins.ToString();

        healthText.GetComponent<TMP_Text>().text = currentHealth.ToString() + "/" + health.ToString();

        if(currentHealth >= health)
        {
            currentHealth = health;
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        for (int i = 0; i < skillStorage.GetComponent<SkillStorage>().skills.Length; i++)
        {
            if (skillStorage.GetComponent<SkillStorage>().skills[i].GetComponent<Skill>().activeSkill && skillStorage.GetComponent<SkillStorage>().skills[i].GetComponent<Skill>().skillOn)
            {
                if (skillStorage.GetComponent<SkillStorage>().skills[i].GetComponent<Skill>().hasCooldown)
                {
                    aaplace[1].GetComponent<Image>().enabled = true;
                    aaplace[1].GetComponent<Image>().sprite = skillStorage.GetComponent<SkillStorage>().skillObjects[i].GetComponent<SpriteRenderer>().sprite;
                }

                aaplace[0].GetComponent<Image>().enabled = true;
                aaplace[0].GetComponent<Image>().sprite = skillStorage.GetComponent<SkillStorage>().skillObjects[i].GetComponent<SpriteRenderer>().sprite;
                break;
            }
            else
            {
                aaplace[0].GetComponent<Image>().enabled = false;
                aaplace[1].GetComponent<Image>().enabled = false;
            }
        }
    }

    public void MoreHealth(int healthAmount)
    {
        currentHealth += healthAmount;

        healthBar.GetComponent<HealthBar>().SetHealth(currentHealth);

        Vector3 indicatorPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
        GameObject ind;

        ind = Instantiate(indicator, indicatorPos, Quaternion.identity);
        ind.transform.SetParent(canvas);
        ind.GetComponent<Animator>().SetTrigger("active");
        ind.GetComponent<TMP_Text>().color = green;
        ind.GetComponent<TMP_Text>().text = healthAmount.ToString();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.GetComponent<HealthBar>().SetHealth(currentHealth);

        anim.SetTrigger("hurt");

        Vector3 indicatorPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
        GameObject ind;

        ind = Instantiate(indicator, indicatorPos, Quaternion.identity);
        ind.transform.SetParent(canvas);
        ind.GetComponent<Animator>().SetTrigger("active");
        ind.GetComponent<TMP_Text>().color = red;
        ind.GetComponent<TMP_Text>().text = damage.ToString();
    }

    public void Purchase(int price)
    {
        coins -= price;
    }

    public void Receive(int coinAmount)
    {
        coins += coinAmount;

        Vector3 indicatorPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
        GameObject ind;

        ind = Instantiate(indicator, indicatorPos, Quaternion.identity);
        ind.transform.SetParent(canvas);
        ind.GetComponent<Animator>().SetTrigger("active");
        ind.GetComponent<TMP_Text>().color = yellow;
        ind.GetComponent<TMP_Text>().text = coinAmount.ToString();
    }

    public void Die()
    {
        ValueStorage.value.healthValue = 10;
        ValueStorage.value.maxHealthValue = 10;
        ValueStorage.value.weaponValue = "PISTOL";
        ValueStorage.value.secondWeaponValue = null;
        ValueStorage.value.coinValue = 0;

        //reset skills
        for (int i = 0; i < SkillStorage.value.skills.Length; i++)
        {
            SkillStorage.value.skills[i].skillOn = false;
            SkillStorage.value.skills[i].skilled = false;
            SkillStorage.value.skills[i].unskilled = false;
        }

        SceneManager.LoadScene("Main Menu");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(GetComponentInChildren<PlayerPlunger>() != null)
        {
            for (int i = 0; i < GetComponentInChildren<PlayerPlunger>().layers.Length; i++)
            {
                if (collision.gameObject.layer == GetComponentInChildren<PlayerPlunger>().layers[i])
                {
                    GetComponentInChildren<PlayerPlunger>().Unhook();
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (GetComponentInChildren<PlayerPlunger>() != null)
        {
            for (int i = 0; i < GetComponentInChildren<PlayerPlunger>().layers.Length; i++)
            {
                if (collision.gameObject.layer == GetComponentInChildren<PlayerPlunger>().layers[i])
                {
                    GetComponentInChildren<PlayerPlunger>().Unhook();
                }
            }
        }
    }
}
