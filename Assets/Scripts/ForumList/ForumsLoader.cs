using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Android;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class ForumsLoader : MonoBehaviour
{

    public RectTransform forumsHandler;
    string uris = "http://193.124.118.62/api/1.0/forum/list?Count=25&Skiped=0";
    public GameObject createdCard, cardPrefab, forumsList, loaderRotation, mapUrlsTextPrefab, mapUrlsText, imageUrlsTextPrefab, imageUrlsText;
    public Forum[] forum;
    public static int number;
    public static string id, title, description, logoUrl, str;
    public static string[] mapUrlArray, imageUrlArray;
    public static DateTime startedAt, endedAt;
    public static Texture2D tex;
    public UnityWebRequest.Result isDownloaded;
    private float ySize;


    void Awake()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#elif UNITY_IOS
			if(!Application.HasUserAuthorization(UserAuthorization.WebCam))
			{
				Application.RequestUserAuthorization(UserAuthorization.WebCam);
			}
#endif
        Application.targetFrameRate = 60;
        StartLoader(uris);
    }

    public void StartLoader(string url)
    {
        StopAllCoroutines();
        loaderRotation.SetActive(true);
        forumsList.SetActive(false);
        int countToDestroy = forumsList.transform.childCount;

        while(countToDestroy > 0)
        {
            Debug.Log(countToDestroy);
            Destroy(forumsList.transform.GetChild(countToDestroy - 1).gameObject);
            countToDestroy--;
        }

        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string uri)
    {

        UnityWebRequest www = UnityWebRequest.Get(uri);

        www.SetRequestHeader("accept", "*/*");
        www.SetRequestHeader("Content-Type", "text/json");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {

            forum = Downloader.CreateFromJSON(www.downloadHandler.text).forumList;

            float height = Screen.height, width = Screen.width;
            ySize = ((((896f / 414f) - (height / width)) * 460f));
            if (forum.Length == 2)
            {
                ySize = ((((896f / 414f) - (height / width)) * 460f) + 250f);
                if (Search.isSearch)
                {
                    ySize += 75f;
                }
            }
            if (forum.Length > 2)
            {
                ySize = ((((896f / 414f) - (height / width)) * 460f) + (460f * (forum.Length - 2)) + 250f);
                if (Search.isSearch)
                {
                    ySize += 75f;
                }
            }

            //forumsHandler.sizeDelta = new Vector2(0f, ySize);

            Debug.Log(forumsHandler.sizeDelta);


            for (int i = 0; i < forum.Length; i++)
            {
                isDownloaded = UnityWebRequest.Result.ProtocolError;
                id = forum[i].id;
                title = forum[i].title;
                description = forum[i].description;
                logoUrl = forum[i].logoUrl;
                mapUrlArray = forum[i].mapUrls;
                imageUrlArray = forum[i].imageUrls;
                startedAt = DateTime.Parse(forum[i].startedAt);
                endedAt = DateTime.Parse(forum[i].endedAt);
                str = startedAt.ToShortDateString() + " - " + endedAt.ToShortDateString();
                createdCard = Instantiate(cardPrefab, forumsList.transform);
                for (int j = 0; j < forum[i].mapUrls.Length; j++)
                {
                    mapUrlArray[j] = forum[i].mapUrls[j];
                    mapUrlsText = Instantiate(mapUrlsTextPrefab, createdCard.transform.GetChild(9));
                    mapUrlsText.GetComponent<TMP_Text>().text = mapUrlArray[j];
                }
                for (int j = 0; j < forum[i].imageUrls.Length; j++)
                {
                    imageUrlArray[j] = forum[i].imageUrls[j];
                    imageUrlsText = Instantiate(imageUrlsTextPrefab, createdCard.transform.GetChild(10));
                    imageUrlsText.GetComponent<TMP_Text>().text = imageUrlArray[j];
                }
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

            request.SetRequestHeader("accept", "*/*");
            request.SetRequestHeader("Content-Type", "text/json");

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
                    createdCard.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(366f, tex.width/(tex.height / 233f));
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
            /*string str = request.error + " " + request.result.ToString();
            createdCard.transform.GetChild(4).GetComponent<TMP_Text>().text = str;*/
        }
    }
}


    [System.Serializable]
    public class Downloader
    {
        public Forum[] forumList;
        public static Downloader CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Downloader>(jsonString);
        }
    }

    [System.Serializable]
    public class Forum
    {
        public string id;
        public string title;
        public string description;
        public string logoUrl;
        public string[] imageUrls;
        public string[] mapUrls;
        public string startedAt;
        public string endedAt;
        public static Forum CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Forum>(jsonString);
        }
    }

[System.Serializable]
public class Maps
{
    public string[] mapUrls;
}
