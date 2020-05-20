using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to player
//Finds what object they are currently looking at and returns its name/tag
//will ignore the ground/environment and the player capsule
public class PlayerRadar : MonoBehaviour
{

    public float objectRadar; //How far the ray must be casted

    //shift layer bits to 8 and 9 (player and environment and dropzone and pickedup objects)
    private int ignoreLayers = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11;

    public PlayerInventory playerInventory;

    [SerializeField] private PickUppable pickUppable;

    //Bool to check if object can be picked up
    [SerializeField] bool canPickUpObject = true;

    //Bool to check if object should be dropped
    [SerializeField] bool canDropObject = false;

    //Bool to check if object can be placed in sink
    [SerializeField] bool canPlaceObjectInSink = false;

    //Bool to check if object can be washed
    [SerializeField] bool canWashObject = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: remove from update
        DetectObject();
        if(pickUppable != null)
        {
            HandleObjectStates();
        }

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
        }
        else //did not hit any object
        {
            //draw a white ray
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * objectRadar, Color.white);
            Debug.Log("PlayerRadar: Did not hit anything");
            //PickUppable.pickedUpObject = null;
            
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

    #region HandleButtonPress

    //FUNCTION TO CHECK IF BUTTON CAN BE PRESSED

    public void HandlePickUpObject()
    {
        //if inventory is empty, and can pick up object, and object is not on the sink
        if (!IsInventoryFull() && canPickUpObject)
        {
            //if there is a pickedup object and it belongs to layer 12 (plate on sink) do not pick it up
            if(PickUppable.pickedUpObject != null && PickUppable.pickedUpObject.layer == 12)
            {
                //if object is on the sink, do not pick up
                return;
            }
            pickUppable.PickUpObject(); //function to pick up object
            canDropObject = true; //object can be dropped
        }
        else
        {
            //Inventory full do not pick up
            Debug.Log("PlayerRadar: Inventory is full");
        }
    }

    //attached to button
    //if object can be dropped, then call the function
    public void HandleDropObject()
    {
        //check if near sink
        if (canDropObject && !canPickUpObject)
        {
            pickUppable.DropObject();
            canDropObject = false;
        }
        else
        {
            Debug.LogWarning("PlayerRadar: Nothing to drop");
        }
    }

    public void HandlePlaceObjectInSink()
    {
        //Player can place the object in the sink
        if(canPlaceObjectInSink && PickUppable.pickedUpObject.tag == "DirtyPlate")
        {
            pickUppable.PlaceObjectInSink();
        }
        else
        {
            Debug.LogWarning("PlayerRadar: Unable to place object in sink");
        }
    }

    public void HandleWashObject()
    {
        if (canWashObject && PickUppable.pickedUpObject.tag == "DirtyPlate")
        {
            pickUppable.WashObject();
            //change object state
            pickUppable.objectState = PickUppable.ObjectState.Washing;
        }
        else
        {
            Debug.LogWarning("PlayerRadar: Unable to wash object");
        }
    }

    #endregion

    public void HandleObjectStates()
    {
        //Switch statement to check object state and allow player to do different things depending on the state
        switch (pickUppable.objectState)
        {
            case PickUppable.ObjectState.PickUppable:
                Debug.Log("PlayerRadar: The object is currently pickuppable");
                //if object can be picked up
                canPickUpObject = true;
                break;

            case PickUppable.ObjectState.Droppable:
                Debug.Log("PlayerRadar: The object can be dropped");
                //if object can be dropped
                canPickUpObject = false;
                canPlaceObjectInSink = false;
                break;

            case PickUppable.ObjectState.PlaceInSink:
                Debug.Log("PlayerRadar: The object can be placed in the sink");
                //if object can be palced in sink
                canDropObject = false;
                canPlaceObjectInSink = true;
                break;

            case PickUppable.ObjectState.Washable:
                Debug.Log("PlayerRadar: The object can be washed");
                //if object can be washed
                canPlaceObjectInSink = false;
                canWashObject = true;
                canPickUpObject = false;
                break;

            case PickUppable.ObjectState.Washing:
                Debug.Log("PlayerRadar: The object is being washed");
                //if object is being washed
                break;

            case PickUppable.ObjectState.Washed:
                Debug.Log("PlayerRadar: The object has been washed");
                //if object is done washing
                canPlaceObjectInSink = false;
                canWashObject = false;
                canDropObject = false;
                canPickUpObject = true;
                break;

            case PickUppable.ObjectState.StoppedWashing:
                Debug.Log("Player Radar: Player left the sink and stopped washing");
                //cannot wash, cannot place object
                canPlaceObjectInSink = false;
                canWashObject = false;
                canPickUpObject = true;
                break;
        }
    }

    //On trigger enter, if the zone is the sink zone
    //Check if pickedup object tag is dirty plates
    //Set state as washable
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "SinkZone")
        {

            //if it was washing before this, go straight to washable state
            if (pickUppable.objectState == PickUppable.ObjectState.StoppedWashing)
            {
                pickUppable.objectState = PickUppable.ObjectState.Washable;
                Debug.Log("Player Radar: Press button to resume washing");
            }

            Debug.Log("PlayerRadar: Near Sink");

            //Do not allow dropping of object when near sink
            canDropObject = false;

            //Careful!!! If the object is active, then this will be detected twice
            if(PickUppable.pickedUpObject != null)
            {
                if (PickUppable.pickedUpObject.tag == "DirtyPlate" && !pickUppable.wasWashing)
                {
                    Debug.Log("PlayerRader: Washable object detected");

                    //Set state to ableto place in sink
                    pickUppable.objectState = PickUppable.ObjectState.PlaceInSink;
                }
            }
            
        }
    }

    //on trigger stay, if zone is sink zone
    //if washable, show icon
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "SinkZone")
        {
            //if it was washing before this, go straight to washable state
            if (pickUppable.objectState == PickUppable.ObjectState.StoppedWashing)
            {
                pickUppable.objectState = PickUppable.ObjectState.Washable;
                Debug.Log("Player Radar: Press button to resume washing");
            }
        }
        
    }

    //On trigger exit, if zone is sink zone
    private void OnTriggerExit(Collider other)
    {
        //if exit sink zone
        if(other.tag == "SinkZone")
        {

            if (pickUppable.wasWashing)
            {
                //if was washing, then set state
                pickUppable.objectState = PickUppable.ObjectState.StoppedWashing;
            }
            

            if (IsInventoryFull())
            {
                //if inventory full, allow to drop object
                canDropObject = true;
                //Set state to droppable 
                pickUppable.objectState = PickUppable.ObjectState.Droppable;
            }
            Debug.Log("Player has exited sink");
            

            if (PickUppable.pickedUpObject != null)
            {
                if (PickUppable.pickedUpObject.tag == "DirtyPlate")
                {
                    Debug.Log("PlayerRadar: Washable object not in sink zone");

                }
            }
            
        }
    }
}
