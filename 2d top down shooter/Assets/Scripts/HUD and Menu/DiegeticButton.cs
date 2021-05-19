using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine;

public class DiegeticButton : MonoBehaviour
{
    [SerializeField] UnityEvent _event;

    void Start()
    {
        //
    }

    void Update()
    {
        //
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<BulletScript>() != null)
        {
            _event.Invoke();
        }
    }
}
