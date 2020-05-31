using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Checks for collisions/trigger enters and calls functions
//Houses functions for interact button and checks for the state of the picked up object

public class PlayerObject : MonoBehaviour
{
    //reference pickuppable object scripts
    [SerializeField] private PickUppable playerPickUppable;


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

    #endregion

   



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerPickUppable = playerRadar.pickUppable;
        playerIngredientShelf = playerRadar.ingredientShelf;

        if (playerPickUppable != null)
        {
            HandleObjectStates();
        }

        if(playerIngredientShelf != null)
        {
            HandleIngredientStates();
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

    #region IngredientFunctions

    public void HandleSpawnIngredient()
    {
        //If inventory is empty and can spawn ingredient
        //When button pressed, call function for pickupobject

        if (playerRadar.facingIngredient)
        {

            if (!IsInventoryFull() && canSpawnIngredient)
            {
                playerIngredientShelf.PickUpObject();
                canDropIngredient = true;
            }
            {
                Debug.Log("PlayerRadar: Inventory is full, cannot pick up");
            }
        }
    }

    public void DropIngredient()
    {
        if (playerRadar.facingIngredient)
        {

            //check if near sink
            if (canDropIngredient && !canSpawnIngredient)
            {
                playerIngredientShelf.DropObject();
                canDropIngredient = false;
            }
            else
            {
                Debug.LogWarning("PlayerRadar: Nothing to drop");
            }
        }
    }

    public void PlaceIngredientOnTable()
    {
        if (playerRadar.facingIngredient)
        {
            if (canPlaceIngredientOnTable == true)
            {
                playerIngredientShelf.PlaceOnTable();
            }
            else
            {
                Debug.LogWarning("PlayerRadar: Unable to place ingredient on table");
            }
        }
        
    }

    #endregion

    #region ObjectFunctions

    public void HandlePickUpObject()
    {
        if (playerRadar.facingPickuppable)
        {
            //if inventory is empty, and can pick up object, and object is not on the sink
            if (!IsInventoryFull() && canPickUpObject)
            {
                //if there is a pickedup object and it belongs to layer 12 (plate on sink) do not pick it up
                if (playerPickUppable.pickedUpObject != null && playerPickUppable.pickedUpObject.layer == 12)
                {
                    //if object is on the sink, do not pick up
                    return;
                }

                playerPickUppable.PickUpObject(); //function to pick up object
                canDropObject = true; //object can be dropped
            }
            else
            {
                //Inventory full do not pick up
                Debug.Log("PlayerRadar: Inventory is full");
            }
        }
        
    }

    //attached to button
    //if object can be dropped, then call the function
    public void HandleDropObject()
    {
        if (playerRadar.facingPickuppable)
        {
            //check if near sink
            if (canDropObject && !canPickUpObject)
            {
                playerPickUppable.DropObject();
                canDropObject = false;
            }
            else
            {
                Debug.LogWarning("PlayerRadar: Nothing to drop");
            }
        }
        
    }


    public void HandlePlaceObjectInSink()
    {
        if (playerRadar.facingPickuppable)
        {
            //Player can place the object in the sink
            if (canPlaceObjectInSink && playerPickUppable.pickedUpObject.tag == "DirtyPlate")
            {
                playerPickUppable.PlaceInSink();
            }
            else
            {
                //Debug.LogWarning("PlayerRadar: Unable to place object in sink");
            }
        }
       
    }

    public void HandleWashObject()
    {
        if (playerRadar.facingPickuppable)
        {
            if (canWashObject && playerPickUppable.pickedUpObject.tag == "DirtyPlate")
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
        
    }

    public void HandlePlaceIngredient()
    {
        if (playerRadar.facingPickuppable)
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
                canPickUpObject = true;
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

    public void HandleIngredientStates()
    {
        //switch statement to handle different states of the ingredient
        switch (playerIngredientShelf.ingredientState)
        {
            case IngredientShelf.IngredientState.Spawnable:
                Debug.Log("PlayerRadar: Spawn an ingredient!");
                //if object can be picked up
                canSpawnIngredient = true;
                break;

            case IngredientShelf.IngredientState.Droppable:
                //if object can be dropped
                canSpawnIngredient = false;
                canDropIngredient = true;
                break;

            case IngredientShelf.IngredientState.PlaceOnIngredientTable:
                Debug.Log("PlayerRadar: Ingredient can be placed on ingredient table");
                canPlaceIngredientOnTable = true;
                canDropIngredient = false;
                break;

            case IngredientShelf.IngredientState.UnInteractable:
                Debug.Log("PlayerRader: Ingredient cannot be interacted with");
                canPlaceIngredientOnTable = false;
                canSpawnIngredient = false;
                canDropIngredient = false;
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
            Debug.Log("Picked up object current tag: " + playerPickUppable.pickedUpObject.tag);

            //Careful!!! If the object is active, then this will be detected twice
            if (playerPickUppable.pickedUpObject != null)
            {
                if (playerPickUppable.pickedUpObject.tag == "DirtyPlate" && !playerPickUppable.wasWashing)
                {
                    Debug.Log("PlayerRader: Washable object detected");


                    //Set state to ableto place in sink
                    playerPickUppable.objectState = PickUppable.ObjectState.PlaceInSink;
                }
            }

        }

        else if (other.tag == "IngredientTableZone")
        {
            //Player is near ingredient table
            Debug.Log("Player Radar: Player is near ingredient table");
            //do not allow dropping of object
            //canDropObject = false;

            //Careful!!! If the object is active, then this will be detected twice
                if (playerPickUppable.pickedUpObject.tag == "Ingredient")
                {
                    Debug.Log("PlayerRader: Placeable ingredient detected");

                    //Set state to able to place on table
                    playerPickUppable.objectState = PickUppable.ObjectState.PlaceOnIngredientTable;
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


            if (playerPickUppable.pickedUpObject != null)
            {
                if (playerPickUppable.pickedUpObject.tag == "DirtyPlate")
                {
                    Debug.Log("PlayerRadar: Washable object not in sink zone");

                }
            }

        }

        if (other.tag == "IngredientTableZone")
        {

            canPlaceObjectOnIngredientTable = false;

            if (IsInventoryFull())
            {
                //if inventory full, allow to drop object
                canDropObject = true;
                //Set state to droppable 
                playerPickUppable.objectState = PickUppable.ObjectState.Droppable;
            }
            Debug.Log("Player has exited ingredient table");

            if (playerPickUppable.pickedUpObject != null)
            {
                if (playerPickUppable.pickedUpObject.tag == "Ingredient")
                {
                    Debug.Log("PlayerRadar: Ingredient not in ingredient table zone");

                }
            }
        }
    }
}
