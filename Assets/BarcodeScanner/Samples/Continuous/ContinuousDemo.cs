using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Android;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class ContinuousDemo : MonoBehaviour {

	private IScanner BarcodeScanner;
	public Text TextHeader;
	public RawImage Image;
	public AudioSource Audio;
	private float RestartTime;
	public static string modelId;

	// Disable Screen Rotation on that screen
	void Awake()
	{
        Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	void Start () {
		// Create a basic scanner
		BarcodeScanner = new Scanner();
		BarcodeScanner.Camera.Play();

		// Display the camera texture through a RawImage
		BarcodeScanner.OnReady += (sender, arg) => {
			// Set Orientation & Texture
			Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
            //Image.transform.localScale = BarcodeScanner.Camera.GetScale();

            //WebCamTexture webcamTexture = new WebCamTexture();
            Image.texture = BarcodeScanner.Camera.Texture;

			TextHeader.text += Image.transform.GetComponent<RectTransform>().sizeDelta;

            // Keep Image Aspect Ratio
			Image.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

            TextHeader.text += Image.transform.GetComponent<RectTransform>().sizeDelta;

#if UNITY_IOS
				Image.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, -1f, 1f);
#endif

			var rect = Image.GetComponent<RectTransform>();
            var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
            TextHeader.text += Image.transform.GetComponent<RectTransform>().sizeDelta;

            RestartTime = Time.realtimeSinceStartup;
		};
	}

	/// <summary>
	/// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
	/// </summary>
	private void StartScanner()
	{
		BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop();
			if (TextHeader.text.Length > 250)
			{
				TextHeader.text = "";
			}
			modelId = barCodeValue;

			TimeSpan timeOnForum = DateTime.Now.Subtract(DateTime.Parse(PlayerPrefs.GetString(AboutForumDownloader.title + "Start")));
			Debug.Log(timeOnForum.TotalSeconds);
			PlayerPrefs.SetInt(AboutForumDownloader.title + "End", (int)timeOnForum.TotalSeconds);
            Scenes.ModelDownloadPage();
			RestartTime += Time.realtimeSinceStartup + 1f;

			// Feedback
			//Audio.Play();

#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
#endif
		});
	}

	/// <summary>
	/// The Update method from unity need to be propagated
	/// </summary>
	void Update()
	{
		if (BarcodeScanner != null)
		{
			BarcodeScanner.Update();
		}

		// Check if the Scanner need to be started or restarted
		if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
		{
			StartScanner();
			RestartTime = 0;
		}
	}

	#region UI Buttons

	public void ClickBack()
	{
		// Try to stop the camera before loading another scene
		StartCoroutine(StopCamera(() => {
			SceneManager.LoadScene("Boot");
		}));
	}

	/// <summary>
	/// This coroutine is used because of a bug with unity (http://forum.unity3d.com/threads/closing-scene-with-active-webcamtexture-crashes-on-android-solved.363566/)
	/// Trying to stop the camera in OnDestroy provoke random crash on Android
	/// </summary>
	/// <param name="callback"></param>
	/// <returns></returns>
	public IEnumerator StopCamera(Action callback)
	{
		// Stop Scanning
		Image = null;
		BarcodeScanner.Destroy();
		BarcodeScanner = null;

		// Wait a bit
		yield return new WaitForSeconds(0.1f);

		callback.Invoke();
	}

	#endregion
}
