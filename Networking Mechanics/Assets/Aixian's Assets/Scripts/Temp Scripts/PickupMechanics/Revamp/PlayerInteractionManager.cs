using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Master script for translating player methods into player input via buttons
/// Handles raycasting at objects and checks for which object it is looking at
/// Defines enum states for the player
/// Stores static variable of currentlyholding object
/// Initialises all other scripts
/// </summary>
public class PlayerInteractionManager : MonoBehaviour
{
    [SerializeField] private string customerTag = "Customer", dishTag = "Dish";
    [SerializeField] private string queueingCustomerLayer = "Queue", takeOrderLayer = "Ordering";

    #region unchanged variables
    //RAYCAST VARIABLES
    public float raycastLength = 2f; //how far the raycast extends

    [SerializeField] private float distFromObject; //distance from the object looking at

    //shift layer bits to 8 and 9 (player, environment, zones, uninteractable)
    //mask these layers, they do not need to be raycasted
    private int layerMask = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 13;

    public static GameObject detectedObject;

    //List of objects in player's inventory, static and same throughout all scripts
    public static List<GameObject> objectsInInventory = new List<GameObject>();

    //attach and drop off point on the player
    public Transform attachPoint;
    public Transform dropOffPoint;

    //SCRIPT INITIALISATIONS

    [Header("Reference to individual scripts")]
    private ShelfInteraction shelfInteraction;
    private IngredientInteraction ingredientInteraction;
    private TableInteraction tableInteraction;
    private WashInteraction washInteraction;
    #endregion
    private PlayerCustomerInteractionManager customerInteraction;
    //--------------------------------------------------------------note: I haven't changed the player inventory from a list to a variable

    //Player states
    public enum PlayerState
    {
        #region unchanged player states
        //Default state
        Default,

        //Spawning ingredients from shelf
        CanSpawnEgg,
        CanSpawnChicken,
        CanSpawnCucumber,
        CanSpawnRice,

        //Ingredients
        CanPickUpIngredient,
        CanDropIngredient,

        //Table items: plates, money etc.
        CanPickUpDirtyPlate,

        //Wash interaction
        CanPlacePlateInSink,
        ExitedSink,
        CanWashPlate,
        WashingPlate,
        StoppedWashingPlate,
        FinishedWashingPlate,
        #endregion

        //Customer Interaction
        CanPickUpCustomer,
        HoldingCustomer,
        CanTakeOrder,
        CanPickUpDish,
        HoldingOrder
    }

    public static PlayerState playerState;

    void Awake()
    {
        #region unchanged initialisation
        //initialise scripts    
        shelfInteraction = gameObject.GetComponent<ShelfInteraction>();
        ingredientInteraction = gameObject.GetComponent<IngredientInteraction>();
        tableInteraction = gameObject.GetComponent<TableInteraction>();
        washInteraction = gameObject.GetComponent<WashInteraction>();
        #endregion
        customerInteraction = gameObject.GetComponent<PlayerCustomerInteractionManager>();

        playerState = PlayerState.Default;
    }

    #region unchanged check inventory full method
    //bool to check if inventory is full
    public static bool IsInventoryFull()
    {

        if (objectsInInventory.Count >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }

        
    }
    #endregion

    //Raycast function
    public void DetectObjects()
    {
        #region unchanged (raycast stuff)
        //check for distance from detected object
        if (detectedObject)
        {
            distFromObject = Vector3.Distance(detectedObject.transform.position, transform.position);

            if (distFromObject >= 2)
            {
                detectedObject = null;
            }
        }

        Debug.Log("PlayerInteractionManager - Player state is currently: " + playerState);

        RaycastHit hit;

        //Raycast from the front of player for specified length and ignore layers on layermask
        bool foundObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastLength, ~layerMask);
        #endregion

        //if an object was found
        if (foundObject)
        {
            #region unchanged
            //draw a yellow ray from object position (origin) forward to the distance of the cast 
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastLength, Color.yellow);
            //Debug.Log("PlayerInteractionManager - Object has been found! \n Object tag is: " + hit.collider.tag);
            #endregion
            //if nothing in inventory
            if(objectsInInventory.Count == 0)
            {
                //set hit object as detectedobject
                detectedObject = hit.collider.gameObject;

                //If the detected obj is a customer that is queueing, change the player state to canpickupcustomer
                if (hit.collider.gameObject.CompareTag(customerTag) && hit.collider.gameObject.layer == LayerMask.NameToLayer(queueingCustomerLayer))
                {
                    playerState = PlayerState.CanPickUpCustomer;

                }
                else if (hit.collider.gameObject.CompareTag(dishTag)) //if the detected obj is a dish, change the player state to canpickupobj
                {
                    playerState = PlayerState.CanPickUpDish;

                }
            }
            else
            {
                if(!CanChangePlayerState())
                {
                    //set hit object as detectedobject
                    detectedObject = hit.collider.gameObject;
                    Debug.Log("detected object: " + detectedObject.tag);
                }
                else
                {
                    //Throw a warning
                    Debug.LogWarning("PlayerInteractionManager - Detected object already has a reference!");
                }

            }

            //if the player is looking at a table ready to order, set their state to cantakeorder
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer(takeOrderLayer) && !IsInventoryFull())
            {
                Debug.Log("Inventory full, cannot take order");
                playerState = PlayerState.CanTakeOrder;

                //set hit object as detectedobject
                detectedObject = hit.collider.gameObject;
                Debug.Log("detected object: " + detectedObject.tag);
            }
            
        }
        #region unchanged (draw ray if no object was hit)
        else
        {
            //no object hit
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
            Debug.Log("PlayerInteractionManager -No object found");
            
        }
        //Debug.Log(!detectedObject); //return true if no detected object
        #endregion
    }

    public void InteractButton()
    {
        //switch cases to check which function to run at which state
        switch (playerState)
        {
            #region unchanged (all your prev player states)
            //spawning ingredients from shelves states
            case PlayerState.CanSpawnEgg:
                shelfInteraction.SpawnEgg(detectedObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.CanSpawnChicken:
                shelfInteraction.SpawnChicken(detectedObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.CanSpawnCucumber:
                shelfInteraction.SpawnCucumber(detectedObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.CanSpawnRice:
                shelfInteraction.SpawnRice(detectedObject, objectsInInventory, attachPoint);
                break;
                


            case PlayerState.CanPickUpIngredient:
                ingredientInteraction.PickUpIngredient(detectedObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.CanDropIngredient:
                ingredientInteraction.DropIngredient(detectedObject, objectsInInventory, dropOffPoint);
                break;

            case PlayerState.CanPickUpDirtyPlate:
                tableInteraction.PickUpTableItem(detectedObject, objectsInInventory, attachPoint);
                break;

            //Washing plate states
            case PlayerState.CanPlacePlateInSink:
                washInteraction.PlacePlateInSink(detectedObject, objectsInInventory);
                break;

            case PlayerState.CanWashPlate:
                washInteraction.WashDirtyPlate();
                break;

            #endregion

            //customer interaction states
            case PlayerState.CanPickUpCustomer:
                customerInteraction.PickCustomerUp(detectedObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.HoldingCustomer:
                customerInteraction.SeatCustomer(objectsInInventory, detectedObject);
                break;

            case PlayerState.CanTakeOrder:
                Debug.Log("can take order player state");
                customerInteraction.CheckHandsEmpty(objectsInInventory, detectedObject);
                break;

            case PlayerState.CanPickUpDish:
                Debug.Log("dish can be picked up");
                customerInteraction.PickOrderUp(detectedObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.HoldingOrder:
                Debug.Log("Holding order player state");
                customerInteraction.CheckCanPutDownOrder(objectsInInventory, detectedObject, dropOffPoint);
                break;

            default:
                Debug.Log("default case");
                break;
        }
    }

    #region unchanged
    // Update is called once per frame
    void Update()
    {
        DetectObjects();

        CheckPlayerStateAndInventory();
    }

    public void CheckPlayerStateAndInventory()
    {/*
        //checks for inventory contents
        foreach (var inventoryObject in objectsInInventory)
        {
            Debug.Log("PlayerInteractionManager - Inventory currently contains: " + inventoryObject);
        }

        ////check inventory count
        //Debug.Log("Inventory count: " + objectsInInventory.Count);
        
        //checks for player state
        Debug.Log("PlayerInteractionManager - Player state is currently: " + playerState);
        */
        if (playerState == PlayerState.FinishedWashingPlate)
        {
            washInteraction.FinishWashingPlate();
        }
    }
    #endregion

    //checks whether the playerstate is something that should not be changed
    public static bool CanChangePlayerState()
    {
        if(playerState == PlayerState.HoldingCustomer || playerState == PlayerState.HoldingOrder)
        {
            return false;
        } 
        else
        {
            return true;
        }
    }

    //changes the player state
    public static void ChangePlayerState(PlayerState newPlayerState, bool canBypassCheck = false)
    {
        if (CanChangePlayerState() || canBypassCheck)
        {
            playerState = newPlayerState;
        } 
        else
        {
            Debug.Log("cannot change playerstate to " + newPlayerState);
        }
    }

}
