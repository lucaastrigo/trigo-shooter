using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRange : MonoBehaviour
{
    public float destroySpeed;

    private void Update()
    {
        Vector3 size = transform.localScale;
        size.x -= destroySpeed * Time.deltaTime;
        size.y = size.x;

        transform.localScale = size;

        if(size.x <= 0)
        {
            Destroy(gameObject);
        }
    }
}
