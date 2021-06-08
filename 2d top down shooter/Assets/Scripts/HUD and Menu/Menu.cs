using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
    }

    public void Feedback()
    {
        Application.OpenURL("https://forms.gle/aAXk1c1PKSdmVzNK6");
    }

    public void MainMenu()
    {
        ValueStorage.value.healthValue = 10;
        ValueStorage.value.maxHealthValue = 10;
        ValueStorage.value.weaponValue = "PISTOL";
        ValueStorage.value.coinValue = 0;

        for (int i = valueStorage.GetComponent<ValueStorage>().WeaponAmmo.Count - 1; i > 0; i--)
        {
            ValueStorage.value.WeaponAmmo.RemoveAt(i);
        }

        for (int i = 0; i <= valueStorage.GetComponent<ValueStorage>().WeaponAmmo.Count - 1; i++)
        {
            ValueStorage.value.WeaponAmmo[i] = 10000;
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
