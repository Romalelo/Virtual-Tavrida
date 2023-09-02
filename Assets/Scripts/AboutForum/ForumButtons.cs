using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class ForumButtons : MonoBehaviour
{
    public GameObject mapsHandler, mapPrefab, imagesHandler, imagePrefab, mapActive, image, map;
    public Texture2D tex;
    private float scrollSpeed = 20f;
    public UnityWebRequest.Result isDownloaded;

    private void Awake()
    {
        StartCoroutine(PostForumToHistory($"http://193.124.118.62/api/1.0/forum/{AboutForumDownloader.id}"));
        for(int i = 0; i < AboutForumDownloader.mapUrls.ToArray().Length; i++)
        {
            map = Instantiate(mapPrefab, mapsHandler.transform);
            map.GetComponent<ScrollRect>().viewport = mapsHandler.GetComponent<RectTransform>();
            StartCoroutine(GetMaps(AboutForumDownloader.mapUrls.ToArray()[i]));
        }

        image = Instantiate(imagePrefab, imagesHandler.transform);
        Sprite sprite;
        sprite = AboutForumDownloader.sprite;
        image.transform.GetComponent<Image>().sprite = sprite;

        StartCoroutine(GetRequest());
    }

    public void FixedUpdate()
    {
        float scrollPosition = 0f;

        if (imagesHandler.transform.childCount % 2 == 0)
        {
            if (imagesHandler.transform.localPosition.x % 207f != 0 && (Input.touchCount == 0 /*|| !Input.GetKey(KeyCode.Space)*/))
            {
                if (imagesHandler.transform.localPosition.x > 0)
                {
                    if (imagesHandler.transform.localPosition.x % 621f <= 414f)
                    {
                        scrollPosition = imagesHandler.transform.localPosition.x - imagesHandler.transform.localPosition.x % 207f;
                        if (scrollPosition == 0)
                        {
                            scrollPosition = 207f;
                        }
                    }
                    else
                    {
                        scrollPosition = imagesHandler.transform.localPosition.x + imagesHandler.transform.localPosition.x % 207f;
                        if (scrollPosition == 0)
                        {
                            scrollPosition = 207f;
                        }
                    }
                }
                else
                {
                    if (imagesHandler.transform.localPosition.x % 621f >= -414f)
                    {
                        scrollPosition = imagesHandler.transform.localPosition.x - imagesHandler.transform.localPosition.x % 207f;
                        if(scrollPosition == 0)
                        {
                            scrollPosition = -207f;
                        }
                    }
                    else
                    {
                        scrollPosition = imagesHandler.transform.localPosition.x + imagesHandler.transform.localPosition.x % 207f;
                        if (scrollPosition == 0)
                        {
                            scrollPosition = -207f;
                        }
                    }
                }
                imagesHandler.transform.localPosition = Vector3.MoveTowards((imagesHandler.transform.localPosition), 
                    new Vector3(scrollPosition , imagesHandler.transform.localPosition.y, 0f), scrollSpeed);
            }
        }
        else
        {
            if (imagesHandler.transform.localPosition.x % 207f != 0 && (Input.touchCount == 0 /*|| !Input.GetKey(KeyCode.Space)*/))
            {
                if (imagesHandler.transform.localPosition.x > 0)
                {
                    if (imagesHandler.transform.localPosition.x % 414f <= 207f)
                    {
                        scrollPosition = imagesHandler.transform.localPosition.x - imagesHandler.transform.localPosition.x % 414f;
                    }
                    else
                    {
                        scrollPosition = imagesHandler.transform.localPosition.x + imagesHandler.transform.localPosition.x % 414f;
                    }
                }
                else
                {
                    if (imagesHandler.transform.localPosition.x % 414f >= -207f)
                    {
                        scrollPosition = imagesHandler.transform.localPosition.x - imagesHandler.transform.localPosition.x % 414f;
                    }
                    else
                    {
                        scrollPosition = imagesHandler.transform.localPosition.x + imagesHandler.transform.localPosition.x % 414f;
                    }
                }
                imagesHandler.transform.localPosition = Vector3.MoveTowards((imagesHandler.transform.localPosition), 
                    new Vector3(scrollPosition, imagesHandler.transform.localPosition.y, 0f), scrollSpeed);
            }
        }
    }

    public void MapButton()
    {
        mapActive.SetActive(true);
    }

    public void MapBackButton()
    {
        mapActive.SetActive(false);
    }

    public void ARButton()
    {
        FavouriteAROpen.arOpenedFromFavourites = false;
        Scenes.QRPage();
    }

    IEnumerator GetRequest()
    {
        for (int i = 0; i < AboutForumDownloader.imageUrls.ToArray().Length; i++)
        {
            isDownloaded = UnityWebRequest.Result.ProtocolError;
            imagesHandler.GetComponent<RectTransform>().sizeDelta = new Vector2(414f * (AboutForumDownloader.imageUrls.ToArray().Length + 1), 330f);
            imagesHandler.transform.localPosition = new Vector3((AboutForumDownloader.imageUrls.ToArray().Length) * 207f, imagesHandler.transform.localPosition.y, 0f);
            image = Instantiate(imagePrefab, imagesHandler.transform);
            StartCoroutine(GetImages(AboutForumDownloader.imageUrls.ToArray()[i]));
            yield return new WaitUntil(() => isDownloaded == UnityWebRequest.Result.Success);
        }
    }

    IEnumerator PostForumToHistory(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", AboutForumDownloader.id);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {

            www.SetRequestHeader("accept", "*/*");
            www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("UserToken")}");


            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Успешно добавлено в историю посещений");
            }
            else
            {
                Debug.Log(www.error + ":" + www.result);
                Debug.Log(AboutForumDownloader.id);
            }
        }
    }

    IEnumerator GetMaps(string url)
    {

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                tex = DownloadHandlerTexture.GetContent(request);
                Sprite sprite;

                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

                map.transform.GetComponent<Image>().sprite = sprite;

            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator GetImages(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {

            yield return request.SendWebRequest();
            isDownloaded = request.result;

            if (request.result == UnityWebRequest.Result.Success)
            {
                tex = DownloadHandlerTexture.GetContent(request);
                Sprite sprite;

                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

                image.transform.GetComponent<Image>().sprite = sprite;

            }
            else
            {
                isDownloaded = UnityWebRequest.Result.Success;
                Debug.Log(request.error);
            }
        }
    }
}
