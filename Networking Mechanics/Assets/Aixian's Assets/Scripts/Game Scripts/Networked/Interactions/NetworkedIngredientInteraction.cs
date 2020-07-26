using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;

/// <summary>
/// Handles changing of player states
/// Depending on what the player is looking at, state changes
/// Spawns ingredients
/// </summary>

public class NetworkedIngredientInteraction : NetworkBehaviour
{

   // [Header("Ingredient Tray")]
    //2 arrays, 1 array to store the pos on the tray, if there is nothing (by default) it is a null element
    //second array will store the transform for the traypositions, public array

    //public Transform[] dirtyPlateSpawnPos; //array to contain all possible dirty plate spawn positions

    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    [Header("Booleans")]

    public bool nearIngredientTray; //check if player is in the ingredient tray zone
    public bool nearTrashBin; //check if player is in the trash zone

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    #region Methods to Detect

    //Check if player has detected a shelf
    public void DetectShelf()
    {
        if (!hasAuthority)
        {
            return;
        }

            //if player is not holding anything
            if (!networkedPlayerInteraction.playerInventory)
            {
                //Debug.Log("NetworkedIngredientInteraction - Able to spawn ingredient!");

                switch (networkedPlayerInteraction.detectedObject.tag)
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

    //Check if player has detected a plate
    public void DetectPlate()
    {
        if (!hasAuthority)
        {
            return;
        }

            //if player is not holding anything
            if (!networkedPlayerInteraction.playerInventory)
            {
                //Debug.Log("NetworkedIngredientInteraction - Able to pick up plate!");

                switch (networkedPlayerInteraction.detectedObject.tag)
                {
                    case "DirtyPlate":
                        //change player state
                        //Debug.Log("NetworkedIngredientInteraction - Able to pick up dirty plate!");

                        if (GameManager.Instance.platesInSinkCount >= 4)
                        {
                            Debug.Log("NetworkedWashInteraction - Too many plates in sink!");
                            return;
                        }
                        networkedPlayerInteraction.playerState = PlayerState.CanPickUpDirtyPlate;
                        break;
                }
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

       // Debug.Log("Ingredient count: " + GameManager.Instance.ingredientsOnTrayCount);

        networkedPlayerInteraction.DetectObject(networkedPlayerInteraction.detectedObject, 14, DetectShelf);

        networkedPlayerInteraction.DetectObject(networkedPlayerInteraction.detectedObject, 16, DetectPlate);

        //pickuppable layer
        networkedPlayerInteraction.PickUpObject(networkedPlayerInteraction.detectedObject, 17, networkedPlayerInteraction.IsInventoryFull(), PlayerState.CanPickUpIngredient);

        //if player is holding something
        if (networkedPlayerInteraction.IsInventoryFull())
        {
            //if player is holding a dirty plate 
            if (networkedPlayerInteraction.playerInventory.tag == "DirtyPlate")
            {
                //Debug.Log("NetworkedIngredientInteraction - Unable to drop plate");
                return;
            }

            //if player is holding a drink
            if(networkedPlayerInteraction.playerInventory.tag == "Drink")
            {
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
            if(GameManager.Instance.ingredientsOnTrayCount >= 4 && nearIngredientTray)
            {
                networkedPlayerInteraction.playerState = PlayerState.Default;
                Debug.Log("Ingredient tray full");
                return;
            }
            networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;
        }

        //Temp spawn plate
        if (Input.GetKeyDown(KeyCode.S))
        {
            //Debug.Log("NetworkedIngredientInteraction - Spawning dirty plate!");
            SpawnPlate();
        }

        //if player sees a rotten ingredient
        networkedPlayerInteraction.PickUpObject(networkedPlayerInteraction.detectedObject, 23, networkedPlayerInteraction.IsInventoryFull(), PlayerState.CanPickUpRottenIngredient);

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

        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.dirtyplate);
        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);
        Debug.Log("Detected object is plate " + networkedPlayerInteraction.detectedObject);

        //heldItem = HeldItem.dirtyplate;
        networkedPlayerInteraction.playerState = PlayerState.HoldingDirtyPlate;

    }

    //Method to be called from player interaction script
    //Since playerinteraction shouldn't be networked, unable to call the CMD directly
    //Instead, call this method and change the ingredient according to the state
    public void SpawnIngredient(HeldItem selectedIngredient)
    {
        networkedPlayerInteraction.CmdChangeHeldItem(selectedIngredient);
        
        networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;

        //error due to attachment point not being updated but was called
        //Could call in an RPC instead, since CMD always gets called first
        //Debug.Log("NetworkedIngredientInteraction - Ingredient tag: " + networkedPlayerInteraction.attachmentPoint.transform.GetChild(0).gameObject.tag);
        //Debug.Log("NetworkedIngredientInteraction - Ingredient tag: " + networkedPlayerInteraction.attachmentPoint.transform.GetChild(0).gameObject);
        //Debug.Log("NetworkedIngredientInteraction - Ingredient tag: " + networkedPlayerInteraction.playerInventory);
    }

    public void DropIngredient() 
    {
        // remove all items from inventory
        //Debug.Log("NetworkedIngredientInteraction - Inventory: " + networkedPlayerInteraction.playerInventory);
        //Debug.Log("//Debugging dropping - Part 1");

        CmdDropIngredient(networkedPlayerInteraction.playerInventory);
        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing);
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
        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);
        //Debug.Log("//Debugging ingredient - Part 1");

        networkedPlayerInteraction.CmdChangeHeldItem(networkedPlayerInteraction.detectedObject.GetComponent<ObjectContainer>().objToSpawn);
        
        //Debug.Log("//Debugging ingredient - Part 2");
        //Debug.Log("NetworkedIngredientInteraction - Ingredient tag: " + networkedPlayerInteraction.playerInventory.tag);
        //networkedPlayerInteraction.playerInventory.layer = LayerMask.NameToLayer("Ingredient");

        networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;
        //Debug.Log("//Debugging ingredient - Part should not be shown");
        

    }

    //throw ingredient
    public void TrashIngredient()
    {
        CmdTrashIngredient(networkedPlayerInteraction.playerInventory);

        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing);
        //Debug.Log("NetworkedIngredientInteraction - Player has thrown an ingredient");

    }

    public void PickUpRottenIngredient()
    {
        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);
        //heldItem = HeldItem.rotten;
        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.rotten);

        ////Debug.Log("NetworkedIngredientInteraction - Rotten Ingredient tag: " + networkedPlayerInteraction.playerInventory.tag);
        //Debug.Log("NetworkedingredientInteraction - Picked up a rotten ingredient!");
        if (networkedPlayerInteraction.IsInventoryFull())
        {
            networkedPlayerInteraction.playerState = PlayerState.HoldingRottenIngredient;
        }
    }

    #endregion

    #region Commands

    //sends a command from client to server to drop the held item in the scene
    [Command]
    void CmdDropIngredient(GameObject playerInventory)
    {

        //if near ingredient tray
        if (nearIngredientTray)
        {
            for (int i = 0; i < GameManager.Instance.ingredientsOnTray.Length; i++)
            {
                if (GameManager.Instance.ingredientsOnTray[i] == null)
                {

                    Vector3 trayPos = GameManager.Instance.trayPositions[i].transform.position;
                    //Debug.Log("Ingredienttray - tray pos " + trayPos);

                    Quaternion trayRot = GameManager.Instance.trayPositions[i].transform.rotation;
                    //Debug.Log("Ingredienttray - tray rot" + trayRot);

                    //Generic drop functions
                    GameObject trayIngredient = Instantiate(networkedPlayerInteraction.objectContainerPrefab, trayPos, trayRot);

                    trayIngredient.GetComponent<Rigidbody>().isKinematic = false;

                    //get sceneobject script from the sceneobject prefab
                    ObjectContainer ingredientContainer = trayIngredient.GetComponent<ObjectContainer>();

                    //instantiate the right ingredient as a child of the object
                    ingredientContainer.SetObjToSpawn(networkedPlayerInteraction.heldItem);

                    //sync var the helditem in scene object to the helditem in the player
                    ingredientContainer.objToSpawn = networkedPlayerInteraction.heldItem;

                    //spawn the scene object on network for everyone to see
                    NetworkServer.Spawn(trayIngredient);

                    //set layer to uninteractable
                    trayIngredient.layer = LayerMask.NameToLayer("UnInteractable");
                    //Debug.Log("//Debugging dropping - Part 4");

                    //Set the ingredient on tray to be the spawned object
                    GameManager.Instance.ingredientsOnTray[i] = trayIngredient;

                    RpcDropIngredient(trayIngredient, i);

                    //clear the inventory after dropping on tray
                    playerInventory = null;

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
            objectContainer.SetObjToSpawn(networkedPlayerInteraction.heldItem);

            //sync var the helditem in scene object to the helditem in the player
            objectContainer.objToSpawn = networkedPlayerInteraction.heldItem;

            //spawn the scene object on network for everyone to see
            NetworkServer.Spawn(droppedIngredient);
            //Debug.Log("//Debugging dropping - Part 7");
            droppedIngredient.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ingredient");

            //clear inventory after dropping
            playerInventory = null;
            //Debug.Log("//Debugging dropping - Part 8");

        }

    }

    [ClientRpc]
    void RpcDropIngredient(GameObject trayIngredient, int i)
    {

        GameManager.Instance.ingredientsOnTrayCount += 1;
        Debug.Log("GameManager - Ingredient on tray: " + GameManager.Instance.ingredientsOnTrayCount);

        //Set the ingredient on tray to be the spawned object
        GameManager.Instance.ingredientsOnTray[i] = trayIngredient;
    }

    [Command]
    void CmdTrashIngredient(GameObject playerInventory)
    {
        var thrownIngredient = networkedPlayerInteraction.playerInventory;
        //Debug.Log("NetworkedIngredientInteraction - Thrown ingredient is " + thrownIngredient);

        //destroy the thrown ingredient
        Destroy(thrownIngredient);

        networkedPlayerInteraction.heldItem = HeldItem.nothing;

        //clear the inventory after throwing the ingredient
        playerInventory = null;

        return;
    }

    //server will spawn a dirty plate
    [ServerCallback]
    void SpawnDirtyPlate()
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
        objectContainer.SetObjToSpawn(HeldItem.dirtyplate);

        //sync var the helditem in object container to the helditem in the player
        objectContainer.objToSpawn = HeldItem.dirtyplate;
        Debug.Log("Object spawned is " + objectContainer.objToSpawn);

        //change layer of the container
        dirtyPlate.layer = LayerMask.NameToLayer("TableItem");
        //Debug.Log("Spawn plates - Spawned plate");

        ////spawn the scene object on network for everyone to see
        NetworkServer.Spawn(dirtyPlate);

        RpcSpawnDirtyPlate(dirtyPlate);
    }

    [ClientRpc]
    public void RpcSpawnDirtyPlate(GameObject dirtyPlate)
    {
        dirtyPlate.layer = LayerMask.NameToLayer("TableItem");
    }
    #endregion


    #region Triggers

    //TRIGGER ZONES
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "IngredientTrayZone")
        {
            //Debug.Log("NetworkedIngredientInteraction - Near the ingredient tray!");
            nearIngredientTray = true;
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
        if (other.tag == "IngredientTrayZone")
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
