using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public GameObject explosionArea;
    public Color color;

    void Start()
    {
        explosionArea.transform.localScale = new Vector3(GetComponent<BulletScript>().explosionRange, GetComponent<BulletScript>().explosionRange, GetComponent<BulletScript>().explosionRange);
        explosionArea.GetComponent<SpriteRenderer>().color = color;
    }

    void Update()
    {
        //
    }
}
