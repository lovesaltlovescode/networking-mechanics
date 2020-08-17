using System.Collections;
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
    cleanplate,

    //customer
    customer,

    //dishes
    roastedChicWRiceBall,
    roastedChicWPlainRice,
    roastedChicWRiceBallEgg,
    roastedChicWPlainRiceEgg,
    steamedChicWRiceBall,
    steamedChicWPlainRice,
    steamedChicWRiceBallEgg,
    steamedChicWPlainRiceEgg
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
    CanThrowDrink,

    //Table items
    CanPickUpDirtyPlate,
    HoldingDirtyPlate,

    //Wash interaction
    CanPlacePlateInSink,
    ExitedSink,
    CanWashPlate,
    WashingPlate,
    StoppedWashingPlate,
    FinishedWashingPlate,

    //Customer Interaction
    CanPickUpCustomer,
    HoldingCustomer,
    CanTakeOrder,
    CanPickUpDish,
    CanThrowDish,
    HoldingOrder
}

public class NetworkedPlayerInteraction : NetworkBehaviour
{
    #region Variables

    [SerializeField] private string customerTag = "Customer", dishTag = "Dish";
    [SerializeField] private string queueingCustomerLayer = "Queue", takeOrderLayer = "Ordering";

    #region Prefabs

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


    [Header("Customer")]
    public GameObject queueingCustomerPrefab;

    [Header("Dishes")]
    public GameObject roastedChicWRiceBall;
    public GameObject roastedChicWPlainRice;
    public GameObject roastedChicWRiceBallEgg;
    public GameObject roastedChicWPlainRiceEgg;
    public GameObject steamedChicWRiceBall;
    public GameObject steamedChicWPlainRice;
    public GameObject steamedChicWRiceBallEgg;
    public GameObject steamedChicWPlainRiceEgg;

    #endregion

    #region Raycast Variables

    //RAYCAST VARIABLES
    [Header("Raycast Variables")]
    public float raycastLength = 1.5f; //how far the raycast extends

    [SerializeField] private float distFromObject; //distance from the object looking at

    //mask these layers, they do not need to be raycasted
    //Player, environment, zones and uninteractable layers
    private int layerMask = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 13;

    //Detected object, object player is looking at
    public GameObject detectedObject;

    #endregion

    #region Player

    //player attachment point
    public GameObject attachmentPoint;
    //player drop point, where items should be dropped
    public GameObject dropPoint;

    //player's inventory

    public GameObject playerInventory;

    public GameObject customer;

    public PlayerState playerState;

    //when the helditem changes, call onchangeingredient method
    [SyncVar(hook = nameof(OnChangeHeldItem))]
    public HeldItem heldItem;


    #endregion

    #region Scripts

    //different scripts to reference
    [SerializeField] private NetworkedIngredientInteraction networkedIngredientInteraction;
    [SerializeField] private NetworkedWashInteraction networkedWashInteraction;
    [SerializeField] private NetworkedDrinkInteraction networkedDrinkInteraction;
    [SerializeField] private NetworkedCustomerInteraction networkedCustomerInteraction;

    #endregion

    #endregion


    #region Initialization

    private void Awake()
    {
        networkedIngredientInteraction = GetComponent<NetworkedIngredientInteraction>();
        networkedWashInteraction = GetComponent<NetworkedWashInteraction>();
        networkedDrinkInteraction = GetComponent<NetworkedDrinkInteraction>();
        networkedCustomerInteraction = GetComponent<NetworkedCustomerInteraction>();

    }

    #endregion


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
            if (newItem == HeldItem.nothing && playerState != PlayerState.HoldingOrder)
            {
               // Debug.Log("NetworkedIngredientInteraction - Destroying held object");
                Destroy(playerInventory);
            }
            //if player is holding something, do nothing
            //Debug.Log("NetworkedIngredient - Inventory is full!");
            yield return null;
        }

        //depending on which held item is being held by player (in update)
        //instantiate the corresponding prefab

        #region Instantiate prefabs

        switch (newItem)
        {
            #region Ingredients

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

            case HeldItem.rotten:
                var rotten = Instantiate(rottenPrefab, attachmentPoint.transform);
                playerInventory = rotten;
                rotten.tag = "RottenIngredient";
                break;

            #endregion

            #region Drinks and plates

            case HeldItem.dirtyplate:
                var dirtyPlate = Instantiate(dirtyPlatePrefab, attachmentPoint.transform);
                playerInventory = dirtyPlate;
                dirtyPlate.tag = "DirtyPlate";
                break;

            case HeldItem.drink:
                var drink = Instantiate(drinkPrefab, attachmentPoint.transform);
                playerInventory = drink;
                drink.tag = "Drink";
                break;

            #endregion

            case HeldItem.customer:
                customer = Instantiate(queueingCustomerPrefab, attachmentPoint.transform);
                playerInventory = customer;
                break;

            //DISHES
            case HeldItem.roastedChicWRiceBall:
                var dish1 = Instantiate(roastedChicWRiceBall, attachmentPoint.transform);
                playerInventory = dish1;
                break;

            case HeldItem.roastedChicWPlainRice:
                var dish2 = Instantiate(roastedChicWPlainRice, attachmentPoint.transform);
                playerInventory = dish2;
                break;

            case HeldItem.roastedChicWRiceBallEgg:
                var dish3 = Instantiate(roastedChicWRiceBallEgg, attachmentPoint.transform);
                playerInventory = dish3;
                break;

            case HeldItem.roastedChicWPlainRiceEgg:
                var dish4 = Instantiate(roastedChicWPlainRiceEgg, attachmentPoint.transform);
                playerInventory = dish4;
                break;

            case HeldItem.steamedChicWRiceBall:
                var dish5 = Instantiate(steamedChicWRiceBall, attachmentPoint.transform);
                playerInventory = dish5;
                break;

            case HeldItem.steamedChicWPlainRice:
                var dish6 = Instantiate(steamedChicWPlainRice, attachmentPoint.transform);
                playerInventory = dish6;
                break;

            case HeldItem.steamedChicWRiceBallEgg:
                var dish7 = Instantiate(steamedChicWRiceBallEgg, attachmentPoint.transform);
                playerInventory = dish7;
                break;

            case HeldItem.steamedChicWPlainRiceEgg:
                var dish8 = Instantiate(steamedChicWPlainRiceEgg, attachmentPoint.transform);
                playerInventory = dish8;
                break;

        }

        #endregion


    }

    #endregion


    #region Server/Commands

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

    //called from client to server to pick up item
    //Pass in a detectedobject parameter so that the server knows which object to look for
    //It should be looking for client's local detectedobject, not the servers''s
    [Command]
    public void CmdPickUpObject(GameObject detectedObject)
    {
        NetworkServer.Destroy(detectedObject);

    }

    [ServerCallback]
    public GameObject ServerSpawnObject(Vector3 pos, Quaternion rot, HeldItem itemToSpawn, string layerName)
    {
        //instantiate scene object
        GameObject spawnObject = Instantiate(objectContainerPrefab, pos, rot);

        //set rigidbody as non-kinematic
        spawnObject.GetComponent<Rigidbody>().isKinematic = false;

        ObjectContainer objectContainer = spawnObject.GetComponent<ObjectContainer>();

        //Instantiate the right held item as child of the object
        objectContainer.SetObjToSpawn(itemToSpawn);

        //sync var helditem in object container
        objectContainer.objToSpawn = itemToSpawn;

        //spawn on network
        NetworkServer.Spawn(spawnObject);

        RpcSpawnObject(spawnObject, layerName);

        return spawnObject;

    }

    [ClientRpc]
    public void RpcSpawnObject(GameObject spawnObject, string layerName)
    {
        spawnObject.layer = LayerMask.NameToLayer(layerName);
    }

    #endregion

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

        Debug.Log("playerstate is " + playerState);

        DetectObjects();

        //DETECT WHAT OBJECTS PLAYER IS LOOKING AT

        #region Detect Object Looking At

        #region Ingredients

        DetectObjectLookingAt(detectedObject, 14, networkedIngredientInteraction.DetectShelf);

        PickUpObject(detectedObject, 17, IsInventoryFull(), PlayerState.CanPickUpIngredient);

        #endregion

        #region Plates and drinks

        DetectObjectLookingAt(detectedObject, 16, networkedIngredientInteraction.DetectPlate);

        DetectObjectLookingAt(detectedObject, 21, networkedDrinkInteraction.DetectFridge);

        #endregion

        #region Customers

        DetectObjectLookingAt(detectedObject, 19, networkedCustomerInteraction.DetectCustomer);

        DetectObjectLookingAt(detectedObject, 25, networkedCustomerInteraction.DetectDish);

        #endregion

        #endregion

        if (Input.GetKeyDown(KeyCode.D))
        {
            TestInventory(playerInventory);
        }


    }

    public void TestInventory(GameObject _playerInventory)
    {
        Debug.Log("Player inventory " + _playerInventory.name);
        CmdTestInventory(_playerInventory);
    }

    [Command]
    public void CmdTestInventory(GameObject _playerInventory)
    {
        Debug.Log("CMD Player inventory " + _playerInventory.name);
    }

    #region Raycast

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

        #region Check for detected object

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

        #endregion


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

                ////If the detected obj is a customer that is queueing, change the player state to canpickupcustomer
                //if (hit.collider.gameObject.tag == customerTag && hit.collider.gameObject.layer == LayerMask.NameToLayer(queueingCustomerLayer))
                //{
                //    playerState = PlayerState.CanPickUpCustomer;
                //}
                //else if (hit.collider.tag == dishTag)
                //{
                //    //change player state
                //    playerState = PlayerState.CanPickUpDish;

                //}
            }
            else
            {
                if (!CanChangePlayerState())
                {
                    //set hit object as detectedobject
                    detectedObject = hit.collider.gameObject;
                    Debug.Log("detected object: " + detectedObject.name);
                }
            }

            //if player is looking at a table ready to order, set their state to cantakeorder
            //if inventory not full
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(takeOrderLayer) && !IsInventoryFull())
            {
                playerState = PlayerState.CanTakeOrder;

                //set hit object as detectedobject
                detectedObject = hit.collider.gameObject;
                //Debug.Log("detected object: " + detectedObject.tag);
            }

        }
        else
        {
            //no object hit
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
            detectedObject = null;

            if (CanChangePlayerState())
            {
                //if holding dirty plate
                if(playerInventory && playerInventory.tag == "DirtyPlate")
                {
                    return;
                }

                else if (networkedWashInteraction.atSink)
                {
                    return;
                }

                ChangePlayerState(PlayerState.Default);
            }
            //Debug.Log("NetworkedPlayer - No object found");

        }
    }

    #endregion

    #region InteractButton


    //Interact button method
    //Changes based on which state the player is
    public void InteractButton()
    {
        switch (playerState)
        {

            #region Ingredients

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
                networkedIngredientInteraction.TrashHeldItem();
                break;

            //ROTTEN INGREDIENT
            case PlayerState.CanPickUpRottenIngredient:
                //Debug.Log("NetworkedPlayerInteraction - Pick up rotten ingredient");
                networkedIngredientInteraction.PickUpRottenIngredient();
                break;

            #endregion


            #region Drinks and plates

            //DRINKS
            case PlayerState.CanSpawnDrink:
                networkedDrinkInteraction.SpawnDrink();
                break;

            case PlayerState.CanPickUpDrink:
                networkedDrinkInteraction.PickUpDrink();
                break;

            case PlayerState.HoldingDrink:
                networkedDrinkInteraction.ServeDrink();
                break;


            //PLATES
            case PlayerState.CanPickUpDirtyPlate:
                //Debug.Log("NetworkedPlayerInteraction - Pick up the dirty plate!");
                networkedIngredientInteraction.PickUpPlate();
                break;

            case PlayerState.CanPlacePlateInSink:
                //Debug.Log("NetworkedPlayerInteraction - Place plate in sink!");
                networkedWashInteraction.PlacePlateInSink(playerInventory);
                break;

            case PlayerState.CanWashPlate:
                //Debug.Log("NetworkedPlayerInteraction - Wash plate in sink!");
                networkedWashInteraction.WashPlate();
                break;

            #endregion


            #region Customer

            //CUSTOMER
            case PlayerState.CanPickUpCustomer:
                networkedCustomerInteraction.PickUpCustomer();
                break;

            case PlayerState.HoldingCustomer:
                networkedCustomerInteraction.CheckCanPutCustomerDown();
                break;

            case PlayerState.CanTakeOrder:
                networkedCustomerInteraction.CheckHandsEmpty();
                break;

            case PlayerState.CanPickUpDish:
                networkedCustomerInteraction.PickUpDish();
                break;

            case PlayerState.CanThrowDish:
                networkedIngredientInteraction.TrashHeldItem();
                break;

            case PlayerState.HoldingOrder:
                networkedCustomerInteraction.CheckCanPutDownOrder();
                break;

            #endregion
                

            default:
                Debug.Log("default case");

                if(detectedObject.tag == "DirtyPlate" && detectedObject.layer == LayerMask.NameToLayer("TableItem"))
                {
                    Debug.Log("Looking at dirty plate");
                    networkedIngredientInteraction.CmdTooManyPlates(detectedObject);
                }

                    break;
        }
    }

    #endregion



    #region DetectMethods

    public void DetectObjectLookingAt(GameObject detectedObject, int layer, Action callback)
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
                ChangePlayerState(_playerState);
            }
        }
    }

    #endregion

    #region ChangePlayerState

    //checks whether the playerstate is something that should not be changed
    public bool CanChangePlayerState()
    {
        if (playerState == PlayerState.HoldingCustomer || playerState == PlayerState.HoldingOrder 
            || playerState == PlayerState.HoldingDirtyPlate || playerState == PlayerState.HoldingDrink)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    //changes the player state
    public void ChangePlayerState(PlayerState newPlayerState, bool canBypassCheck = false)
    {
        if (CanChangePlayerState() || canBypassCheck)
        {
            playerState = newPlayerState;
        }
        else
        {
            //Debug.Log("cannot change playerstate to " + newPlayerState);
        }
    }

    #endregion


}
