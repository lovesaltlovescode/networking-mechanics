using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Handles changing of player states
/// Depending on what the player is looking at, state changes
/// Spawns ingredients
/// </summary>

public class NetworkedIngredientInteraction : NetworkBehaviour
{

    [Header("Ingredient Tray")]
    //2 arrays, 1 array to store the pos on the tray, if there is nothing (by default) it is a null element
    //second array will store the transform for the traypositions, public array
    public static GameObject trayParentZone; //Tray object that contains all ingredient tray positions

    private static Transform[] trayPositions; //array to contain all tray positions

    public static GameObject[] ingredientsOnTray = new GameObject[4]; //array to contain all ingredients on the tray

    //public Transform[] dirtyPlateSpawnPos; //array to contain all possible dirty plate spawn positions

    //when the helditem changes, call onchangeingredient method
    [SyncVar(hook = nameof(OnChangeIngredient))]
    public HeldItem heldItem;

    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    [Header("Booleans")]

    public bool nearIngredientTray; //check if player is in the ingredient tray zone
    public bool nearTrashBin; //check if player is in the trash zone

    //if detectedobj is an ingredient shelf, this is true
    public bool detectedShelf = false;

    public bool detectedPlate = false; //if detectedobj is a plate, this is true

    [SerializeField] private string testString = null;

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    #region Test Methods

    public void InitialiseString(string secretString)
    {
        if (!isServer)
        {
            //Debug.Log("Secret - Secret string is " + secretString);
            //Debug.Log("Secret - I am not a server");
            CmdSendString(secretString);
            return;
        }
            //Debug.Log("Secret - I am a server, call CMD instead");
            CmdSendString(secretString);
       
    }

    [Command]
    public void CmdSendString(string secretString)
    {
        //Debug.Log("Secret - Calling RPC");
        //Debug.Log("Cmd Secret - Secret string is " + secretString);
        RpcUpdateString(secretString);
        
    }

    [ClientRpc]
    public void RpcUpdateString(string secretString)
    {
        //Debug.Log("RPC Secret - Secret string is " + secretString);
        testString = secretString;
        //Debug.Log("RPC Secret - Test string is " + secretString);
    }

    #endregion


    #region SyncVar

    void OnChangeIngredient(HeldItem oldIngredient, HeldItem newIngredient)
    {
        //Debug.Log("NetworkedIngredientInteraction - Starting coroutine!");
        StartCoroutine(ChangeIngredient(newIngredient));
    }

    IEnumerator ChangeIngredient(HeldItem newIngredient)
    {
        //If the player is holding something
        while (networkedPlayerInteraction.playerInventory)
        {
            //if player is holding nothing, destroy the existing child
            if (newIngredient == HeldItem.nothing)
            {
                Debug.Log("NetworkedIngredientInteraction - Destroying held object");
                Destroy(networkedPlayerInteraction.playerInventory);
            }
            //if player is holding something, do nothing
            //Debug.Log("NetworkedIngredient - Inventory is full!");
            yield return null;
        }

        //depending on which held item is being held by player (in update)
        //instantiate the corresponding prefab
        switch (newIngredient)
        {
            case HeldItem.chicken:
                var chicken = Instantiate(networkedPlayerInteraction.chickenPrefab, networkedPlayerInteraction.attachmentPoint.transform);
                networkedPlayerInteraction.playerInventory = chicken;
                ////Debug.Log("player inventory is " + chicken + networkedPlayerInteraction.playerInventory);
                chicken.tag = "Chicken";
                break;

            case HeldItem.egg:
                var egg = Instantiate(networkedPlayerInteraction.eggPrefab, networkedPlayerInteraction.attachmentPoint.transform);
                networkedPlayerInteraction.playerInventory = egg;
                egg.tag = "Egg";
                break;

            case HeldItem.cucumber:
                var cucumber = Instantiate(networkedPlayerInteraction.cucumberPrefab, networkedPlayerInteraction.attachmentPoint.transform);
                networkedPlayerInteraction.playerInventory = cucumber;
                cucumber.tag = "Cucumber";
                break;

            case HeldItem.rice:
                var rice = Instantiate(networkedPlayerInteraction.ricePrefab, networkedPlayerInteraction.attachmentPoint.transform);
                networkedPlayerInteraction.playerInventory = rice;
                rice.tag = "Rice";
                break;

            case HeldItem.dirtyplate:
                var dirtyPlate = Instantiate(networkedPlayerInteraction.dirtyPlatePrefab, networkedPlayerInteraction.attachmentPoint.transform);
                networkedPlayerInteraction.playerInventory = dirtyPlate;
                dirtyPlate.tag = "DirtyPlate";
                break;

            case HeldItem.rotten:
                var rotten = Instantiate(networkedPlayerInteraction.rottenPrefab, networkedPlayerInteraction.attachmentPoint.transform);
                networkedPlayerInteraction.playerInventory = rotten;
                rotten.tag = "RottenIngredient";
                break;

        }
    }

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (networkedPlayerInteraction.attachmentPoint.transform.childCount > 0)
        {
            //Debug.Log("Update NetworkedIngredientInteraction - Ingredient tag: " + networkedPlayerInteraction.attachmentPoint.transform.GetChild(0).gameObject);
        }
        
        //Check for detected object and if it is a shelf
        if (networkedPlayerInteraction.detectedObject && networkedPlayerInteraction.detectedObject.layer == 14)
        {
            detectedShelf = true;
            //Debug.Log("NetworkedIngredientInteraction - Detected a shelf!");

            DetectShelf(networkedPlayerInteraction.detectedObject);
        }
        else
        {
            detectedShelf = false;
        }

        //check for detected object and if it is a plate
        if (networkedPlayerInteraction.detectedObject && networkedPlayerInteraction.detectedObject.layer == 16)
        {
            detectedPlate = true;
            //Debug.Log("NetworkedIngredientInteraction - Detected a plate!");

            DetectPlate(networkedPlayerInteraction.detectedObject);
        }
        else
        {
            detectedPlate = false;
        }

        //pickuppable layer
        if (networkedPlayerInteraction.detectedObject && networkedPlayerInteraction.detectedObject.layer == 17)
        {
            //Debug.Log("ObjectContainer - Pickuppable ingredient detected!");

            if (!networkedPlayerInteraction.IsInventoryFull())
            {
                //if not holding anything, change state
                networkedPlayerInteraction.playerState = PlayerState.CanPickUpIngredient;
            }
        }

        //if player is holding something
        if (networkedPlayerInteraction.IsInventoryFull())
        {
            //if player is holding a dirty plate
            if (networkedPlayerInteraction.playerInventory.tag == "DirtyPlate")
            {
                //Debug.Log("NetworkedIngredientInteraction - Unable to drop plate");
                return;
            }

            //if player is holding a rotten ingredient
            if (networkedPlayerInteraction.playerInventory.tag == "RottenIngredient")
            {
                //Debug.Log("NetworkedIngredientInteraction - Unable to drop rotten ingredient");

                if (nearTrashBin)
                {
                    networkedPlayerInteraction.playerState = PlayerState.CanThrowIngredient;
                }
                else
                {
                    networkedPlayerInteraction.playerState = PlayerState.HoldingRottenIngredient;
                }

                return;

            }

            if (nearTrashBin)
            {
                networkedPlayerInteraction.playerState = PlayerState.CanThrowIngredient;
                //Debug.Log("NetworkedIngredientInteraction - Can throw ingredient!");
                return;
            }

            //Debug.Log("NetworkedIngredientInteraction - Able to drop ingredient!");
            networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;
        }

        //Temp spawn plate
        if (Input.GetKeyDown(KeyCode.S))
        {
            //Debug.Log("NetworkedIngredientInteraction - Spawning dirty plate!");
            SpawnPlate();
        }

        //if player sees a rotten ingredient
        if (networkedPlayerInteraction.detectedObject && networkedPlayerInteraction.detectedObject.tag == "RottenIngredient")
        {
            //Debug.Log("NetworkedIngredientInteraction - Rotten ingredient spotted!");

            if (!networkedPlayerInteraction.IsInventoryFull())
            {
                //if player not holding anything
                networkedPlayerInteraction.playerState = PlayerState.CanPickUpRottenIngredient;
            }
        }



    }

    //Check if player has detected a shelf
    public void DetectShelf(GameObject detectedObject)
    {
        if (!hasAuthority)
        {
            return;
        }

        //ingredient shelf layer
        if (detectedShelf)
        {

            //if player is not holding anything
            if (!networkedPlayerInteraction.playerInventory)
            {
                //Debug.Log("NetworkedIngredientInteraction - Able to spawn ingredient!");

                switch (detectedObject.tag)
                {
                    case "ChickenShelf":
                        //change player state
                        //Debug.Log("NetworkedIngredientInteraction - Able to spawn chicken!");
                        networkedPlayerInteraction.playerState = PlayerState.CanSpawnChicken;
                        break;

                    case "EggShelf":
                        //change player state
                        //Debug.Log("NetworkedIngredientInteraction - Able to spawn egg!");
                        networkedPlayerInteraction.playerState = PlayerState.CanSpawnEgg;
                        break;

                    case "CucumberShelf":
                        //change player state
                        //Debug.Log("NetworkedIngredientInteraction - Able to spawn cucumber!");
                        networkedPlayerInteraction.playerState = PlayerState.CanSpawnCucumber;
                        break;

                    case "RiceTub":
                        //change player state
                        //Debug.Log("NetworkedIngredientInteraction - Able to spawn rice!");
                        networkedPlayerInteraction.playerState = PlayerState.CanSpawnRice;
                        break;
                }
            }


        }
    }

    //Check if player has detected a plate
    public void DetectPlate(GameObject detectedObject)
    {
        if (!hasAuthority)
        {
            return;
        }

        //ingredient shelf layer
        if (detectedPlate)
        {

            //if player is not holding anything
            if (!networkedPlayerInteraction.playerInventory)
            {
                //Debug.Log("NetworkedIngredientInteraction - Able to pick up plate!");

                switch (detectedObject.tag)
                {
                    case "DirtyPlate":
                        //change player state
                        //Debug.Log("NetworkedIngredientInteraction - Able to pick up dirty plate!");

                        if (NetworkedWashInteraction.platesInSinkCount >= 4)
                        {
                            //Debug.Log("NetworkedWashInteraction - Too many plates in sink!");
                            return;
                        }
                        networkedPlayerInteraction.playerState = PlayerState.CanPickUpDirtyPlate;
                        break;
                }
            }



        }
    }

    #endregion


    #region RemoteMethods
    //Methods that are called in the playerinteraction script

    public void SpawnPlate()
    {
        //spawn a plate on the server
        //for now, on key press
        SpawnDirtyPlate();
    }

    public void PickUpPlate()
    {
        CmdPickUpIngredient(networkedPlayerInteraction.detectedObject);

        heldItem = HeldItem.dirtyplate;
        networkedPlayerInteraction.playerState = PlayerState.HoldingDirtyPlate;

    }

    //Method to be called from player interaction script
    //Since playerinteraction shouldn't be networked, unable to call the CMD directly
    //Instead, call this method and change the ingredient according to the state
    public void SpawnIngredient(HeldItem selectedIngredient)
    {
        CmdChangeHeldItem(selectedIngredient);
        
        networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;
        ////Debug.Log("Spawning Part 3");
        //Debug.Log("NetworkedIngredientInteraction - Ingredient tag: " + networkedPlayerInteraction.attachmentPoint.transform.GetChild(0).gameObject.tag);
        //Debug.Log("NetworkedIngredientInteraction - Ingredient tag: " + networkedPlayerInteraction.attachmentPoint.transform.GetChild(0).gameObject);
        //Debug.Log("NetworkedIngredientInteraction - Ingredient tag: " + networkedPlayerInteraction.playerInventory);
    }

    public void DropIngredient() 
    {
        // remove all items from inventory
        //Debug.Log("NetworkedIngredientInteraction - Inventory: " + networkedPlayerInteraction.playerInventory);
        //Debug.Log("//Debugging dropping - Part 1");

        CmdDropIngredient();
        //Debug.Log("//Debugging dropping - Part 2");

        //change player state to can pick up if inventory is not full
        if (!networkedPlayerInteraction.IsInventoryFull())
        {
            networkedPlayerInteraction.playerState = PlayerState.CanPickUpIngredient;
            //Debug.Log("NetworkedIngredientInteraction - Inventory empty, can pick up ingredient");
            //Debug.Log("//Debugging dropping - Part 3");
        }

    }

    public void PickUpIngredient()
    {
        CmdPickUpIngredient(networkedPlayerInteraction.detectedObject);
        //Debug.Log("//Debugging ingredient - Part 1");

        
        //Debug.Log("//Debugging ingredient - Part 2");
        //Debug.Log("NetworkedIngredientInteraction - Ingredient tag: " + networkedPlayerInteraction.playerInventory.tag);
        //networkedPlayerInteraction.playerInventory.layer = LayerMask.NameToLayer("Ingredient");

        if (networkedPlayerInteraction.IsInventoryFull())
        {
            networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;
            //Debug.Log("//Debugging ingredient - Part should not be shown");
        }

    }

    //throw ingredient
    public void ThrowIngredient()
    {
        CmdThrowIngredient();
        //Debug.Log("NetworkedIngredientInteraction - Player has thrown an ingredient");
       
    }

    public void PickUpRottenIngredient()
    {
        CmdPickUpIngredient(networkedPlayerInteraction.detectedObject);
        heldItem = HeldItem.rotten;
        ////Debug.Log("NetworkedIngredientInteraction - Rotten Ingredient tag: " + networkedPlayerInteraction.playerInventory.tag);
        //Debug.Log("NetworkedingredientInteraction - Picked up a rotten ingredient!");
        if (networkedPlayerInteraction.IsInventoryFull())
        {
            networkedPlayerInteraction.playerState = PlayerState.HoldingRottenIngredient;
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
        heldItem = selectedIngredient;
    }

    //sends a command from client to server to drop the held item in the scene
    [Command]
    void CmdDropIngredient()
    {

        //if near ingredient tray
        if (nearIngredientTray)
        {
            for (int i = 0; i < ingredientsOnTray.Length; i++)
            {
                if (ingredientsOnTray[i] == null)
                {
                    //if null, assign it as held ingredient
                    var detectedIngredient = networkedPlayerInteraction.detectedObject;
                    //Debug.Log("Ingredienttray - detected ingredient " + detectedIngredient);

                    Vector3 trayPos = trayPositions[i].transform.position;
                    //Debug.Log("Ingredienttray - tray pos " + trayPos);

                    Quaternion trayRot = trayPositions[i].transform.rotation;
                    //Debug.Log("Ingredienttray - tray rot" + trayRot);

                    //Generic drop functions
                    GameObject trayIngredient = Instantiate(networkedPlayerInteraction.objectContainerPrefab, trayPos, trayRot);

                    trayIngredient.GetComponent<Rigidbody>().isKinematic = false;

                    //get sceneobject script from the sceneobject prefab
                    ObjectContainer ingredientContainer = trayIngredient.GetComponent<ObjectContainer>();

                    //instantiate the right ingredient as a child of the object
                    ingredientContainer.SetHeldItem(heldItem);

                    //sync var the helditem in scene object to the helditem in the player
                    ingredientContainer.heldItem = heldItem;

                    //set player's sync var to nothing so clients won't see the ingredient anymore
                    heldItem = HeldItem.nothing;

                    //spawn the scene object on network for everyone to see
                    NetworkServer.Spawn(trayIngredient);

                    //set layer to uninteractable
                    trayIngredient.layer = LayerMask.NameToLayer("UnInteractable");
                    //Debug.Log("//Debugging dropping - Part 4");

                    //Set the ingredient on tray to be the spawned object
                    ingredientsOnTray[i] = trayIngredient;

                    //clear the inventory after dropping on tray
                    networkedPlayerInteraction.playerInventory = null;

                    return;
                }
            }
        }
        else
        {
            //instantiate scene object on the server at the drop point
            Vector3 pos = networkedPlayerInteraction.dropPoint.transform.position;
            Quaternion rot = networkedPlayerInteraction.dropPoint.transform.rotation;
            GameObject droppedIngredient = Instantiate(networkedPlayerInteraction.objectContainerPrefab, pos, rot);
            //Debug.Log("//Debugging dropping - Part 5");

            //set Rigidbody as non-kinematic on SERVER only (isKinematic = true in prefab)
            droppedIngredient.GetComponent<Rigidbody>().isKinematic = false;

            //get sceneobject script from the sceneobject prefab
            ObjectContainer objectContainer = droppedIngredient.GetComponent<ObjectContainer>();
            //Debug.Log("//Debugging dropping - Part 6");

            //instantiate the right ingredient as a child of the object
            objectContainer.SetHeldItem(heldItem);

            //sync var the helditem in scene object to the helditem in the player
            objectContainer.heldItem = heldItem;

            //set player's sync var to nothing so clients won't see the ingredient anymore
            heldItem = HeldItem.nothing;

            //spawn the scene object on network for everyone to see
            NetworkServer.Spawn(droppedIngredient);
            //Debug.Log("//Debugging dropping - Part 7");
            droppedIngredient.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ingredient");

            //clear inventory after dropping
            networkedPlayerInteraction.playerInventory = null;
            //Debug.Log("//Debugging dropping - Part 8");

        }

    }

    [Command]
    void CmdThrowIngredient()
    {
        var thrownIngredient = networkedPlayerInteraction.playerInventory;
        //Debug.Log("NetworkedIngredientInteraction - Thrown ingredient is " + thrownIngredient);

        //destroy the thrown ingredient
        Destroy(thrownIngredient);

        heldItem = HeldItem.nothing;

        //clear the inventory after throwing the ingredient
        networkedPlayerInteraction.playerInventory = null;

        return;
    }

    //server will spawn a dirty plate
    [ServerCallback]
    void SpawnDirtyPlate()
    {
        
        RpcSpawnDirtyPlate();
    }

    [ClientRpc]
    public void RpcSpawnDirtyPlate()
    {
        //Instantiate scene object on the server at a fixed position
        //Temporarily at drop pos
        Vector3 pos = networkedPlayerInteraction.dropPoint.transform.position;
        Quaternion rot = networkedPlayerInteraction.dropPoint.transform.rotation;
        GameObject dirtyPlate = Instantiate(networkedPlayerInteraction.objectContainerPrefab, pos, rot);

        ////set Rigidbody as non-kinematic on the instantiated object only (isKinematic = true in prefab)
        dirtyPlate.GetComponent<Rigidbody>().isKinematic = false;

        //get sceneobject script from the sceneobject prefab
        ObjectContainer objectContainer = dirtyPlate.GetComponent<ObjectContainer>();

        //instantiate the right item as a child of the object
        objectContainer.SetHeldItem(HeldItem.dirtyplate);

        //sync var the helditem in scene object to the helditem in the player
        objectContainer.heldItem = heldItem;

        //change layer of the container
        dirtyPlate.layer = LayerMask.NameToLayer("TableItem");
        //Debug.Log("Spawn plates - Spawned plate");

        ////spawn the scene object on network for everyone to see
        //NetworkServer.Spawn(dirtyPlate);

    }

    //called from client to server to pick up item
    [Command]
    public void CmdPickUpIngredient(GameObject detectedObject)
    {
        //set player's syncvar so clients can show the right ingredient
        //according to which item the sceneobject currently contains
        //Debug.Log("//Debugging ingredient - Part 3");

        heldItem = networkedPlayerInteraction.detectedObject.GetComponent<ObjectContainer>().heldItem;
        ////Debug.Log("NetworkedIngredientInteraction - " + networkedPlayerInteraction.detectedObject.tag + " was picked up!");
        //Debug.Log("//Debugging ingredient - Part 4");

        //destroy the scene object when it has been picked up
        NetworkServer.Destroy(networkedPlayerInteraction.detectedObject);
        //Debug.Log("//Debugging ingredient - Part 5");

    }



    #endregion


    #region Triggers

    //TRIGGER ZONES
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "IngredientTableZone")
        {
            //Debug.Log("NetworkedIngredientInteraction - Near the ingredient tray!");
            nearIngredientTray = true;
            trayParentZone = other.gameObject; //hit zone is the tray parent zone

            trayPositions = trayParentZone.GetComponent<IngredientTrayZones>().trayPositions;
        }

        //if trash bin
        if(other.tag == "TrashZone")
        {
            //Debug.Log("NetworkedIngredientInteraction - Near the trash bin!");
            nearTrashBin = true;
            //if there is an ingredient being held
            if(networkedPlayerInteraction.playerInventory &&
                networkedPlayerInteraction.playerInventory.layer == 15)
            
            networkedPlayerInteraction.playerState = PlayerState.CanThrowIngredient;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "IngredientTableZone")
        {
            //Debug.Log("NetworkedIngredientInteraction - Exited ingredient tray");
            nearIngredientTray = false;
        }

        //if trash bin
        if (other.tag == "TrashZone")
        {
            //Debug.Log("NetworkedIngredientInteraction - Exited trash bin!");
            nearTrashBin = false;
        }
    }


    #endregion



}
