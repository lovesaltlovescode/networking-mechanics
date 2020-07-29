using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to player
//Finds what object they are currently looking at and returns its name/tag
//will ignore the ground/environment and the player capsule
public class PlayerRadar : MonoBehaviour
{

    [SerializeField] float objectRadar = 3f; 
    //How far the ray must be casted
    //How close must the object be to be detected by the player

    //shift layer bits to 8 and 9 (player and environment and dropzone and pickedup objects)
    //ignore these layers, they do not need to be raycasted
    private int ignoreLayers = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11;

    //player inventory slot
    //public PlayerInventory playerInventory;

    //when a object is detected, we will reference its  pickuppable script
    //change this back to non-static if needed
    public PickUppable pickUppableScript;

    public IngredientShelf shelfScript;

    //bool to check if player is holding an object, if they are then, do not assign new object
    //public bool holdingPickedUpObject = false;

    public float dist;

    //when the player detects an object, this object will be set as the hit object
    public GameObject detectedObject;

    //public GameObject pickedUpIngredient;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //look for an object to detect every frame
        DetectObject();

        //Item count in inventory
        Debug.Log("Current inventory: " + PickUppable.objectsInInventory.Count);


        //check distance between hit object and player

        if (detectedObject)
        {
            dist = Vector3.Distance(detectedObject.transform.position, transform.position);
            // Debug.Log("PlayerRader: Distance from " + PickUppable.pickedUpObject + " is " + dist);
        }



        if (dist >= 3)
        {
            detectedObject = null;
            pickUppableScript = null;
            shelfScript = null;
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
                //if it is the ingredient shelf, reference that script
                //Layer 14 is the ingredient shelf layer
                if(hit.collider.gameObject.layer == 14)
                {
                    Debug.Log("PlayerRadar: Object hit is an ingredient shelf!");

                    //Set the shelf as the picked up object first
                    detectedObject = hit.collider.gameObject;

                    shelfScript = detectedObject.GetComponent<IngredientShelf>();
                }
                else if(hit.collider.gameObject.layer == 17)
                {
                    Debug.Log("PlayerRadar: Object hit is a pickuppable! " + hit.collider.gameObject);

                    
                    //set the picked up object to be the hit gameobject
                    detectedObject = hit.collider.gameObject;

                    //reference pickuppable script
                    pickUppableScript = detectedObject.GetComponent<PickUppable>();
                }

                //there is now a picked up object that the player is holding
                //holdingPickedUpObject = true;

            }
            else
            {
                //if there is something in the inventory, do not allow referencing another script
                Debug.LogWarning("PlayerRadar: Player is already holding something!");
            }

        }

        //how can i make the player keep a reference to the pickedup object when they are holding it
        //and make it null when there is nothing in front of them
        //what to do about the sink? should i try changing its logic?

        //REMOVE IN FINAL BUILD
        else if(!hitObject) 
        {
            //draw a white ray
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * objectRadar, Color.white);
            Debug.Log("PlayerRadar: Did not hit anything");

        }

    }

    
}