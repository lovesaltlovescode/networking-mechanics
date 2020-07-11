using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Networked player interaction
/// Player raycast for detected object
/// Depending on which object it is, change the function
/// </summary>

//enums of ingredients that could be spawned, placed outside class so it can be accessed elsewhere
public enum HeldItem
{
    nothing,
    egg,
    chicken,
    cucumber,
    rice,

    //Plates
    dirtyplate,
    cleanplate
}

//State of player
public enum PlayerState
{
    Default,

    CanDropIngredient,
    CanPickUpIngredient,
    CanThrowIngredient,

    //Spawning ingredients from shelf
    CanSpawnChicken,
    CanSpawnEgg,
    CanSpawnCucumber,
    CanSpawnRice,

    //Table items
    CanPickUpDirtyPlate,
    HoldingDirtyPlate,

    //Wash interaction
    CanPlacePlateInSink,
    ExitedSink,
    CanWashPlate,
    WashingPlate,
    StoppedWashingPlate,
    FinishedWashingPlate
}

public class NetworkedPlayerInteraction : NetworkBehaviour
{

    [Header("Spawnable Objects")]
    public GameObject objectContainerPrefab;

    //PREFABS to be spawned
    [Header("Ingredients")]
    public GameObject cucumberPrefab;
    public GameObject eggPrefab;
    public GameObject chickenPrefab;
    public GameObject ricePrefab;

    [Header("Plates")]
    public GameObject dirtyPlatePrefab;
    public GameObject cleanPlatePrefab;


    //RAYCAST VARIABLES
    [Header("Raycast Variables")]
    public float raycastLength = 1.5f; //how far the raycast extends

    [SerializeField] private float distFromObject; //distance from the object looking at

    //mask these layers, they do not need to be raycasted
    //Player, environment, zones and uninteractable layers
    private int layerMask = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 13;

    //Detected object, object player is looking at
    public GameObject detectedObject;

    //player attachment point
    public GameObject attachmentPoint;
    //player drop point, where items should be dropped
    public GameObject dropPoint;

    public List<GameObject> objectsInInventory = new List<GameObject>();

    //different scripts to reference
    [SerializeField] private NetworkedIngredientInteraction networkedIngredientInteraction;
    [SerializeField] private NetworkedWashInteraction networkedWashInteraction;

    public PlayerState playerState;

    private void Awake()
    {
        networkedIngredientInteraction = GetComponent<NetworkedIngredientInteraction>();
        networkedWashInteraction = GetComponent<NetworkedWashInteraction>();
    }


    //bool to check if inventory is full
    public bool IsInventoryFull()
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

            if (distFromObject >= raycastLength)
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
            Debug.Log("NetworkedPlayer - Object has been found! \n Object tag is: " + hit.collider.tag);

            //if nothing in inventory
            if (objectsInInventory.Count == 0)
            {
                //set hit object as detectedobject
                detectedObject = hit.collider.gameObject;
            }
            else
            {
                //Throw a warning
                Debug.LogWarning("NetworkedPlayer - Detected object already has a reference!");
            }


            //returns the detectedobject's layer (number) as a name
            //Debug.Log("NetworkedPlayer - Detected object layer: " + LayerMask.LayerToName(detectedObject.layer) + " of layer " + detectedObject.layer);
        }
        else
        {
            //no object hit
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
            Debug.Log("NetworkedPlayer - No object found");

        }
    }


    //Interact button method
    //Changes based on which state the player is
    public void InteractButton()
    {
        switch (playerState)
        {
            case PlayerState.CanSpawnChicken:
                Debug.Log("NetworkedPlayerInteraction - Spawn a chicken!");
                networkedIngredientInteraction.UpdateIngredient(HeldItem.chicken);
                break;

            case PlayerState.CanSpawnEgg:
                Debug.Log("NetworkedPlayerInteraction - Spawn a chicken!");
                networkedIngredientInteraction.UpdateIngredient(HeldItem.egg);
                break;

            case PlayerState.CanSpawnCucumber:
                Debug.Log("NetworkedPlayerInteraction - Spawn a chicken!");
                networkedIngredientInteraction.UpdateIngredient(HeldItem.cucumber);
                break;

            case PlayerState.CanSpawnRice:
                Debug.Log("NetworkedPlayerInteraction - Spawn some rice!");
                networkedIngredientInteraction.UpdateIngredient(HeldItem.rice);
                break;

            case PlayerState.CanDropIngredient:
                Debug.Log("NetworkedPlayerInteraction - Drop the ingredient!");
                networkedIngredientInteraction.DropIngredient();
                break;

            case PlayerState.CanPickUpIngredient:
                Debug.Log("NetworkedPlayerInteraction - Pick up the ingredient!");
                networkedIngredientInteraction.PickUpIngredient();
                break;

            case PlayerState.CanThrowIngredient:
                Debug.Log("NetworkedPlayerInteraction - Throw the ingredient!");
                networkedIngredientInteraction.ThrowIngredient();
                break;

            //PLATES
            case PlayerState.CanPickUpDirtyPlate:
                Debug.Log("NetworkedPlayerInteraction - Pick up the dirty plate!");
                networkedIngredientInteraction.PickUpPlate();
                break;

            case PlayerState.CanPlacePlateInSink:
                Debug.Log("NetworkedPlayerInteraction - Place plate in sink!");
                networkedWashInteraction.PlacePlateInSink();
                break;

            case PlayerState.CanWashPlate:
                Debug.Log("NetworkedPlayerInteraction - Wash plate in sink!");
                networkedWashInteraction.WashPlate();
                break;
                

        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        DetectObjects();

        CheckPlayerStateAndInventory();

        //if (!detectedObject && !IsInventoryFull())
        //{
        //    playerState = PlayerState.Default;
        //}

    }

    public void CheckPlayerStateAndInventory()
    {
        //checks for inventory contents
        foreach (var inventoryObject in objectsInInventory)
        {
            Debug.Log("NetworkedPlayerInteraction - Inventory currently contains: " + inventoryObject);
        }

        ////check inventory count
        Debug.Log("NetworkedPlayerInteraction - Inventory count: " + objectsInInventory.Count);

        //checks for player state
        Debug.Log("NetworkedPlayerInteraction - Player state is currently: " + playerState);

        if (playerState == PlayerState.FinishedWashingPlate)
        {
            networkedWashInteraction.CmdFinishWashingPlate();
        }
    }
}
