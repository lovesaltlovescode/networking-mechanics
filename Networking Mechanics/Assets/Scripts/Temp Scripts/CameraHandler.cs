using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Toggles camera 
//if in storeroom trigger, turn on storeroom camera
public class CameraHandler : MonoBehaviour
{
    public Camera shopCam;

    public Camera storeroomCam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "StoreroomZone")
        {
            Debug.Log("CameraHandler: Player in storeroom");
            storeroomCam.enabled = true;
            shopCam.enabled = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "StoreroomZone")
        {
            Debug.Log("CameraHandler: Player in shop");
            storeroomCam.enabled = false;
            shopCam.enabled = true;
        }
    }
}
