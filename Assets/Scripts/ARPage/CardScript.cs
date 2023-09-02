using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CardScript : MonoBehaviour
{
    public static string path;
    // Start is called before the first frame update
    public void Awake()
    {
        //StartCoroutine(GetFileRequest($"http://193.124.118.62/api/1.0/data/model/{ContinuousDemo.modelId}"));
        StartCoroutine(GetFileRequest($"https://github.com/KhronosGroup/glTF-Sample-Models/raw/master/2.0/Duck/glTF-Binary/Duck.glb"));
    }

    IEnumerator GetFileRequest(string url)
    {
        Debug.Log("нетнет");
        var sss = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        string path1 = Path.Combine(Application.persistentDataPath, "sss.glb");
        sss.downloadHandler = new DownloadHandlerFile(path1);
        //sss.SetRequestHeader("Authorization", $"Bearer {TextInput.token}");
        Debug.Log("нетнет");
        yield return sss.SendWebRequest();
        if (sss.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(sss.error);
            Debug.Log("нетнет1");
        }
        else
        {
            path = Application.persistentDataPath + "/sss.glb";
            Debug.Log("нетнет");
            Debug.Log(path);
            Scenes.ARPage();
        }
    }
}
