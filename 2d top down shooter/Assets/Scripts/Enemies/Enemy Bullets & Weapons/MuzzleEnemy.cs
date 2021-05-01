using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleEnemy : MonoBehaviour
{
    float angle;
    GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        Vector3 difference = player.transform.position - transform.position;
        angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
