using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StartPage : MonoBehaviour
{
    private string username, password, uri;

    public void Start()
    {
        ApplicationChrome.statusBarState = ApplicationChrome.navigationBarState = ApplicationChrome.States.TranslucentOverContent;
        Application.targetFrameRate = 60;
        uri = "http://193.124.118.62/api/1/Auth/Login";
        username = PlayerPrefs.GetString("Username");
        password = PlayerPrefs.GetString("Password");
        if (username != "" && password != "")
        {
            StartCoroutine(GetRequest(uri, username, password));
        }
        else if (PlayerPrefs.GetString("UserToken") == "")
        {
            StartCoroutine(GetNoName());
        }
        else
        {
            Scenes.QRPage();
        }
        Screen.fullScreen = false;
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
            Debug.Log("Выполнен вход");
            Scenes.QRPage();
        }
        else
        {
            Scenes.QRPage();
            Debug.Log(www.error + ": " + www.result);
        }
    }

    IEnumerator GetNoName()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://193.124.118.62/api/1.0/auth/noname");

        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);

        PlayerPrefs.SetString("UserToken", www.downloadHandler.text);
        Scenes.QRPage();
    }

    [System.Serializable]
    public class User
    {
        public string email;
        public string password;
    }
}
