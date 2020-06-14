using System.Collections;
using System.Collections.Generic;
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
    //RAYCAST VARIABLES
    public float raycastLength = 2f; //how far the raycast extends

    [SerializeField] private float distFromObject; //distance from the object looking at

    //shift layer bits to 8 and 9 (player, environment, zones, uninteractable)
    //mask these layers, they do not need to be raycasted
    private int layerMask = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 13;

    public static GameObject detectedObject;

    //Static variable for currentlyheld object
    public static GameObject heldObject;

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
    

    //Player states
    public enum PlayerState
    {
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
        FinishedWashingPlate
    }

    public static PlayerState playerState;

    void Awake()
    {
        //initialise scripts    
        shelfInteraction = gameObject.GetComponent<ShelfInteraction>();
        ingredientInteraction = gameObject.GetComponent<IngredientInteraction>();
        tableInteraction = gameObject.GetComponent<TableInteraction>();
        washInteraction = gameObject.GetComponent<WashInteraction>();

        playerState = PlayerState.Default;
    }

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

    //Raycast function
    public void DetectObjects()
    {
        //check for distance from detected object
        if (detectedObject)
        {
            distFromObject = Vector3.Distance(detectedObject.transform.position, transform.position);

            if (distFromObject >= 2)
            {
                detectedObject = null;
            }
        }

      

        RaycastHit hit;

        //Raycast from the front of player for specified length and ignore layers on layermask
        bool foundObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastLength, ~layerMask);

        //if an object was found
        if (foundObject)
        {

            //draw a yellow ray from object position (origin) forward to the distance of the cast 
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastLength, Color.yellow);
            Debug.Log("PlayerInteractionManager - Object has been found! \n Object tag is: " + hit.collider.tag);
            
            //if nothing in inventory
            if(objectsInInventory.Count == 0)
            {
                //set hit object as detectedobject
                detectedObject = hit.collider.gameObject;
            }
            else
            {
                //Throw a warning
                Debug.LogWarning("PlayerInteractionManager - Detected object already has a reference!");
            }
            

            //returns the detectedobject's layer (number) as a name
            Debug.Log("PlayerInteractionManager - Detected object layer: " + LayerMask.LayerToName(detectedObject.layer) + " of layer " + detectedObject.layer);
        }
        else
        {
            //no object hit
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
            Debug.Log("PlayerInteractionManager -No object found");
            
        }
        //Debug.Log(!detectedObject); //return true if no detected object
    }

    public void InteractButton()
    {
        //switch cases to check which function to run at which state
        switch (playerState)
        {
            //spawning ingredients from shelves states
            case PlayerState.CanSpawnEgg:
                shelfInteraction.SpawnEgg(heldObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.CanSpawnChicken:
                shelfInteraction.SpawnChicken(heldObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.CanSpawnCucumber:
                shelfInteraction.SpawnCucumber(heldObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.CanSpawnRice:
                shelfInteraction.SpawnRice(heldObject, objectsInInventory, attachPoint);
                break;
                


            case PlayerState.CanPickUpIngredient:
                ingredientInteraction.PickUpIngredient(detectedObject, objectsInInventory, attachPoint);
                break;

            case PlayerState.CanDropIngredient:
                ingredientInteraction.DropIngredient(heldObject, objectsInInventory, dropOffPoint);
                break;

            case PlayerState.CanPickUpDirtyPlate:
                tableInteraction.PickUpTableItem(detectedObject, objectsInInventory, attachPoint);
                break;

            //Washing plate states
            case PlayerState.CanPlacePlateInSink:
                washInteraction.PlacePlateInSink(heldObject, objectsInInventory);
                break;

            case PlayerState.CanWashPlate:
                washInteraction.WashDirtyPlate();
                break;


        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectObjects();

        CheckPlayerStateAndInventory();
    }

    public void CheckPlayerStateAndInventory()
    {
        //checks for inventory contents
        foreach (var inventoryObject in objectsInInventory)
        {
            Debug.Log("PlayerInteractionManager - Inventory currently contains: " + inventoryObject);
        }

        ////check inventory count
        Debug.Log("Inventory count: " + objectsInInventory.Count);

        //checks for player state
        Debug.Log("PlayerInteractionManager - Player state is currently: " + playerState);

        if (playerState == PlayerState.FinishedWashingPlate)
        {
            washInteraction.FinishWashingPlate();
        }
    }


}
