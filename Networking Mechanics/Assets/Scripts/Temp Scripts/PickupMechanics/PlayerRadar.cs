using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to player
//Finds what object they are currently looking at and returns its name/tag
//will ignore the ground/environment and the player capsule
public class PlayerRadar : MonoBehaviour
{

    public float objectRadar; 
    //How far the ray must be casted
    //How close must the object be to be detected by the player

    //shift layer bits to 8 and 9 (player and environment and dropzone and pickedup objects)
    //ignore these layers, they do not need to be raycasted
    private int ignoreLayers = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11;

    //player inventory slot
    public PlayerInventory playerInventory;

    //when a object is detected, we will reference its  pickuppable script
    //change this back to non-static if needed
    public static PickUppable pickUppable;

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //look for an object to detect every frame
        DetectObject();

        Debug.Log("Current inventory: " + PickUppable.objectsInInventory.Count);
    }

    //Detects if an object has been hit on button press using raycast
    //if object has been hit and is pickuppable, pick it up
    //since zones, player and ground have been hidden from detection, the object should always be pickuppable
    public void DetectObject()
    {
        RaycastHit hit;

        //Raycast from object origin forward and ignore objects on layer masks
        bool hitObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, objectRadar, ~ignoreLayers);


        //if an object was hit
        if (hitObject)
        {
            //draw a yellow ray from object position (origin) forward to the distance of the cast (objectRadar)
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * objectRadar, Color.yellow);
            
            //Return the hit object tag
            Debug.Log("PlayerRadar: Hit " + hit.collider.tag);

            ////check distance between hit object and player
            //float dist = Vector3.Distance(hit.transform.position, transform.position);
            //Debug.Log("PlayerRader: Distance from " + hit.collider.gameObject + " is " + dist);


            //if there is nothing currently in the inventory
            if (PickUppable.objectsInInventory.Count == 0)
            {
                //reference the pickuppable script that belongs to the object hit
                pickUppable = hit.collider.gameObject.GetComponent<PickUppable>();
                //set the picked up object to be the hit gameobject
                PickUppable.pickedUpObject = hit.collider.gameObject;

            }
            else
            {
                //if there is something in the inventory, do not allow referencing another script
                Debug.LogWarning("PlayerRadar: PickUppable already has a reference!");
            }


        }
        else //did not hit any object
        {
            //draw a white ray
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * objectRadar, Color.white);
            Debug.Log("PlayerRadar: Did not hit anything");
            //PickUppable.pickedUpObject = null;

        }

    }

    
}