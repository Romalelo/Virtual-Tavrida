using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class CodeEnter : MonoBehaviour
{
    public GameObject codeEnterDialogue;
    private string code = "";
    public TMP_InputField inputField;
    public Sprite errorDialogue, normalDialogue, normalInputField, errorInputField;
    public static bool isFromCodeEnter;

    public void CodeEnterDialogue()
    {
        codeEnterDialogue.SetActive(true);
        inputField.ActivateInputField();
    }

    public void CodeExitDialogue()
    {
        isFromCodeEnter = false;
        inputField.text = "";
        codeEnterDialogue.transform.GetChild(0).GetComponent<Image>().sprite = normalDialogue;
        codeEnterDialogue.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = normalInputField;
        codeEnterDialogue.SetActive(false);
    }

    public void ContinueButton()
    {
        code = inputField.text;
        Debug.Log(code);
        if (code.Length == 4)
        {
            StartCoroutine(IsModelExist(code));
        }
    }

    public void Update()
    {
        if(inputField.text.Length < 4)
        {
            codeEnterDialogue.transform.GetChild(0).GetComponent<Image>().sprite = normalDialogue;
            codeEnterDialogue.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = normalInputField;
        }
    }

    IEnumerator IsModelExist(string code)
    {

        UnityWebRequest www = UnityWebRequest.Get($"http://193.124.118.62/api/1.0/model/code={code}");

        www.SetRequestHeader("accept", "*/*");
        www.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("UserToken")}");

        Debug.Log(PlayerPrefs.GetString("UserToken"));

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            isFromCodeEnter = true;
            FavouriteAROpen.arOpenedFromFavourites = false;
            ForumDownloader.isFromForum = false;
            inputField.text = "";
            ContinuousDemo.modelId = Model.CreateFromJSON(www.downloadHandler.text).id;
            Scenes.ModelDownloadPage();
        }
        else
        {
            codeEnterDialogue.transform.GetChild(0).GetComponent<Image>().sprite = errorDialogue;
            codeEnterDialogue.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = errorInputField;
            Debug.Log(www.error + ":" + www.result);
        }
    }

}

public class Model
{
    public string id;
    public static Model CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Model>(jsonString);
    }
}