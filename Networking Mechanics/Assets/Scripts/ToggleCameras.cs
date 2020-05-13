using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //hostRoomButton.gameObject.SetActive(false);
        mobileCam.gameObject.SetActive(true);
        vrCamera.gameObject.SetActive(false);
    }

#endif
}
