using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderAnimation : MonoBehaviour
{
    public GameObject loaderRotation;

    void FixedUpdate()
    {
        loaderRotation.transform.rotation = Quaternion.Euler(0f, 0f, loaderRotation.transform.rotation.eulerAngles.z - 5f);
    }
}
