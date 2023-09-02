using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System.Collections;
using Siccity.GLTFUtility;

//
// This script allows us to create anchors with
// a prefab attached in order to visbly discern where the anchors are created.
// Anchors are a particular point in space that you are asking your device to track.
//

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class AnchorCreator : MonoBehaviour
{
    // This is the prefab that will appear every time an anchor is created.
    [SerializeField]
    GameObject m_AnchorPrefab;

    public GameObject createdModel, rotationSliderActivator, scaleSliderActivator;
    public Slider rotationSlider, scaleSlider;
    public static bool IsCreated = false;
    private Vector3 startScaleSize;
    public Camera mainCamera;

    public GameObject AnchorPrefab
    {
        get => m_AnchorPrefab;
        set => m_AnchorPrefab = value;
    }

    // Removes all the anchors that have been created.
    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_AnchorPoints)
        {
            Destroy(anchor);
        }
        m_AnchorPoints.Clear();
    }

    // On Awake(), we obtains a reference to all the required components.
    // The ARRaycastManager allows us to perform raycasts so that we know where to place an anchor.
    // The ARPlaneManager detects surfaces we can place our objects on.
    // The ARAnchorManager handles the processing of all anchors and updates their position and rotation.
    void Awake()
    {
        /*createdModel = null;

        StartCoroutine(WaitUntilModelDownloaded());

        //startScaleSize = m_AnchorPrefab.transform.localScale;

        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_AnchorPoints = new List<ARAnchor>();*/

        m_AnchorPrefab = Importer.LoadFromFile(CardScript.path);
        //m_AnchorPrefab.transform.localScale = new UnityEngine.Vector3(0.05f, 0.05f, 0.05f);
    }

    IEnumerator WaitUntilModelDownloaded()
    {
        //ModelDownloader.isModelDownloaded = false;
        //yield return new WaitUntil(() => ModelDownloader.isModelDownloaded);
        //m_AnchorPrefab = ModelDownloader.model;
        //startScaleSize = m_AnchorPrefab.transform.localScale;

        m_AnchorPrefab = Resources.Load<GameObject>(ContinuousDemo.modelId);
        startScaleSize = m_AnchorPrefab.transform.localScale;
        IsCreated = false;
        yield return true;
    }

    void Update()
    {
        if (IsCreated)
        {
            Rotator();
            Scaler();
        }

        Vector2 rayPos = new Vector2(Screen.width / 2, Screen.height / 2);

        // If there is no tap, then simply do nothing until the next call to Update().
        if (IsCreated)
        {
            if (Input.touchCount == 0)
                return;

            var touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
                return;

            rayPos = touch.position;

        }
        if (m_RaycastManager.Raycast(rayPos, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            //If touch position on y axis is lower than 30% of screen height (in px), then simply 
            //do nothing until the next call to Update().
            Vector2 touchDeltaPosition = rayPos;
            if (rayPos.y <= 0.3f * Screen.height)
            {
                return;
            }

            if (rayPos.x >= 0.8f * Screen.width)
            {
                return;
            }

            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;
            var hitTrackableId = s_Hits[0].trackableId;
            var hitPlane = m_PlaneManager.GetPlane(hitTrackableId);

            // This attaches an anchor to the area on the plane corresponding to the raycast hit,
            // and afterwards instantiates an instance of your chosen prefab at that point.
            // This prefab instance is parented to the anchor to make sure the position of the prefab is consistent
            // with the anchor, since an anchor attached to an ARPlane will be updated automatically by the ARAnchorManager as the ARPlane's exact position is refined.
            var anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);

            //If model is created, then destroy it.
            if (IsCreated)
            {
                Destroy(createdModel);
                IsCreated = false;
            }

            //Spawn model.
            createdModel = Instantiate(m_AnchorPrefab, anchor.transform);
            if (createdModel != null)
            {
                IsCreated = true;

                //Activate slider.
                rotationSliderActivator.SetActive(true);
                scaleSliderActivator.SetActive(true);
                createdModel.transform.position = new Vector3(createdModel.transform.position.x, createdModel.transform.position.y + 0.1f,
                    createdModel.transform.position.z);
            }
            rotationSliderActivator.SetActive(true);
            scaleSliderActivator.SetActive(true);
            Debug.Log("Created");

            if (anchor == null)
            {
                Debug.Log("Error creating anchor.");
            }
            else
            {
                // Stores the anchor so that it may be removed later.
                m_AnchorPoints.Add(anchor);
            }
        }
    }

    /*void Update()
    {
        Debug.Log(m_AnchorPrefab);
        // If there is no tap, then simply do nothing until the next call to Update().
        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
            return;

        if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;
            var hitTrackableId = s_Hits[0].trackableId;
            var hitPlane = m_PlaneManager.GetPlane(hitTrackableId);

            // This attaches an anchor to the area on the plane corresponding to the raycast hit,
            // and afterwards instantiates an instance of your chosen prefab at that point.
            // This prefab instance is parented to the anchor to make sure the position of the prefab is consistent
            // with the anchor, since an anchor attached to an ARPlane will be updated automatically by the ARAnchorManager as the ARPlane's exact position is refined.
            var anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);
            createdModel = Instantiate(m_AnchorPrefab, anchor.transform);

            if (anchor == null)
            {
                Debug.Log("Error creating anchor.");
            }
            else
            {
                // Stores the anchor so that it may be removed later.
                m_AnchorPoints.Add(anchor);
            }
        }
    }*/

    //If model is created, then rotate it to Slider value.
    private void Rotator()
    {
        if (IsCreated)
        {
            float yRotation = rotationSlider.value;
            createdModel.transform.eulerAngles = new Vector3(0f, yRotation, 0f);
        }
    }

    private void Scaler()
    {
        if (IsCreated)
        {
            float scaleSize = scaleSlider.value;
            if (scaleSlider.value > 1)
            {
                scaleSize *= scaleSize;
            }
            Debug.Log(scaleSize);
            createdModel.transform.localScale = startScaleSize * scaleSize;
        }
    }


    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
