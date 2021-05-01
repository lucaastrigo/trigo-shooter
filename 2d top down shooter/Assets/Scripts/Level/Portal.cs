using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string scene;
    public float pressTime;
    //public GameObject portalHUD;

    [HideInInspector] public float time;
    [HideInInspector] public bool inPortal;
    GameObject player;

    private void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        //portalHUD.SetActive(inPortal);

        if (inPortal)
        {
            if (Input.GetKey(KeyCode.E))
            {
                time += Time.deltaTime;
                Camera.main.GetComponent<CameraScript>().Shake(time / 500, 0.05f);
            }
            else
            {
                time -= Time.deltaTime;
                Camera.main.GetComponent<CameraScript>().StopShake();
            }

            //portalHUD.transform.position = Camera.main.WorldToScreenPoint(player.transform.position);
            //GetComponentInChildren<PortalHUD>().SetTime(time);
            //GetComponentInChildren<PortalHUD>().SetMaxTime(pressTime);
        }
        else
        {
            time = 0;
        }

        if(time >= pressTime)
        {
            SceneManager.LoadScene(scene);

        }else if(time <= 0)
        {
            time = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inPortal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inPortal = false;
        }
    }
}
