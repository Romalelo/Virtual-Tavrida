using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForumVisitToHistory : MonoBehaviour
{
    public static string id;

    public void ForumVisited()
    {
        id = AboutForumDownloader.id;

        StartCoroutine(PostForumToHistory($"http://193.124.118.62/api/1.0/forum/{id}"));
    }

    IEnumerator PostForumToHistory(string url)
    {
        UnityWebRequest www = UnityWebRequest.PostWwwForm(url, id);

        www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("UserToken")}");

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Форум успешно добавлен в историю посещений");
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}
