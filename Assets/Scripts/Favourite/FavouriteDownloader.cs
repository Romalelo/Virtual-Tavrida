using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FavouriteDownloader : MonoBehaviour
{
    public RectTransform favouriteModelsHandler;
    string uris = "http://193.124.118.62/api/1.0/model/favorites";
    public GameObject createdCard, cardPrefab, favouriteModelsList, loaderRotation;
    public FavouriteModel[] favouriteModel;
    public static int number;
    public static string id, title, logoUrl, str, forumTitle, forumLogoUrl;
    public static DateTime startedAt, endedAt;
    public static Texture2D tex;
    public UnityWebRequest.Result isDownloaded;
    private float ySize;


    void Awake()
    {
        Application.targetFrameRate = 60;
        StartLoader(uris);
    }

    public void StartLoader(string url)
    {
        StopAllCoroutines();
        loaderRotation.SetActive(true);
        favouriteModelsList.SetActive(false);
        int countToDestroy = favouriteModelsList.transform.childCount;

        while (countToDestroy > 0)
        {
            Debug.Log(countToDestroy);
            Destroy(favouriteModelsList.transform.GetChild(countToDestroy - 1).gameObject);
            countToDestroy--;
        }

        StartCoroutine(GetRequest(url));
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
            favouriteModel = FavouriteDownload.CreateFromJSON(www.downloadHandler.text).favorites;

            float height = Screen.height, width = Screen.width;
            if (favouriteModel.Length == 2)
            {
                ySize = ((((896f / 414f) - (height / width)) * 460f) + 250f);
                //favouriteModelsHandler.sizeDelta = new Vector2(0f, ySize);
            }
            else
            {
                ySize = ((((896f / 414f) - (height / width)) * 460f) + (460f * (favouriteModel.Length - 2)) + 250f);
                //favouriteModelsHandler.sizeDelta = new Vector2(0f, ySize);
            }

            for (int i = 0; i < favouriteModel.Length; i++)
            {
                isDownloaded = UnityWebRequest.Result.ProtocolError;
                id = favouriteModel[i].id;
                title = favouriteModel[i].title;
                logoUrl = favouriteModel[i].logoUrl;
                forumTitle = favouriteModel[i].forumTitle;
                forumLogoUrl = favouriteModel[i].forumLogoUrl;
                startedAt = DateTime.Parse(favouriteModel[i].startedAt);
                endedAt = DateTime.Parse(favouriteModel[i].endedAt);
                str = startedAt.ToShortDateString() + " - " + endedAt.ToShortDateString();
                createdCard = Instantiate(cardPrefab, favouriteModelsList.transform);
                StartCoroutine(DownloadForumLogo(forumLogoUrl));
                yield return new WaitUntil(() => isDownloaded == UnityWebRequest.Result.Success);
                isDownloaded = UnityWebRequest.Result.ProtocolError;
                StartCoroutine(DownloadLogo(logoUrl));
                yield return new WaitUntil(() => isDownloaded == UnityWebRequest.Result.Success);
                createdCard.transform.GetChild(5).GetComponent<TMP_Text>().text = title;
                createdCard.transform.GetChild(1).GetComponent<TMP_Text>().text = str;
                createdCard.transform.GetChild(0).GetComponent<TMP_Text>().text = forumTitle;
                createdCard.transform.GetChild(6).GetComponent<TMP_Text>().text = id;
            }

            loaderRotation.SetActive(false);
            favouriteModelsList.SetActive(true);

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

                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

                createdCard.transform.GetChild(3).GetChild(0).GetComponent<Image>().sprite = sprite;

                if (tex.width / 366f < tex.height / 320f)
                {
                    createdCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(366f, tex.height / (tex.width / 366f));
                }
                else
                {
                    createdCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width / (tex.height / 320f), 320f);
                }

            }
            else
            {
                isDownloaded = UnityWebRequest.Result.Success;
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator DownloadForumLogo(string MediaUrl)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl))
        {
            yield return request.SendWebRequest();
            isDownloaded = request.result;

            if (request.result == UnityWebRequest.Result.Success)
            {
                tex = DownloadHandlerTexture.GetContent(request);

                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

                createdCard.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = sprite;

                if (tex.width / 56f < tex.height / 56f)
                {
                    createdCard.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(56f, tex.height / (tex.width / 56f));
                }
                else
                {
                    createdCard.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width / (tex.height / 56f), 56f);
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
public class FavouriteDownload
{
    public FavouriteModel[] favorites;
    public static FavouriteDownload CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<FavouriteDownload>(jsonString);
    }
}

[System.Serializable]
public class FavouriteModel
{
    public string id;
    public string title;
    public string logoUrl;
    public string forumId;
    public string forumTitle;
    public string forumLogoUrl;
    public string startedAt;
    public string endedAt;
    public string valueUrl;
    public static FavouriteModel CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<FavouriteModel>(jsonString);
    }
}
