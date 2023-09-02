using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public static GameObject isLogined, favourite, account;

    public static void LoginPage()
    {
        SceneManager.LoadScene("LoginPage");
    }

    public static void SignUpPage()
    {
        SceneManager.LoadScene("SignUpPage");
    }

    public static void ForumListPage()
    {
        Debug.Log(PlayerPrefs.GetString("UserToken"));
        SceneManager.LoadScene("ForumListPage");
    }

    public static void AboutForumPage()
    {
        SceneManager.LoadScene("AboutForumPage");
    }

    public static void AccountPage()
    {
        if (PlayerPrefs.GetString("Username") == "")
        {
            SceneManager.LoadScene("Account 1");
        }
        else
        {
            SceneManager.LoadScene("Account");
        }
    }

    public static void Favourites()
    {
        if (PlayerPrefs.GetString("Username") == "")
        {
            SceneManager.LoadScene("Favourite 1");
        }
        else
        {
            SceneManager.LoadScene("Favourite");
        }
    }

    public static void QRPage()
    {
        SceneManager.LoadScene("QRCodeScanPage");
    }

    public static void ARPage() 
    {
        SceneManager.LoadScene("BlankAR");
    }

    public static void PrivacyPolicy()
    {
        Application.OpenURL("http://185.233.187.109/policy.html");
    }

    public static void ModelDownloadPage()
    {
        SceneManager.LoadScene("ModelLoaderPage");
    }
}
