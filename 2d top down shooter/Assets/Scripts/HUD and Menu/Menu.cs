using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Menu : MonoBehaviour
{
    GameObject valueStorage;

    private void Update()
    {
        if (valueStorage == null)
        {
            valueStorage = GameObject.FindGameObjectWithTag("Value Storage");
        }
    }

    public void Print(string namee)
    {
        print(namee);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        ValueStorage.value.healthValue = 10;
        ValueStorage.value.maxHealthValue = 10;
        ValueStorage.value.weaponValue = "PISTOL";
        ValueStorage.value.coinValue = 0;
    }

    public void Feedback()
    {
        Application.OpenURL("https://youtube.com");
    }

    public void MainMenu()
    {
        ValueStorage.value.healthValue = 10;
        ValueStorage.value.maxHealthValue = 10;
        ValueStorage.value.weaponValue = "PISTOL";
        ValueStorage.value.coinValue = 0;

        //reset skills
        for (int i = 0; i < SkillStorage.value.skills.Length; i++)
        {
            SkillStorage.value.skills[i].skillOn = false;
            SkillStorage.value.skills[i].skilled = false;
            SkillStorage.value.skills[i].unskilled = false;
        }

        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
