using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to player
//Finds what object they are currently looking at and returns its name/tag
//will ignore the ground/environment and the player capsule
public class PlayerRadar : MonoBehaviour
{

    public float objectRadar; //How far the ray must be casted

    //shift layer bits to 8 and 9 (player and environment and dropzone)
    private int ignoreLayers = 1 << 8 | 1 << 9 | 1 << 10;

    public PlayerInventory playerInventory;

    [SerializeField] private PickUppable pickUppable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: remove from update
        DetectObject();

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
            //Return hit object tag
            Debug.Log("PlayerRadar: Hit " + hit.collider.tag);

            //if there is nothing currently in the inventory
            if(PickUppable.objectsInInventory.Count == 0)
            {
                //reference the pickuppable script that belongs to the object hit
                pickUppable = hit.collider.gameObject.GetComponent<PickUppable>();
            }
            else
            {
                //do not allow referencing another script
                Debug.LogWarning("PlayerRadar: PickUppable already has a reference!");
            }


            //set the picked up object to be the hit gameobject
            PickUppable.pickedUpObject = hit.collider.gameObject;
            HandleObjectStates(); //Always check state of object when hit

            if (!IsInventoryFull())
            {
                pickUppable.PickUpObject(); //function to pick up object
            }
            else
            {
                //Inventory full do not pick up
                Debug.Log("PlayerRadar: Inventory is full");
            }


        }
        else
        {
            //draw a white ray
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * objectRadar, Color.white);
            Debug.Log("PlayerRadar: Did not hit anything");
        }

    }

    //function to return the value of if inventory is full
    public bool IsInventoryFull()
    {
        //returns true if inventory is full
        if(PickUppable.objectsInInventory.Count >= 1) //if 1 object or more, then it is full 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void HandleObjectStates()
    {
        //Switch statement to check object state and allow player to do different things depending on the state
        switch (pickUppable.objectState)
        {
            case PickUppable.ObjectState.PickUppable:
                Debug.Log("PlayerRadar: The object is currently pickuppable");
                //some function here
                break;

            case PickUppable.ObjectState.Droppable:
                Debug.Log("PlayerRadar: The object can be dropped");
                //some function to drop the object
                break;
        }
    }
   
}
