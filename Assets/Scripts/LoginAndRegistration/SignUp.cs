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

public class SignUp : MonoBehaviour
{
    private string email, password, passwordRepeated;
    public TMP_InputField emailInputField, passwordInputField, passwordRepeatedInputField;
    private bool isShown = false, isShownRepeated = false, isPasswordGood = true, isRepeatedPasswordGood = true;
    public GameObject emailObject, passwordObject, passwordRepeatedObject, showPasswordObject, showPasswordRepeatedObject, 
        badSource, goodSource, badSourceShow, goodSourceShow, badSourceUnshown, badTxt, goodSourceUnshown;

    public void SignUpButton()
    {
        string uri = "http://193.124.118.62/api/1.0/auth/update/noname";
        email = emailInputField.text;
        password = passwordInputField.text;
        passwordRepeated = passwordRepeatedInputField.text;
        Debug.Log(password);
        if (PasswordCheck())
        {
            StartCoroutine(GetRequest(uri, email, password));
        }

    }


    IEnumerator GetRequest(string uri, string email, string password)
    {
        User userInstance = new User();
        userInstance.userName = email;
        userInstance.email = email;
        userInstance.password = password;
        string userToJson = JsonUtility.ToJson(userInstance, true);
        Debug.Log(userToJson);

        UnityWebRequest www = UnityWebRequest.PostWwwForm(uri, userToJson);

        www.SetRequestHeader("accept", "*/*");
        www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("UserToken")}");
        www.SetRequestHeader("Content-Type", "text/json");
        byte[] userRaw = Encoding.UTF8.GetBytes(userToJson);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(userRaw);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
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
            badTxt.GetComponent<TMP_Text>().text = "Пользователь с такими данными уже зарегистрирован";
            badTxt.SetActive(true);
            emailObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            passwordObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            showPasswordObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            passwordRepeatedObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            showPasswordRepeatedObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            isPasswordGood = false;
            isRepeatedPasswordGood = false;
            if (isShown)
            {
                showPasswordObject.GetComponent<Image>().sprite = badSourceUnshown.GetComponent<Image>().sprite;
            }
            else
            {
                showPasswordObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            }
            if (isShownRepeated)
            {
                showPasswordRepeatedObject.GetComponent<Image>().sprite = badSourceUnshown.GetComponent<Image>().sprite;
            }
            else
            {
                showPasswordRepeatedObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            }
        }
    }

    public bool PasswordCheck()
    {
        if (email.Length < 5)
        {
            badTxt.GetComponent<TMP_Text>().text = "Введённая почта слишком короткая";
            badTxt.SetActive(true);
            emailObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            return false;
        }
        if (!email.Contains("@"))
        {
            badTxt.GetComponent<TMP_Text>().text = "Необходимо ввести почту";
            badTxt.SetActive(true);
            emailObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            return false;
        }
        if (email.Length == 0)
        {
            badTxt.GetComponent<TMP_Text>().text = "Логин не введён";
            badTxt.SetActive(true);
            emailObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            return false;
        }
        if (password.Contains(" "))
        {
            badTxt.GetComponent<TMP_Text>().text = "Пароль содержит недопустимые символы";
            badTxt.SetActive(true);
            passwordObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            showPasswordObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            isPasswordGood = false;
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
        if (password.Length < 8)
        {
            badTxt.GetComponent<TMP_Text>().text = "Длина пароля должна быть от 8 до 64 символов";
            badTxt.SetActive(true);
            passwordObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            showPasswordObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            isPasswordGood = false;
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
        if (password != passwordRepeated)
        {
            badTxt.GetComponent<TMP_Text>().text = "Пароли не совпадают";
            badTxt.SetActive(true);
            passwordRepeatedObject.GetComponent<Image>().sprite = badSource.GetComponent<Image>().sprite;
            showPasswordRepeatedObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            isRepeatedPasswordGood = false;
            if (isShownRepeated)
            {
                showPasswordRepeatedObject.GetComponent<Image>().sprite = badSourceUnshown.GetComponent<Image>().sprite;
            }
            else
            {
                showPasswordRepeatedObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
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
        passwordRepeatedObject.GetComponent<Image>().sprite = goodSource.GetComponent<Image>().sprite;
        showPasswordObject.GetComponent<Image>().sprite = goodSourceShow.GetComponent<Image>().sprite;
        showPasswordRepeatedObject.GetComponent<Image>().sprite = goodSourceShow.GetComponent<Image>().sprite;
        if (isShown)
        {
            showPasswordObject.GetComponent<Image>().sprite = goodSourceUnshown.GetComponent<Image>().sprite;
        }
        else
        {
            showPasswordObject.GetComponent<Image>().sprite = goodSourceShow.GetComponent<Image>().sprite;
        }
        if (isShownRepeated)
        {
            showPasswordRepeatedObject.GetComponent<Image>().sprite = goodSourceUnshown.GetComponent<Image>().sprite;
        }
        else
        {
            showPasswordRepeatedObject.GetComponent<Image>().sprite = goodSourceShow.GetComponent<Image>().sprite;
        }
        isPasswordGood = true;
        isRepeatedPasswordGood = true;
    }

    public void ShowPassword()
    {
        if (isShown)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
            isShown = false;
            if (isPasswordGood)
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
            if (isPasswordGood)
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

    public void ShowRepeatedPassword()
    {
        if (isShownRepeated)
        {
            passwordRepeatedInputField.contentType = TMP_InputField.ContentType.Password;
            isShownRepeated = false;
            if (isRepeatedPasswordGood)
            {
                showPasswordRepeatedObject.GetComponent<Image>().sprite = goodSourceShow.GetComponent<Image>().sprite;
            }
            else
            {
                showPasswordRepeatedObject.GetComponent<Image>().sprite = badSourceShow.GetComponent<Image>().sprite;
            }
        }
        else
        {
            passwordRepeatedInputField.contentType = TMP_InputField.ContentType.Standard;
            isShownRepeated = true;
            if (isRepeatedPasswordGood)
            {
                showPasswordRepeatedObject.GetComponent<Image>().sprite = goodSourceUnshown.GetComponent<Image>().sprite;
            }
            else
            {
                showPasswordRepeatedObject.GetComponent<Image>().sprite = badSourceUnshown.GetComponent<Image>().sprite;
            }
        }
        passwordRepeatedInputField.ForceLabelUpdate();
    }

    [System.Serializable]
    public class User
    {
        public string userName;
        public string email;
        public string password;
        public string token;
    }
}
