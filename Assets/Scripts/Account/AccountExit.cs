using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AccountExit : MonoBehaviour
{
    public GameObject exitScreen;
    public void AccountExitButton()
    {
        exitScreen.SetActive(true);
        exitScreen.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = $"Вы уверены, что хотите выйти из аккаунта {PlayerPrefs.GetString("Username")}?";
    }

    public void ExitButton()
    {
        PlayerPrefs.DeleteAll();
        Scenes.LoginPage();
    }

    public void CloseButton()
    {
        exitScreen.SetActive(false);
    }
}
