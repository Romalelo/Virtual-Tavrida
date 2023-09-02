using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Globalization;
using UnityEngine.Networking;
using System.Net;
using System.Text;
using UnityEngine.Android;

public class Login : MonoBehaviour
{

    private string email, password;
    public TMP_InputField emailInputField, passwordInputField;
    private bool isShown = false, isGood = true;
    public GameObject emailObject, passwordObject, showPasswordObject, badSource, goodSource, badSourceShow, goodSourceShow, 
        badTxt, badSourceUnshown, goodSourceUnshown;

    public void LoginButton()
    {
        string uri = "http://193.124.118.62/api/1/Auth/Login";
        email = emailInputField.text;
        password = passwordInputField.text;
        if (EmailAndPasswordCheck())
        {
            StartCoroutine(GetRequest(uri, email, password));
        }
    }


    IEnumerator GetRequest(string uri, string email, string password)
    {
        User userInstance = new User();
        userInstance.email = email;
        userInstance.password = password;
        string userToJson = JsonUtility.ToJson(userInstance, true);
        Debug.Log(userToJson);

        UnityWebRequest www = UnityWebRequest.PostWwwForm(uri, userToJson);

        www.SetRequestHeader("accept", "*/*");
        www.SetRequestHeader("Content-Type", "text/json");
        byte[] userRaw = Encoding.UTF8.GetBytes(userToJson);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(userRaw);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
            userInstance.token = www.downloadHandler.text;
            PlayerPrefs.SetString("UserToken", userInstance.token);
            PlayerPrefs.SetString("Username", userInstance.email);
            PlayerPrefs.SetString("Password", userInstance.password);
            Debug.Log(userInstance.token);
            Scenes.ForumListPage();
        }
        else
        {
            Debug.Log(www.error + ": " + www.result);
            badTxt.GetComponent<TMP_Text>().text = "Пользователь с такими данными не зарегистрирован";
            badTxt.SetActive(true);
            emailObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            passwordObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            showPasswordObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            isGood = false;
            if (isShown)
            {
                showPasswordObject.GetComponent<Image>().sprite = badSourceUnshown.GetComponent<Image>().sprite;
            }
            else
            {
                showPasswordObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            }
        }
    }

    public bool EmailAndPasswordCheck()
    {
        if (email.Length == 0)
        {
            badTxt.GetComponent<TMP_Text>().text = "Логин не введён";
            badTxt.SetActive(true);
            emailObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            isGood = false;
            return false;
        }
        if (password.Length < 8)
        {
            badTxt.GetComponent<TMP_Text>().text = "Длина пароля должна быть от 8 до 64 символов";
            badTxt.SetActive(true);
            passwordObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            isGood = false;
            if (isShown)
            {
                showPasswordObject.GetComponent<Image>().sprite = badSourceUnshown.GetComponent<Image>().sprite;
            }
            else
            {
                showPasswordObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            }
            return false;
        }
        return true;
    }

    public void OnValueChange()
    {
        badTxt.SetActive(false);
        emailObject.GetComponent<Image>().sprite = goodSource.GetComponent<Image>().sprite;
        passwordObject.GetComponent<Image>().sprite = goodSource.GetComponent<Image>().sprite;
        if (isShown)
        {
            showPasswordObject.GetComponent<Image>().sprite = goodSourceUnshown.GetComponent<Image>().sprite;
        }
        else
        {
            showPasswordObject.GetComponent<Image>().sprite = goodSourceShow.GetComponent<Image>().sprite;
        }
        isGood = true;
    }

    public void ShowPassword()
    {
        if (isShown)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
            isShown = false;
            if (isGood)
            {
                showPasswordObject.GetComponent<Image>().sprite = goodSourceShow.GetComponent<Image>().sprite;
            }
            else
            {
                showPasswordObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            }
        }
        else
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            isShown = true;
            if (isGood)
            {
                showPasswordObject.GetComponent<Image>().sprite = goodSourceUnshown.GetComponent<Image>().sprite;
            }
            else
            {
                showPasswordObject.GetComponent<Image>().sprite = badSourceUnshown.GetComponent<Image>().sprite;
            }
        }
        passwordInputField.ForceLabelUpdate();
    }

    [System.Serializable]
    public class User
    {
        public string email;
        public string password;
        public string token;
    }
}
