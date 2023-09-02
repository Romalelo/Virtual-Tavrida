using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using NativeGalleryNamespace;
//using UnityEngine.WSA;

public class Buttons : MonoBehaviour
{
    public GameObject likeButton, info, infoButton, rotationSlider, scaleSlider, copyButton, screenshotButton, backButton, artWalkWaterMark, museumOfHealthWaterMark;
    public Sprite likedSprite, dislikedSprite, closeSprite, infoSprite;
    private bool isLiked = false, isInfoShowed = false, isAlreadyLiked = false;
    private Model model;
    private string modelTitleDownload, code;
    public TMP_Text modelTitleText;

    public void Awake()
    {
        StartCoroutine(IsModelAlreadyLiked(ContinuousDemo.modelId));
        //StartCoroutine(IsModelAlreadyLiked("e6f19710-9c79-40e1-9148-d5e4b601197c"));
    }

    public void CopyText()
    {
        GUIUtility.systemCopyBuffer = code;
    }

    public void LikeButton()
    {
        StopAllCoroutines();
        StartCoroutine(DisLikeModel(ContinuousDemo.modelId));
        //StartCoroutine(DisLikeModel("e6f19710-9c79-40e1-9148-d5e4b601197c"));
    }

    public void InfoButton()
    {
        if (!isInfoShowed)
        {
            screenshotButton.SetActive(false);
            info.SetActive(true);
            isInfoShowed = true;
            infoButton.GetComponent<Image>().sprite = closeSprite;
            rotationSlider.SetActive(false);
            scaleSlider.SetActive(false);
        }
        else
        {
            screenshotButton.SetActive(true);
            info.SetActive(false);
            isInfoShowed = false;
            infoButton.GetComponent<Image>().sprite = infoSprite;
            if (AnchorCreator.IsCreated)
            {
                rotationSlider.SetActive(true);
                scaleSlider.SetActive(true);
            }
        }
    }

    public void MakeScrenshot()
    {
        likeButton.SetActive(false);
        infoButton.SetActive(false);
        screenshotButton.SetActive(false);
        backButton.SetActive(false);
        copyButton.SetActive(false);
        rotationSlider.SetActive(false);
        scaleSlider.SetActive(false);
        museumOfHealthWaterMark.SetActive(true);
        artWalkWaterMark.SetActive(true);

        StartCoroutine(TakeScreenshotAndSave());
    }

    private IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        yield return new WaitForSeconds(1f);

        museumOfHealthWaterMark.SetActive(false);
        artWalkWaterMark.SetActive(false);
        likeButton.SetActive(true);
        infoButton.SetActive(true);
        screenshotButton.SetActive(true);
        backButton.SetActive(true);
        copyButton.SetActive(true);
        if (AnchorCreator.IsCreated)
        {
            rotationSlider.SetActive(true);
            scaleSlider.SetActive(true);
        }

        // Save the screenshot to Gallery/Photos
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(ss, "ArtWalk", 
            modelTitleDownload + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss"), (success, path) => Debug.Log("Media save result: " + success + " " + path));

        Debug.Log("Permission result: " + permission);

        // To avoid memory leaks
        Destroy(ss);
    }


    IEnumerator DisLikeModel(string modelId)
    {
        UnityWebRequest www = UnityWebRequest.Put($"http://193.124.118.62/api/1.0/model/{modelId}", new byte[] { });

        www.SetRequestHeader("accept", "*/*");
        www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("UserToken")}");

        Debug.Log(PlayerPrefs.GetString("UserToken"));

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            if (isLiked)
            {
                likeButton.GetComponent<Image>().sprite = dislikedSprite;
                isLiked = false;
            }
            else
            {
                likeButton.GetComponent<Image>().sprite = likedSprite;
                isLiked = true;
            }
        }
        else
        {
            Debug.Log(www.error + ":" + www.result);
        }
    }

    IEnumerator IsModelAlreadyLiked(string modelId)
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://193.124.118.62/api/1.0/model/id={modelId}?duration={PlayerPrefs.GetInt(AboutForumDownloader.title + "End")}");

        www.SetRequestHeader("accept", "*/*");
        www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("UserToken")}");

        Debug.Log(PlayerPrefs.GetString("UserToken"));

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            modelTitleDownload = Model.CreateFromJSON(www.downloadHandler.text).title;
            if (FavouriteAROpen.arOpenedFromFavourites)
            {
                isAlreadyLiked = true;
            }
            else
            {
                isAlreadyLiked = Model.CreateFromJSON(www.downloadHandler.text).like;
            }
            isLiked = isAlreadyLiked;
            code = Model.CreateFromJSON(www.downloadHandler.text).code;

            modelTitleText.text = modelTitleDownload;

            Debug.Log(code);

            if (!isAlreadyLiked)
            {
                likeButton.GetComponent<Image>().sprite = dislikedSprite;
            }
            else
            {
                likeButton.GetComponent<Image>().sprite = likedSprite;
            }
        }
        else
        {
            Debug.Log(www.error + ":" + www.result);
        }
    }

    private class Model
    {
        public bool like;
        public string title;
        public string code;

        public static Model CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Model>(jsonString);
        }
    }
}