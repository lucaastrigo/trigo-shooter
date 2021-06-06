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

        player.GetComponent<Player>().TakeDamage(100);
    }

    public void ReturnToMenu()
    {
        Continue();

        ValueStorage.value.healthValue = 10;
        ValueStorage.value.maxHealthValue = 10;
        ValueStorage.value.weaponValue = "PISTOL";
        ValueStorage.value.coinValue = 0;

        for (int i = 0; i <= valueStorage.GetComponent<ValueStorage>().WeaponAmmo.Count - 1; i++)
        {
            ValueStorage.value.WeaponAmmo[i] = 1000;
        }

        for (int i = valueStorage.GetComponent<ValueStorage>().WeaponAmmo.Count - 1; i > 0; i--)
        {
            ValueStorage.value.WeaponAmmo.RemoveAt(i);
        }

        //reset skills
        for (int i = 0; i < SkillStorage.value.skills.Length; i++)
        {
            SkillStorage.value.skills[i].skillOn = false;
            SkillStorage.value.skills[i].skilled = false;
            SkillStorage.value.skills[i].unskilled = false;
        }

        SceneManager.LoadScene("Main Menu");
    }
}
