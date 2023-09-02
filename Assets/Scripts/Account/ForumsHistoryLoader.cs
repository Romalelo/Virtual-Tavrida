using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using UnityEngine.SceneManagement;
using TMPro;

public class ForumsHistoryLoader : MonoBehaviour
{

    public RectTransform ForumsHistoryHandler;
    string uri = "http://193.124.118.62/api/1.0/forum/history";
    public GameObject createdCard, cardPrefab, loaderRotation, forumsList, mapUrlsText;
    public ForumsHistory[] forumsHistory;
    public static int number;
    public static string id, title, description, logoUrl, str;
    public static string[] mapUrls;
    public static DateTime startedAt, endedAt;
    public static Texture2D tex;
    public UnityWebRequest.Result isDownloaded;
    public TMP_Text username;


    void Awake()
    {
        username.text = PlayerPrefs.GetString("Username");
        StartCoroutine(GetRequest(uri));
    }

    IEnumerator GetRequest(string uri)
    {

        UnityWebRequest www = UnityWebRequest.Get(uri);

        www.SetRequestHeader("accept", "*/*");
        www.SetRequestHeader("Content-Type", "text/json");
        www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("UserToken")}");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            forumsHistory = HistoryDownloader.CreateFromJSON(www.downloadHandler.text).forumList;

            float height = Screen.height, width = Screen.width, ySize;
            if (forumsHistory.Length <= 2)
            {
                ySize = ((((896f / 414f) - (height / width)) * 460f) + 370f);
                //ForumsHistoryHandler.sizeDelta = new Vector2(0f, ySize);
            }
            else
            {
                ySize = ((((896f / 414f) - (height / width)) * 460f) + (460f * (forumsHistory.Length - 2)) + 370f);
                //ForumsHistoryHandler.sizeDelta = new Vector2(0f, ySize);
            }

            for (int i = 0; i < forumsHistory.Length; i++)
            {
                isDownloaded = UnityWebRequest.Result.ProtocolError;
                id = forumsHistory[i].id;
                title = forumsHistory[i].title;
                description = forumsHistory[i].description;
                logoUrl = forumsHistory[i].logoUrl;
                startedAt = DateTime.Parse(forumsHistory[i].startedAt);
                endedAt = DateTime.Parse(forumsHistory[i].endedAt);
                str = startedAt.ToShortDateString() + " - " + endedAt.ToShortDateString();
                createdCard = Instantiate(cardPrefab, ForumsHistoryHandler.transform);
                StartCoroutine(DownloadLogo(logoUrl));
                yield return new WaitUntil(() => isDownloaded == UnityWebRequest.Result.Success);
                createdCard.transform.GetChild(2).GetComponent<TMP_Text>().text = title;
                createdCard.transform.GetChild(4).GetComponent<TMP_Text>().text = description;
                createdCard.transform.GetChild(6).GetComponent<TMP_Text>().text = str;
                createdCard.transform.GetChild(7).GetComponent<TMP_Text>().text = id;
            }
            loaderRotation.SetActive(false);
            forumsList.SetActive(true);
        }
        else
        {
            Debug.Log(www.error + ": " + www.result);
        }
    }

    IEnumerator DownloadLogo(string MediaUrl)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl))
        {
            yield return request.SendWebRequest();
            isDownloaded = request.result;

            if (request.result == UnityWebRequest.Result.Success)
            {
                tex = DownloadHandlerTexture.GetContent(request);
                Sprite sprite;

                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

                createdCard.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;

                if (tex.width / 366f < tex.height / 233f)
                {
                    createdCard.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(366f, tex.width / (tex.height / 233f));
                }
                else
                {
                    createdCard.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(tex.height / (tex.width / 366f), 233f);
                }

            }
            else
            {
                isDownloaded = UnityWebRequest.Result.Success;
                Debug.Log(request.error);
            }
        }
    }
}


[System.Serializable]
public class HistoryDownloader
{
    public ForumsHistory[] forumList;
    public static HistoryDownloader CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<HistoryDownloader>(jsonString);
    }
}

[System.Serializable]
public class ForumsHistory
{
    public string id;
    public string title;
    public string description;
    public string logoUrl;
    public string[] imageUrl;
    public string[] mapUrls;
    public string startedAt;
    public string endedAt;
    public static ForumsHistory CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ForumsHistory>(jsonString);
    }
}
