using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DetectXRGrab : MonoBehaviour
{
    GameObject triggeredObj;

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Detect")
        {

            triggeredObj = other.gameObject;
            other.gameObject.AddComponent<XRGrabInteractable>();
            if (other.gameObject.GetComponent<XRGrabInteractable>())
            {
                Debug.Log("Add success");
            }
            else
            {
                Debug.Log("Add fail");
            }
            Debug.Log("Enter detect trigger");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Destroy(triggeredObj.GetComponent<XRGrabInteractable>());

        if (other.tag == "Detect")
        {
            if (triggeredObj.GetComponent<XRGrabInteractable>())
            {
                Debug.Log("failed");
            }
            else
            {
                Debug.Log("Destroy Success");
            }
            
        }
    }
}
