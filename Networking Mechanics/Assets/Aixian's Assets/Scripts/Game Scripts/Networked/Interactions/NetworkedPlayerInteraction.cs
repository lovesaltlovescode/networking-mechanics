﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

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
    rotten,
    drink,

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

    //Rotten Ingredients
    CanPickUpRottenIngredient,
    HoldingRottenIngredient,

    //Spawning ingredients from shelf
    CanSpawnChicken,
    CanSpawnEgg,
    CanSpawnCucumber,
    CanSpawnRice,

    //Drinks
    CanSpawnDrink,
    CanPickUpDrink,
    HoldingDrink,
    CanUseDrink, //temp

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
    public GameObject rottenPrefab;
    public GameObject drinkPrefab;

    [Header("Plates")]
    public GameObject dirtyPlatePrefab;
    public GameObject cleanPlatePrefab;

    public GameObject tempObject;

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

    //player's inventory
    public GameObject playerInventory;

    //different scripts to reference
    [SerializeField] private NetworkedIngredientInteraction networkedIngredientInteraction;
    [SerializeField] private NetworkedWashInteraction networkedWashInteraction;
    [SerializeField] private NetworkedDrinkInteraction networkedDrinkInteraction;

    public PlayerState playerState;

    //when the helditem changes, call onchangeingredient method
    [SyncVar(hook = nameof(OnChangeHeldItem))]
    public HeldItem heldItem;


   


    #region SyncVar

    void OnChangeHeldItem(HeldItem oldItem, HeldItem newItem)
    {
        //Debug.Log("NetworkedIngredientInteraction - Starting coroutine!");
        StartCoroutine(ChangeHeldItem(newItem));
    }

    IEnumerator ChangeHeldItem(HeldItem newItem)
    {
        //If the player is holding something
        while (playerInventory)
        {
            //if player is holding nothing, destroy the existing child
            if (newItem == HeldItem.nothing)
            {
                Debug.Log("NetworkedIngredientInteraction - Destroying held object");
                Destroy(playerInventory);
            }
            //if player is holding something, do nothing
            //Debug.Log("NetworkedIngredient - Inventory is full!");
            yield return null;
        }

        //depending on which held item is being held by player (in update)
        //instantiate the corresponding prefab
        switch (newItem)
        {
            case HeldItem.chicken:
                var chicken = Instantiate(chickenPrefab, attachmentPoint.transform);
                playerInventory = chicken;
                chicken.tag = "Chicken";
                break;

            case HeldItem.egg:
                var egg = Instantiate(eggPrefab, attachmentPoint.transform);
                playerInventory = egg;
                egg.tag = "Egg";
                break;

            case HeldItem.cucumber:
                var cucumber = Instantiate(cucumberPrefab, attachmentPoint.transform);
                playerInventory = cucumber;
                cucumber.tag = "Cucumber";
                break;

            case HeldItem.rice:
                var rice = Instantiate(ricePrefab, attachmentPoint.transform);
                playerInventory = rice;
                rice.tag = "Rice";
                break;

            case HeldItem.dirtyplate:
                var dirtyPlate = Instantiate(dirtyPlatePrefab, attachmentPoint.transform);
                playerInventory = dirtyPlate;
                dirtyPlate.tag = "DirtyPlate";
                break;

            case HeldItem.rotten:
                var rotten = Instantiate(rottenPrefab, attachmentPoint.transform);
                playerInventory = rotten;
                rotten.tag = "RottenIngredient";
                break;

            case HeldItem.drink:
                var drink = Instantiate(drinkPrefab, attachmentPoint.transform);
                playerInventory = drink;
                drink.tag = "Drink";
                break;

        }
    }

    #endregion

    #region Commands

    //called on server, main function to change the held ingredient
    //change held ingredient to the new ingredient
    //triggers sync var -> Coroutine to instantiate the corresponding prefab
    [Command]
    public void CmdChangeHeldItem(HeldItem selectedIngredient)
    {
        //Debug.Log("RPC Spawning - Update held item");
        //Change ingredient the player is holding
        heldItem = selectedIngredient;
    }

    #endregion

    private void Awake()
    {
        networkedIngredientInteraction = GetComponent<NetworkedIngredientInteraction>();
        networkedWashInteraction = GetComponent<NetworkedWashInteraction>();
        networkedDrinkInteraction = GetComponent<NetworkedDrinkInteraction>();

    }



    //bool to check if inventory is full
    public bool IsInventoryFull()
    {

        if (playerInventory != null)
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
        if (!hasAuthority)
        {
            ////Debug.Log("NOT LOCAL PLAYER");
            return;
        }

        //check for distance from detected object
        if (detectedObject)
        {
            distFromObject = Vector3.Distance(detectedObject.transform.position, transform.position);

            if (distFromObject >= raycastLength && !IsInventoryFull())
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
            //Debug.Log("NetworkedPlayer - Object has been found! \n Object tag is: " + hit.collider.tag);
            //Debug.Log("NetworkedPlayer - Object has been found! \n Object tag is: " + hit.collider.gameObject.layer);

            //if nothing in inventory
            if (!playerInventory)
            {
                //set hit object as detectedobject
                detectedObject = hit.collider.gameObject;
            }
            else
            {
                //Throw a warning
                //Debug.LogWarning("NetworkedPlayer - Detected object already has a reference!");
            }


            //returns the detectedobject's layer (number) as a name
            //Debug.Log("NetworkedPlayer - Detected object layer: " + LayerMask.LayerToName(detectedObject.layer) + " of layer " + detectedObject.layer);
            //Debug.Log("Detected object: " + detectedObject.name);
        }
        else
        {
            //no object hit
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
            //Debug.Log("NetworkedPlayer - No object found");

        }
    }


    //Interact button method
    //Changes based on which state the player is
    public void InteractButton()
    {
        switch (playerState)
        {
            case PlayerState.CanSpawnChicken:
                //Debug.Log("NetworkedPlayerInteraction - Spawn a chicken!");
                networkedIngredientInteraction.SpawnIngredient(HeldItem.chicken);
                break;

            case PlayerState.CanSpawnEgg:
                //Debug.Log("NetworkedPlayerInteraction - Spawn a chicken!");
                networkedIngredientInteraction.SpawnIngredient(HeldItem.egg);
                break;

            case PlayerState.CanSpawnCucumber:
                //Debug.Log("NetworkedPlayerInteraction - Spawn a chicken!");
                networkedIngredientInteraction.SpawnIngredient(HeldItem.cucumber);
                break;

            case PlayerState.CanSpawnRice:
                //Debug.Log("NetworkedPlayerInteraction - Spawn some rice!");
                networkedIngredientInteraction.SpawnIngredient(HeldItem.rice);
                break;

            case PlayerState.CanDropIngredient:
                //Debug.Log("NetworkedPlayerInteraction - Drop the ingredient!");
                networkedIngredientInteraction.DropIngredient();
                break;

            case PlayerState.CanPickUpIngredient:
                //Debug.Log("NetworkedPlayerInteraction - Pick up the ingredient!");
                networkedIngredientInteraction.PickUpIngredient();
                break;

            case PlayerState.CanThrowIngredient:
                //Debug.Log("NetworkedPlayerInteraction - Throw the ingredient!");
                networkedIngredientInteraction.TrashIngredient();
                break;

            //ROTTEN INGREDIENT
            case PlayerState.CanPickUpRottenIngredient:
                //Debug.Log("NetworkedPlayerInteraction - Pick up rotten ingredient");
                networkedIngredientInteraction.PickUpRottenIngredient();
                break;

            //DRINKS
            case PlayerState.CanSpawnDrink:
                networkedDrinkInteraction.SpawnDrink();
                break;

            case PlayerState.CanPickUpDrink:
                networkedDrinkInteraction.PickUpDrink();
                break;


            //PLATES
            case PlayerState.CanPickUpDirtyPlate:
                //Debug.Log("NetworkedPlayerInteraction - Pick up the dirty plate!");
                networkedIngredientInteraction.PickUpPlate();
                break;

            case PlayerState.CanPlacePlateInSink:
                //Debug.Log("NetworkedPlayerInteraction - Place plate in sink!");
                networkedWashInteraction.PlacePlateInSink();
                break;

            case PlayerState.CanWashPlate:
                //Debug.Log("NetworkedPlayerInteraction - Wash plate in sink!");
                networkedWashInteraction.WashPlate();
                break;
                

        }
    }

    #region Master Methods


     #region DetectMethods

    public void DetectObject(GameObject detectedObject, int layer, Action callback)
    {
        //Check for detected object and if it is a certain layer
        if (detectedObject && detectedObject.layer == layer)
        {
            //detected = true;
            

            callback?.Invoke();
        }
    }

    public void PickUpObject(GameObject detectedObject, int layer, bool inventoryFull, PlayerState _playerState)
    {
        //pickuppable layer
        if (detectedObject && detectedObject.layer == layer)
        {
            //Debug.Log("ObjectContainer - Pickuppable ingredient detected!");

            if (!inventoryFull)
            {
                //if not holding anything, change state
                playerState = _playerState;
            }
        }
    }

    #endregion

    //called from client to server to pick up item
    //Pass in a detectedobject parameter so that the server knows which object to look for
    //It should be looking for client's local detectedobject, not the servers''s
    [Command]
    public void CmdPickUpObject(GameObject detectedObject)
    {
        //set player's syncvar so clients can show the right ingredient
        //according to which item the sceneobject currently contains
        //Debug.Log("Debugging ingredient - Part 3" + detectedObject);

        //destroy the scene object when it has been picked up, on the SERVER
        Debug.Log("Destroying detected object: " + detectedObject);
        NetworkServer.Destroy(detectedObject);
        //Debug.Log("//Debugging ingredient - Part 5");

    }

    [Command]
    public void CmdSpawnObject(Vector3 pos, Quaternion rot, HeldItem itemToSpawn, string layerName)
    {
        //instantiate scene object
        
        GameObject spawnObject = Instantiate(objectContainerPrefab, pos, rot);
        tempObject = spawnObject;

        //set rigidbody as non-kinematic
        spawnObject.GetComponent<Rigidbody>().isKinematic = false;

        ObjectContainer objectContainer = spawnObject.GetComponent<ObjectContainer>();

        //Instantiate the right held item as child of the object
        objectContainer.SetObjToSpawn(itemToSpawn);

        //sync var helditem in object container
        objectContainer.objToSpawn = itemToSpawn;

        //change layer
        spawnObject.layer = LayerMask.NameToLayer(layerName);

        //spawn on network
        NetworkServer.Spawn(spawnObject);

    }


    #endregion

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

        if (!playerInventory && attachmentPoint.transform.childCount > 0)
        {
            playerInventory = attachmentPoint.transform.GetChild(0).gameObject;
        }
        else if (!playerInventory)
        {
            playerInventory = null;
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
        //Debug.Log("NetworkedPlayerInteraction - Inventory currently contains: " + playerInventory);

        //////check inventory count
        ////Debug.Log("NetworkedPlayerInteraction - Inventory count: " + objectsInInventory.Count);

        //checks for player state
        //Debug.Log("NetworkedPlayerInteraction - Player state is currently: " + playerState);

        if (playerState == PlayerState.FinishedWashingPlate)
        {
            networkedWashInteraction.FinishWashingPlate();
        }
    }
}
