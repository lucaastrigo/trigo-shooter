using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBrothers : MonoBehaviour
{
    public void DestroyBros()
    {
        if(transform.parent != null)
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                Destroy(transform.parent.GetChild(i).gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
