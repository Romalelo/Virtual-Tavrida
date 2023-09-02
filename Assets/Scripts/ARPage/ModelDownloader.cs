using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using System.IO;

public class ModelDownloader : MonoBehaviour
{
    private string url = "http://193.124.118.62/api/1.0/data/model/04582526db404835a735396ca36e9565.gltf";

    [SerializeField]
    public static GameObject wrapper, model;
    string filePath, path, token;
    public static bool isModelDownloaded = false;

    private void Start()
    {
        isModelDownloaded = false;
        filePath = $"{Application.persistentDataPath}/Files/";
        wrapper = new GameObject
        {
            name = "Model"
        };

        StartCoroutine(GetFileRequest(url));
    }
    public void DownloadFile(string url)
    {
        path = GetFilePath(url);
        if (File.Exists(path))
        {
            Debug.Log("Found file locally, loading...");
            LoadModel(path);
            return;
        }

        StartCoroutine(GetFileRequest(url));
    }

    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        string filename = pieces[pieces.Length - 1];

        return $"{filePath}{filename}";
    }

    void LoadModel(string path)
    {
        GameObject model = Importer.LoadFromFile(path);
    }

    /*IEnumerator GetToken(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "password");
        form.AddField("client_id", "ArCore");
        form.AddField("username", "457101af-6b98-4587-93c4-6b216325d0b5");
        form.AddField("password", "123");
        form.AddField("scope", "School");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                token = JsonUtility.FromJson<Token>(www.downloadHandler.text).access_token;
                StartCoroutine(GetFileRequest("http://api.ar-education.xyz/project/models/12/content"));
            }
            else
            {
                Debug.Log(www.error);
            }
        }
    }*/

    IEnumerator GetFileRequest(string url)
    {
        isModelDownloaded = false;
        using (UnityWebRequest ddd = UnityWebRequest.Get(url))
        {
            Debug.Log(token);
            //ddd.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return ddd.SendWebRequest();

            if (ddd.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(ddd.downloadHandler.data);
                byte[] bytes = ddd.downloadHandler.data;
                File.WriteAllBytes(Application.dataPath + "/Export.gltf", bytes);
                model = Importer.LoadFromFile(Application.dataPath + "/Export.gltf");
                isModelDownloaded = true;
                model.SetActive(false);
                //AnchorCreator.m_AnchorPrefab = model;
            }
            else
            {
                Debug.Log(ddd.error);
            }
        }
    }
}
