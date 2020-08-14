#define VR
//#define MOBILE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To toggle camera views according to what platform it is being run on
//May include other toggling in the future
public class ToggleCameras : MonoBehaviour
{
    public GameObject hostRoomBtn;
    public GameObject joinRoomBtn;

#if (VR)

    public void Awake()
    {
        Debug.Log("In VR");
        //Show the host room
        hostRoomBtn.SetActive(true);
        joinRoomBtn.SetActive(false);

    }
#endif

#if (MOBILE)

    public void Awake()
    {
        Debug.Log("In mobile");
        //Show the host room
        hostRoomBtn.SetActive(false);
        joinRoomBtn.SetActive(true);
    }

#endif

}
