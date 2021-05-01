using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerEnemy : MonoBehaviour
{
    public GameObject[] spawnables;

    public void Spawn()
    {
        for (int i = 0; i < spawnables.Length; i++)
        {
            Instantiate(spawnables[i], transform.position, Quaternion.identity);
        }
    }
}
