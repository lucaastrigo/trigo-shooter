using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailFX : MonoBehaviour
{
    public float timeBtw;
    public GameObject trailObject;
    public Transform feet;

    float time;
    PlayerMovement player;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if(player.xMove != 0 || player.yMove != 0)
        {
            if (time <= 0)
            {
                Instantiate(trailObject, feet.transform.position, Quaternion.identity);
                time = timeBtw;
            }
            else
            {
                time -= Time.deltaTime;
            }
        }
    }
}
