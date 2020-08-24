using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Detectver2 : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            Debug.Log("Enter detect trigger ver 2");
            transform.parent.gameObject.AddComponent<XRGrabInteractable>();
            if (transform.parent.gameObject.GetComponent<XRGrabInteractable>())
            {
                Debug.Log("Add success ver 2");
            }
            else
            {
                Debug.Log("Add fail ver 2");
            }
            
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Hand")
        {
            Destroy(transform.parent.gameObject.GetComponent<XRGrabInteractable>());

            if (transform.parent.gameObject.GetComponent<XRGrabInteractable>())
            {
                Debug.Log("Destroy failed ver 2");
            }
            else
            {
                Debug.Log("Destroy Success ver 2");
            }

        }
    }
}
