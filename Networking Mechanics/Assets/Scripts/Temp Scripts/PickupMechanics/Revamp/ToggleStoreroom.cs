using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hides the meshes of certain walls and objects if player enters the storeroom
public class ToggleStoreroom : MonoBehaviour
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
                hiddenObjects[i].GetComponent<MeshRenderer>().enabled = false;
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
                hiddenObjects[i].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}