using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class AmmoExpert : MonoBehaviour
{
    public GameObject title, description;

    bool inTrigger;
    Light2D l;

    void Start()
    {
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
    public float increaseValue;

    void ActivateSkill()
    {
        Instantiate(Resources.Load("Particle FX/Color Effects/YellowFX"), transform.position, Quaternion.identity);

        SkillStorage.value.ammoExpert += increaseValue;

        GetComponent<DestroyBrothers>().DestroyBros();
    }
}
