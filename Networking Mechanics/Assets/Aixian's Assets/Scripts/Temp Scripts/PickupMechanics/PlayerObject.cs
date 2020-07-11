using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Checks for collisions/trigger enters and calls functions
//Houses functions for interact button and checks for the state of the picked up object

public class PlayerObject : MonoBehaviour
{
    //reference pickuppable object scripts
    [SerializeField] private PickUppable playerPickUppable;

    [SerializeField] private IngredientShelf playerShelfScript;

    //Use this reference if static variable cannot be used
    public PlayerRadar playerRadar;


    #region Pickuppable Object Booleans

    //Bool to check if object can be picked up
    [SerializeField] bool canPickUpObject = true;

    //Bool to check if object should be dropped
    [SerializeField] bool canDropObject = false;

    //Bool to check if object can be placed on ingredient table
    [SerializeField] bool canPlaceObjectOnIngredientTable = false;

    //Bool to check if object can be placed in sink
    [SerializeField] bool canPlaceObjectInSink = false;

    //Bool to check if object can be washed
    [SerializeField] bool canWashObject = false;

    //Bool to check if an ingredient can be spawned 
    [SerializeField] bool canSpawnIngredient = false;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerPickUppable = playerRadar.pickUppableScript;

        playerShelfScript = playerRadar.shelfScript;
       

        if (playerPickUppable != null)
        {
            HandleObjectStates();
        }

        //DEBUGGING
        if (Input.GetKeyDown(KeyCode.W))
        {
           // playerPickUppable.SpawnIngredient();
        }

        if (playerShelfScript.ingredientObject)
        {
            playerRadar.detectedObject = playerShelfScript.ingredientObject;
        }
        


        playerRadar.pickUppableScript = playerRadar.detectedObject.GetComponent<PickUppable>();
        Debug.Log("Looking for object");


        //Check if can spawn ingredient

            if (!canDropObject)
            {
                canSpawnIngredient = true;
            }
        
        



    }

    //function to return the value of if inventory is full
    public bool IsInventoryFull()
    {
        //returns true if inventory is full
        if (PickUppable.objectsInInventory.Count >= 1) //if 1 object or more, then it is full 
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

    #region ObjectFunctions

    //If ingredient shelf, spawn an ingredient
    //If not ingredient shelf, pick up the object 
    public void HandleSpawnIngredient()
    {
        if (canSpawnIngredient)
        {

            playerShelfScript.SpawnIngredient();

            canDropObject = true;
            canSpawnIngredient = false;
            canPickUpObject = false;
            

        }
    }


    //public void HandlePickUpSpawnObject()
    //{
    //    //if inventory is not full and there is a picked up object
    //    if (!IsInventoryFull())
    //    {
    //        //if facing ingredient shelf
    //        if (playerRadar.facingShelf && canSpawnIngredient)
    //        {
    //            Debug.Log("Facing shelf, spawn ingredient");
    //            playerPickUppable.SpawnIngredient();
    //            canDropObject = true;
    //        }

    //        //if facing pickuppable
    //        else if (playerRadar.facingPickuppable)
    //        {
    //            if (canPickUpObject)
    //            {
    //                //if it belongs to layer 12
    //                if(playerRadar.detectedObject && playerRadar.detectedObject.layer == 12)
    //                {
    //                    return;
    //                }
    //                else
    //                {
    //                    playerPickUppable.PickUpObject(); //pick up object
    //                    canDropObject = true; //object can now be dropped
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("PlayerRadar: Unable to pick up");
    //    }
    //}

    //attached to button
    //if object can be dropped, then call the function

    public void HandleDropObject()
    {

        if (canDropObject && !canPickUpObject)
        {
            playerPickUppable.DropObject();
            Debug.Log("PlayerObject: Dropped an object");
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
           
        if (canPlaceObjectInSink && playerRadar.detectedObject.tag == "DirtyPlate")
            
        {
            playerPickUppable.PlaceInSink();
           
        }
        else    
        {
           
            //Debug.LogWarning("PlayerRadar: Unable to place object in sink")     
        }
        
       
    }

    public void HandleWashObject()
    {

            if (canWashObject && playerRadar.detectedObject.tag == "DirtyPlate")
            {
                playerPickUppable.WashObject();
                //change object state
                playerPickUppable.objectState = PickUppable.ObjectState.Washing;
            }
            else
            {
                Debug.LogWarning("PlayerRadar: Unable to wash object");
            }
        
        
    }

    public void HandlePlaceIngredient()
    {

            if (canPlaceObjectOnIngredientTable == true)
            {
                playerPickUppable.PlaceOnTable();
            }
            else
            {
                Debug.LogWarning("PlayerRadar: Unable to place ingredient");
            }
        
        
    }

    #endregion


    #endregion

    public void HandleObjectStates()
    {
        //Switch statement to check object state and allow player to do different things depending on the state
        switch (playerPickUppable.objectState)
        {

            case PickUppable.ObjectState.PickUppable:
                Debug.Log("PlayerRadar: The object is currently pickuppable");
                //if object can be picked up
                break;

            case PickUppable.ObjectState.Droppable:
                Debug.Log("PlayerRadar: The object can be dropped");
                //if object can be dropped
                canPickUpObject = false;
                canPlaceObjectInSink = false;
                break;

            case PickUppable.ObjectState.PlaceOnIngredientTable:
                Debug.Log("PlayerRadar: Object can be placed on ingredient table");
                canPlaceObjectOnIngredientTable = true;
                canDropObject = false;
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

            case PickUppable.ObjectState.UnInteractable:
                Debug.Log("PlayerRader: Object cannot be interacted with");
                canPlaceObjectOnIngredientTable = false;
                canPickUpObject = false;
                canDropObject = false;
                break;
        }
    }

    //On trigger enter, if the zone is the sink zone
    //Check if pickedup object tag is dirty plates
    //Set state as washable
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SinkZone")
        {

            //if it was washing before this, go straight to washable state
            if (playerPickUppable.objectState == PickUppable.ObjectState.StoppedWashing)
            {
                playerPickUppable.objectState = PickUppable.ObjectState.Washable;
                Debug.Log("Player Radar: Press button to resume washing");
            }

            Debug.Log("PlayerRadar: Near Sink");

            //Do not allow dropping of object when near sink
            canDropObject = false;
            Debug.Log("Picked up object current tag: " + playerRadar.detectedObject.tag);

            //Careful!!! If the object is active, then this will be detected twice
            if (playerRadar.detectedObject)
            {
                if (playerRadar.detectedObject.tag == "DirtyPlate" && !playerPickUppable.wasWashing)
                {
                    Debug.Log("PlayerRader: Washable object detected");


                    //Set state to ableto place in sink
                    playerPickUppable.objectState = PickUppable.ObjectState.PlaceInSink;
                }
            }

        }
    }

    //on trigger stay, if zone is sink zone
    //if washable, show icon
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "SinkZone")
        {
            //if it was washing before this, go straight to washable state
            if (playerPickUppable != null)
            {
                if (playerPickUppable.objectState == PickUppable.ObjectState.StoppedWashing)
                {
                    playerPickUppable.objectState = PickUppable.ObjectState.Washable;
                    Debug.Log("Player Radar: Press button to resume washing");
                }
            }

        }

    }

    //On trigger exit, if zone is sink zone
    private void OnTriggerExit(Collider other)
    {
        //if exit sink zone
        if (other.tag == "SinkZone")
        {

            playerPickUppable.washIcon.gameObject.SetActive(false);

            if (playerPickUppable.wasWashing)
            {
                //if was washing, then set state
                playerPickUppable.objectState = PickUppable.ObjectState.StoppedWashing;
            }


            if (IsInventoryFull())
            {
                //if inventory full, allow to drop object
                canDropObject = true;
                //Set state to droppable 
                playerPickUppable.objectState = PickUppable.ObjectState.Droppable;
            }
            Debug.Log("Player has exited sink");


            if (playerRadar.detectedObject != null)
            {
                if (playerRadar.detectedObject.tag == "DirtyPlate")
                {
                    Debug.Log("PlayerRadar: Washable object not in sink zone");

                }
            }

        }
    }
}
