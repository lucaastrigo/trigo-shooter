using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LayerMask hitable;

    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if(Physics2D.Raycast(transform.position, transform.up))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 500, hitable);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, hit.point);
        }
    }
}
