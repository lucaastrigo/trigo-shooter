using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsFX : MonoBehaviour
{
    float time = 0.5f;
    SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sprite.color = new Color(0, 0, 0, time);

        if (time <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            time -= Time.deltaTime / 2.5f;
        }
    }
}
