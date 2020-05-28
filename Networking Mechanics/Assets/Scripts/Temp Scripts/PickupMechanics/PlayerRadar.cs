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
    public PickUppable pickUppable;

    //bool to check if player is holding an object, if they are then, do not assign new object
    public bool holdingPickedUpObject = false;

    public float dist;

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


        if(PickUppable.pickedUpObject != null)
        {
            //check distance between hit object and player
            dist = Vector3.Distance(PickUppable.pickedUpObject.transform.position, transform.position);
            Debug.Log("PlayerRader: Distance from " + PickUppable.pickedUpObject + " is " + dist);


            if (dist >= 2.5)
            {
                pickUppable = null;
            }
        }

        
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


            //if there is nothing currently in the inventory
            if (PickUppable.objectsInInventory.Count == 0)
            {
                //reference the pickuppable script that belongs to the object hit
                pickUppable = hit.collider.gameObject.GetComponent<PickUppable>();
                //set the picked up object to be the hit gameobject
                PickUppable.pickedUpObject = hit.collider.gameObject;
                //there is now a picked up object that the player is holding
                holdingPickedUpObject = true;

            }
            else
            {
                //if there is something in the inventory, do not allow referencing another script
                Debug.LogWarning("PlayerRadar: PickUppable already has a reference!");
            }

        }

        //how can i make the player keep a reference to the pickedup object when they are holding it
        //and make it null when there is nothing in front of them
        //what to do about the sink? should i try changing its logic?
        else if(!hitObject) 
        {
            //draw a white ray
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * objectRadar, Color.white);
            Debug.Log("PlayerRadar: Did not hit anything");



            //pickUppable = null;


        }

    }

    
}