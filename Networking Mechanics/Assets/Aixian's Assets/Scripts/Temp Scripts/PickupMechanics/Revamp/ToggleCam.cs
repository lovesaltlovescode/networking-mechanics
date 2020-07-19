using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hides the meshes of certain walls and objects if player enters the storeroom
public class ToggleCam : MonoBehaviour
{
    public GameObject hiddenObjects;

    [SerializeField] private MeshRenderer[] meshesToHide;


    // Start is called before the first frame update
    void Start()
    {
        //hide meshes
        meshesToHide = hiddenObjects.GetComponentsInChildren<MeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //if hit object is a player
        if (other.gameObject.layer == 8)
        {
            Debug.Log("CameraHandler: Player in storeroom");

            //hide meshes
            for (int i = 0; i < meshesToHide.Length; i++)
            {
                meshesToHide[i].enabled = false;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Debug.Log("CameraHandler: Player in shop");


            //show meshes
            for (int i = 0; i < meshesToHide.Length; i++)
            {
                meshesToHide[i].enabled = true;
            }
        }
    }
}