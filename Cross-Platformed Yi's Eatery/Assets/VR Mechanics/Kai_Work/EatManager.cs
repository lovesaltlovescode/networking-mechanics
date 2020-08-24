using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EatManager : MonoBehaviour
{
    public bool isEatable = false;
    XRGrabInteractable grabInteractable;
    MeshRenderer meshrenderer;
    private void Awake()
    {
        meshrenderer = GetComponent<MeshRenderer>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.onActivate.AddListener(Eat);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Eat")
        {
            Debug.Log("Reached Trig");
            isEatable = true;
        }

        if(other.tag == "Trash")
        {
            Destroy(gameObject);
        }
    }



    public void Eat(XRBaseInteractor interactor)
    {
        if (isEatable)
        {
            Debug.Log("reached eat");
            gameObject.layer = LayerMask.NameToLayer("Ungrabbable");
            Debug.Log(gameObject.layer);
            meshrenderer.enabled = false;
            //Destroy(gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Eat")
        {
            Debug.Log("Did not reach Trig");
            isEatable = false;
        }
    }
}