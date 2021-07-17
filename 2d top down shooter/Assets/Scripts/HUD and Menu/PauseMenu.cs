using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool paused;
    public static bool mapped;

    public GameObject pauseMenu;
    public GameObject UI;

    GameObject player, valueStorage;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (valueStorage == null)
        {
            valueStorage = GameObject.FindGameObjectWithTag("Value Storage");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }
    }

    public void Pause()
    {
        paused = true;
        pauseMenu.SetActive(true);
        UI.SetActive(false);
        Time.timeScale = 0f;

        player.GetComponent<Player>().enabled = false;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<PlayerPickup>().enabled = false;
    }

    public void Continue()
    {
        paused = false;
        pauseMenu.SetActive(false);
        UI.SetActive(true);
        Time.timeScale = 1f;

        player.GetComponent<Player>().enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<PlayerPickup>().enabled = true;
    }

    public void Retry()
    {
        Continue();

        player.GetComponent<Player>().TakeDamage(1000);
    }

    public void ReturnToMenu()
    {
        Continue();

        player.GetComponent<Player>().TakeDamage(1000);
    }
}
