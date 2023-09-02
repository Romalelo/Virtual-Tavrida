using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModelDelete : MonoBehaviour
{

    private string id;
    public GameObject card;
    public Sprite likedSprite, dislikedSprite;
    private bool isLiked = true;

    public void OnButtonClicked()
    {
        StopAllCoroutines();
        id = card.transform.GetChild(6).GetComponent<TMP_Text>().text;
        StartCoroutine(DislikeModel(id));
    }

    IEnumerator DislikeModel(string modelId)
    {
        UnityWebRequest www = UnityWebRequest.Put($"http://193.124.118.62/api/1.0/model/{modelId}", new byte[] { });

        www.SetRequestHeader("accept", "*/*");
        www.SetRequestHeader("Content-Type", "text/json");
        www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("UserToken")}");

        Debug.Log(PlayerPrefs.GetString("UserToken"));

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            if (isLiked) {
                card.transform.GetChild(4).GetComponent<Image>().sprite = dislikedSprite;
                isLiked = false;
            }
            else
            {
                card.transform.GetChild(4).GetComponent<Image>().sprite = likedSprite;
                isLiked = true;
            }
        }
        else
        {
            Debug.Log(www.error + ":" + www.result);
        }
    }
}
