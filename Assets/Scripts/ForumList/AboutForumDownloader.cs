using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AboutForumDownloader : MonoBehaviour
{
    public GameObject card;
    public static string title, description, dates, id;
    public static List<string> mapUrls = new List<string>();
    public static List<string> imageUrls = new List<string>();
    public static Sprite sprite;
    public void OnButtonClicked()
    {
        mapUrls.Clear();
        imageUrls.Clear();
        sprite = card.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
        title = card.transform.GetChild(2).GetComponent<TMP_Text>().text;
        description = card.transform.GetChild(4).GetComponent<TMP_Text>().text;
        dates = card.transform.GetChild(6).GetComponent<TMP_Text>().text;
        id = card.transform.GetChild(7).GetComponent<TMP_Text>().text;
        int mapUrlsSize;
        mapUrlsSize = card.transform.GetChild(9).childCount;
        for(int i = 0; i < mapUrlsSize; i++)
        {
            mapUrls.Add(card.transform.GetChild(9).GetChild(i).GetComponent<TMP_Text>().text);
            Debug.Log(mapUrls.ToArray()[i]);
        }

        int imageUrlsSize;
        imageUrlsSize = card.transform.GetChild(10).childCount;
        for (int i = 0; i < imageUrlsSize; i++)
        {
            imageUrls.Add(card.transform.GetChild(10).GetChild(i).GetComponent<TMP_Text>().text);
            Debug.Log(imageUrls.ToArray()[i]);
        }
    }
}
