using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class Healthier : MonoBehaviour
{
    public GameObject title, description;

    bool inTrigger;
    Player player;
    Light2D l;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        l = GetComponent<Light2D>();
    }

    void Update()
    {
        if (inTrigger)
        {
            title.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));

            if (Input.GetKeyDown(KeyCode.E))
            {
                ActivateSkill();
            }
        }

        title.SetActive(inTrigger);
        description.SetActive(inTrigger);
        l.enabled = inTrigger;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inTrigger = false;
        }
    }

    [Header("Skill")]
    public int increaseValue;

    void ActivateSkill()
    {
        Instantiate(Resources.Load("Particle FX/Color Effects/RedFX"), transform.position, Quaternion.identity);

        player.health += increaseValue;
        player.MoreHealth(increaseValue);

        player.healthBar.GetComponent<HealthBar>().SetHealth(player.currentHealth);
        player.healthBar.GetComponent<HealthBar>().SetMaxHealth(player.health);

        GetComponent<DestroyBrothers>().DestroyBros();
    }
}
