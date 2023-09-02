using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FavouriteAROpen : MonoBehaviour
{
    public TMP_Text id;
    public static bool arOpenedFromFavourites;
    
    public void ModelAROpen()
    {
        arOpenedFromFavourites = true;
        ForumDownloader.isFromForum = false;
        CodeEnter.isFromCodeEnter = false;
        ContinuousDemo.modelId = id.text;
        Scenes.ModelDownloadPage();
    }
}
