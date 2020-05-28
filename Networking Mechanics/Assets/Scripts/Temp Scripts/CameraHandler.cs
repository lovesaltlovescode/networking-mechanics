using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Toggles camera 
//if in storeroom trigger, turn on storeroom camera
public class CameraHandler : MonoBehaviour
{
    public GameObject[] hiddenObjects;

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
        if (other.tag == "StoreroomZone")
        {
            Debug.Log("CameraHandler: Player in storeroom");
            for(int i = 0; i < hiddenObjects.Length; i++)
            {
                hiddenObjects[i].SetActive(false);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "StoreroomZone")
        {
            Debug.Log("CameraHandler: Player in shop");
            for (int i = 0; i < hiddenObjects.Length; i++)
            {
                hiddenObjects[i].SetActive(true);
            }
        }
    }
}