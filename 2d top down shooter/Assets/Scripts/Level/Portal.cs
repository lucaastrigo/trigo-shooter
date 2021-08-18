using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string firstLevel, nextLevel;
    public bool scenePlus;

    int sceneNo;
    bool inTrigger;
    GameObject player;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        sceneNo = SceneManager.sceneCountInBuildSettings;
    }

    private void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if(inTrigger && Input.GetKeyDown(KeyCode.Q))
        {
            Stairs();
        }
    }

    void Stairs()
    {
        anim.SetTrigger("play");
        player.SetActive(false);
    }

    public void LoadScene()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadSceneAsync(nextLevel);

        /*
        if (scenePlus)
        {
            int sceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;

            if(sceneToLoad >= sceneNo)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
                SceneManager.LoadSceneAsync(firstLevel);
            }
            else
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
                SceneManager.LoadSceneAsync(sceneToLoad);
            }
        }
        else
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadSceneAsync(nextLevel);
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inTrigger = false;
        }
    }
}
