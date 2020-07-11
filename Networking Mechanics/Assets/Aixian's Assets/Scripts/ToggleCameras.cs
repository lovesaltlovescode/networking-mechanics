using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To toggle camera views according to what platform it is being run on
//May include other toggling in the future
public class ToggleCameras : MonoBehaviour
{

    public Camera vrCamera;
    public Camera mobileCam;

#if UNITY_EDITOR

    public void Awake()
    {
        Debug.Log("In editor!");
        vrCamera.gameObject.SetActive(true);
        mobileCam.gameObject.SetActive(false);
        
    }

#elif UNITY_STANDALONE_WIN

    public void Awake()
    {
        //Debug.Log("Standalone windows!");
        mobileCam.gameObject.SetActive(true);
        vrCamera.gameObject.SetActive(false);
    }


#elif UNITY_ANDROID

    public void Awake()
    {
        Debug.Log("On android!");
        mobileCam.gameObject.SetActive(true);
        vrCamera.gameObject.SetActive(false);
    }
#endif
}
