using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlunger : MonoBehaviour
{
    public int[] layers;
    public float fireRate, minDistance;

    [HideInInspector] public float hookTime;
    float angle, fireTime;
    DistanceJoint2D dj;
    LineRenderer lr;

    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;

        dj = GetComponentInParent<DistanceJoint2D>();
        dj.enabled = false;

        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    void Update()
    {
        if (fireTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                RaycastHit2D hit = Physics2D.Linecast(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.parent.position);

                if (hit.collider != null)
                {
                    for (int i = 0; i < layers.Length; i++)
                    {
                        if (hit.collider.gameObject.layer == layers[i])
                        {
                            Hook(hit.collider.gameObject.transform.position);
                        }
                        else
                        {
                            Hook(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                        }
                    }
                }
            }
        }
        else
        {
            fireTime -= Time.deltaTime;
        }

        if (lr.enabled)
        {
            lr.SetPosition(0, transform.parent.position);
        }

        if (dj.enabled)
        {
            if (Vector2.Distance(transform.position, transform.parent.position) <= dj.distance + minDistance)
            {
                Unhook();
            }
            else
            {
                if (hookTime <= 0)
                {
                    Unhook();
                }
                else
                {
                    hookTime -= Time.deltaTime;
                }
            }
        }
        else
        {
            Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    public void Hook(Vector2 hookPos)
    {
        fireTime = fireRate;
        hookTime = Vector2.Distance(transform.position, transform.parent.position) / 10;

        transform.position = hookPos;
        GetComponent<SpriteRenderer>().enabled = true;

        dj.enabled = true;
        lr.enabled = true;

        lr.SetPosition(1, hookPos);
    }

    public void Unhook()
    {
        dj.enabled = false;
        lr.enabled = false;

        GetComponent<SpriteRenderer>().enabled = false;
        transform.position = transform.parent.position;
    }
}
