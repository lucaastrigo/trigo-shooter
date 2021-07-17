using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosive : MonoBehaviour
{
    public int damage;
    public float pursueRange, pursueSpeed, explosionRange;
    public GameObject explosionFX;

    float normalSpeed;
    GameObject player;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        normalSpeed = GetComponent<Enemy>().speed;
    }

    void Update()
    {
        if(GetComponent<MovementEnemy>() != null && GetComponent<MovementEnemy>().distToPlayer <= pursueRange)
        {
            GetComponent<Enemy>().speed = pursueSpeed;
        }
        else
        {
            GetComponent<Enemy>().speed = normalSpeed;
        }
    }

    public void Explode()
    {
        if (GetComponent<Explosive>() != null)
        {
            GameObject explo = GetComponent<Explosive>().explosionArea;
            GameObject explos = Instantiate(explo, transform.position, Quaternion.identity);
            explos.transform.localScale = new Vector3(explosionRange, explosionRange, explosionRange);
        }

        Camera.main.GetComponent<CameraScript>().Shake(0.01f, 0.05f);

        Collider2D[] hitExplosion = Physics2D.OverlapCircleAll(transform.position, explosionRange);

        foreach (Collider2D thing in hitExplosion)
        {
            if (thing.GetComponent<Player>() != null)
            {
                thing.GetComponent<Player>().TakeDamage(damage);
            }

            if (thing.gameObject.CompareTag("Wall"))
            {
                Destroy(thing.gameObject);
            }
        }

        Instantiate(explosionFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, pursueRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
