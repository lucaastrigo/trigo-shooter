using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage;
    public float speed;
    public GameObject destroyEffect;

    GameObject skillStorage;

    private void Start()
    {
        skillStorage = GameObject.FindGameObjectWithTag("Skill Storage");
    }

    void Update()
    {
        if (skillStorage.GetComponentInChildren<SixthSenseSkill>().skill.GetComponent<Skill>().skillOn)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime * skillStorage.GetComponentInChildren<SixthSenseSkill>()._speedDecrease * Global.globalSpeed);
        }
        else
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime * Global.globalSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
    }
}
