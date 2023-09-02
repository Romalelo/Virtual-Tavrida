using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Search : MonoBehaviour
{
    public GameObject search, ordinaryMode, targetObj;
    public RectTransform scrollRect, forumsHandler;
    public static bool isSearch = false, isFavourite = false;
    private ForumsLoader actionForumTarget;
    private FavouriteDownloader actionFavouriteTarget;

    public void SearchEnter()
    {
        search.SetActive(true);
        ordinaryMode.SetActive(false);
        scrollRect.sizeDelta = new Vector2(0f, -75f);
        forumsHandler.sizeDelta = new Vector2(0f, forumsHandler.sizeDelta.y + 75f);
        isSearch = true;
    }

    public void SearchExit()
    {
        Scenes.ForumListPage();
        isSearch = false;
    }

    public void FavouriteSearchExit()
    {
        Scenes.Favourites();
        isSearch = false;
    }

    public void ForumsReadStringInput(string s)
    {
        actionForumTarget = targetObj.GetComponent<ForumsLoader>();
        isFavourite = false;
        StopAllCoroutines();
        StartCoroutine(StartSearch($"http://193.124.118.62/api/1.0/forum/list/{s}"));
    }

    public void ReadStringInput(string s)
    {
        actionFavouriteTarget = targetObj.GetComponent<FavouriteDownloader>();
        isFavourite = true;
        StopAllCoroutines();
        StartCoroutine(StartSearch($"http://193.124.118.62/api/1.0/model/favorites/{s}"));
    }


    IEnumerator StartSearch(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        www.SetRequestHeader("accept", "*/*");
        www.SetRequestHeader("Content-Type", "text/json");
        www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("UserToken")}");

        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            if (isFavourite)
            {
                Debug.Log(url);
                actionFavouriteTarget.StartLoader(url);
            }
            else
            {
                Debug.Log(url);
                actionForumTarget.StartLoader(url);
            }
        }
        else
        {
            Debug.Log(www.error + ":" + www.result);
        }
    }
}
