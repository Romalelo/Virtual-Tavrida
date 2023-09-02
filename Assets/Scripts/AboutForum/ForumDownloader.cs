using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForumDownloader : MonoBehaviour
{
    public GameObject ForumTemplate, ClickedButton;
    public static bool isFromForum;

    void Start()
    {
        isFromForum = true;
        CodeEnter.isFromCodeEnter = false;
        FavouriteAROpen.arOpenedFromFavourites = false;
        //ForumTemplate.transform.GetChild(2).GetComponent<Image>().sprite = AboutForumDownloader.sprite;
        PlayerPrefs.SetString(AboutForumDownloader.title + "Start", DateTime.Now.ToString());
        ForumTemplate.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = AboutForumDownloader.title;
        ForumTemplate.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = AboutForumDownloader.description;
        ForumTemplate.transform.GetChild(1).GetChild(0).GetChild(3).GetChild(1).GetComponent<TMP_Text>().text = AboutForumDownloader.dates;
        ForumTemplate.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
    }

    private void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            ForumTemplate.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
        }
    }
}
