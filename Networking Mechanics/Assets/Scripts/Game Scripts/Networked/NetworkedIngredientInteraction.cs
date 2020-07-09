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
    public HeldIngredient heldIngredient;

    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    [Header("Booleans")]

    public bool nearIngredientTray; //check if player is in the ingredient tray zone

    //if detectedobj is an ingredient shelf, this is true
    public bool detectedShelf = false;

    public bool detectedPlate = false; //if detectedobj is a plate, this is true

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }


    #region SyncVar

    void OnChangeIngredient(HeldIngredient oldIngredient, HeldIngredient newIngredient)
    {
        //Debug.Log("NetworkedIngredientInteraction - Starting coroutine!");
        StartCoroutine(ChangeIngredient(newIngredient));
    }

    IEnumerator ChangeIngredient(HeldIngredient newIngredient)
    {
        //If the player is holding something
        while (networkedPlayerInteraction.attachmentPoint.transform.childCount > 0)
        {
            //if player is holding nothing, destroy the existing child
            if (newIngredient == HeldIngredient.nothing)
            {
                Debug.Log("NetworkedIngredientInteraction - Destroying held object");
                Destroy(networkedPlayerInteraction.attachmentPoint.transform.GetChild(0).gameObject);
            }
            //if player is holding something, do nothing
            //Debug.Log("NetworkedIngredient - Inventory is full!");
            yield return null;
        }

        //depending on which held item is being held by player (in update)
        //instantiate the corresponding prefab
        switch (newIngredient)
        {
            case HeldIngredient.chicken:
                var chicken = Instantiate(networkedPlayerInteraction.chickenPrefab, networkedPlayerInteraction.attachmentPoint.transform);
                networkedPlayerInteraction.objectsInInventory.Add(chicken);
                break;

            case HeldIngredient.egg:
                var egg = Instantiate(networkedPlayerInteraction.eggPrefab, networkedPlayerInteraction.attachmentPoint.transform);
                networkedPlayerInteraction.objectsInInventory.Add(egg);
                break;

            case HeldIngredient.cucumber:
                var cucumber = Instantiate(networkedPlayerInteraction.cucumberPrefab, networkedPlayerInteraction.attachmentPoint.transform);
                networkedPlayerInteraction.objectsInInventory.Add(cucumber);
                break;

            case HeldIngredient.dirtyplate:
                var dirtyPlate = Instantiate(networkedPlayerInteraction.dirtyPlatePrefab, networkedPlayerInteraction.attachmentPoint.transform);
                break;

        }
    }

    #endregion


    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        //Check for detected object and if it is a shelf
        if (networkedPlayerInteraction.detectedObject && networkedPlayerInteraction.detectedObject.layer == 14)
        {
            detectedShelf = true;
            Debug.Log("NetworkedIngredientInteraction - Detected a shelf!");

            DetectShelf(networkedPlayerInteraction.detectedObject);
        }
        else
        {
            detectedShelf = false;
        }

        //check for detected object and if it is a plate
        if(networkedPlayerInteraction.detectedObject && networkedPlayerInteraction.detectedObject.layer == 16)
        {
            detectedPlate = true;
            Debug.Log("NetworkedIngredientInteraction - Detected a plate!");

            DetectPlate(networkedPlayerInteraction.detectedObject);
        }
        else
        {
            detectedPlate = false;
        }

        //pickuppable layer
        if (networkedPlayerInteraction.detectedObject && networkedPlayerInteraction.detectedObject.layer == 17)
        {
            Debug.Log("ObjectContainer - Pickuppable ingredient detected!");

            if (!networkedPlayerInteraction.IsInventoryFull())
            {
                //if not holding anything, change state
                networkedPlayerInteraction.playerState = PlayerState.CanPickUpIngredient;
            }
        }

        //if player is holding something
        if (networkedPlayerInteraction.objectsInInventory.Count > 0)
        {
            //if player is holding a dirty plate
            if(networkedPlayerInteraction.attachmentPoint.transform.GetChild(0).tag == "DirtyPlate")
            {
                return;
            }

            Debug.Log("NetworkedIngredientInteraction - Able to drop ingredient!");
            networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("NetworkedIngredientInteraction - Spawning dirty plate!");
            SpawnPlate();
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
        if(detectedShelf)
        {

            //if player is not holding anything
            if(networkedPlayerInteraction.attachmentPoint.transform.childCount == 0)
            {
                Debug.Log("NetworkedIngredientInteraction - Able to spawn ingredient!");

                switch (detectedObject.tag)
                {
                    case "ChickenShelf":
                        //change player state
                        Debug.Log("NetworkedIngredientInteraction - Able to spawn chicken!");
                        networkedPlayerInteraction.playerState = PlayerState.CanSpawnChicken;
                        break;

                    case "EggShelf":
                        //change player state
                        Debug.Log("NetworkedIngredientInteraction - Able to spawn egg!");
                        networkedPlayerInteraction.playerState = PlayerState.CanSpawnEgg;
                        break;

                    case "CucumberShelf":
                        //change player state
                        Debug.Log("NetworkedIngredientInteraction - Able to spawn cucumber!");
                        networkedPlayerInteraction.playerState = PlayerState.CanSpawnCucumber;
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
            if (networkedPlayerInteraction.attachmentPoint.transform.childCount == 0)
            {
                Debug.Log("NetworkedIngredientInteraction - Able to pick up plate!");

                switch (detectedObject.tag)
                {
                    case "DirtyPlate":
                        //change player state
                        Debug.Log("NetworkedIngredientInteraction - Able to pick up dirty plate!");
                        networkedPlayerInteraction.playerState = PlayerState.CanPickUpDirtyPlate;
                        break;
                }
            }



        }
    }


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
        CmdPickUpIngredient();

        heldIngredient = HeldIngredient.dirtyplate;
        networkedPlayerInteraction.playerState = PlayerState.HoldingDirtyPlate;

        //if (networkedPlayerInteraction.IsInventoryFull())
        //{
        //    //change to can place plate in sink
        //    networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;
        //}


    }

    //Method to be called from player interaction script
    //Since playerinteraction shouldn't be networked, unable to call the CMD directly
    //Instead, call this method and change the ingredient according to the state
    public void UpdateIngredient(HeldIngredient selectedIngredient)
    {

        CmdChangeHeldIngredient(selectedIngredient);
        networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;

    }

    public void DropIngredient()
    {
        // remove all items from inventory
        Debug.Log("NetworkedIngredientInteraction - Inventory: " + networkedPlayerInteraction.objectsInInventory);

        CmdDropIngredient();

        //change player state to can pick up if inventory is not full
        if (!networkedPlayerInteraction.IsInventoryFull())
        {
            networkedPlayerInteraction.playerState = PlayerState.CanPickUpIngredient;
            Debug.Log("NetworkedIngredientInteraction - Inventory empty, can pick up ingredient");
        }

    }

    public void PickUpIngredient()
    {
        CmdPickUpIngredient();

        heldIngredient = networkedPlayerInteraction.detectedObject.GetComponent<ObjectContainer>().heldIngredient;

        if (networkedPlayerInteraction.IsInventoryFull())
        {
            networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;
        }

    }

    #endregion

    #region Commands

    //called on server, main function to change the held ingredient
    //change held ingredient to the new ingredient
    //triggers sync var -> Coroutine to instantiate the corresponding prefab
    [Command]
    public void CmdChangeHeldIngredient(HeldIngredient selectedIngredient)
    {
        Debug.Log("NetworkedIngredientInteraction - Held ingredient has been changed!");
        heldIngredient = selectedIngredient;
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
                    Debug.Log("Ingredienttray - detected ingredient " + detectedIngredient);

                    Vector3 trayPos = trayPositions[i].transform.position;
                    Debug.Log("Ingredienttray - tray pos " + trayPos);

                    Quaternion trayRot = trayPositions[i].transform.rotation;
                    Debug.Log("Ingredienttray - tray rot" + trayRot);

                    //Generic drop functions
                    GameObject trayIngredient = Instantiate(networkedPlayerInteraction.objectContainerPrefab, trayPos, trayRot);

                    trayIngredient.GetComponent<Rigidbody>().isKinematic = false;

                    //get sceneobject script from the sceneobject prefab
                    ObjectContainer ingredientContainer = trayIngredient.GetComponent<ObjectContainer>();

                    //instantiate the right ingredient as a child of the object
                    ingredientContainer.SetHeldIngredient(heldIngredient);

                    //sync var the helditem in scene object to the helditem in the player
                    ingredientContainer.heldIngredient = heldIngredient;

                    //set player's sync var to nothing so clients won't see the ingredient anymore
                    heldIngredient = HeldIngredient.nothing;

                    //spawn the scene object on network for everyone to see
                    NetworkServer.Spawn(trayIngredient);

                    //set layer to uninteractable
                    trayIngredient.layer = LayerMask.NameToLayer("UnInteractable");

                    //Set the ingredient on tray to be the spawned object
                    ingredientsOnTray[i] = trayIngredient;


                    //clear the inventory after dropping on tray
                    networkedPlayerInteraction.objectsInInventory.Clear();

                    return;
                }
            }
        }
        else
        {
            //instantiate scene object on the server at the drop point
            Vector3 pos = networkedPlayerInteraction.dropPoint.transform.position;
            Quaternion rot = networkedPlayerInteraction.dropPoint.transform.rotation;
            GameObject newContainer = Instantiate(networkedPlayerInteraction.objectContainerPrefab, pos, rot);

            //set Rigidbody as non-kinematic on SERVER only (isKinematic = true in prefab)
            newContainer.GetComponent<Rigidbody>().isKinematic = false;

            //get sceneobject script from the sceneobject prefab
            ObjectContainer objectContainer = newContainer.GetComponent<ObjectContainer>();

            //instantiate the right ingredient as a child of the object
            objectContainer.SetHeldIngredient(heldIngredient);

            //sync var the helditem in scene object to the helditem in the player
            objectContainer.heldIngredient = heldIngredient;

            //set player's sync var to nothing so clients won't see the ingredient anymore
            heldIngredient = HeldIngredient.nothing;

            //spawn the scene object on network for everyone to see
            NetworkServer.Spawn(newContainer);

            //clear inventory after dropping
            networkedPlayerInteraction.objectsInInventory.Clear();
        }

    }

    //server will spawn a dirty plate
    [ServerCallback]
    void SpawnDirtyPlate()
    {
        //Instantiate scene object on the server at a fixed position
        //Temporarily at drop pos
        Vector3 pos = networkedPlayerInteraction.dropPoint.transform.position;
        Quaternion rot = networkedPlayerInteraction.dropPoint.transform.rotation;
        GameObject dirtyPlateContainer = Instantiate(networkedPlayerInteraction.objectContainerPrefab, pos, rot);

        //set Rigidbody as non-kinematic on the instantiated object only (isKinematic = true in prefab)
        dirtyPlateContainer.GetComponent<Rigidbody>().isKinematic = false;

        //get sceneobject script from the sceneobject prefab
        ObjectContainer objectContainer = dirtyPlateContainer.GetComponent<ObjectContainer>();

        //instantiate the right item as a child of the object
        objectContainer.SetHeldIngredient(HeldIngredient.dirtyplate);

        //sync var the helditem in scene object to the helditem in the player
        objectContainer.heldIngredient = heldIngredient;

        //change layer of the container
        dirtyPlateContainer.layer = LayerMask.NameToLayer("TableItem");

        //spawn the scene object on network for everyone to see
        NetworkServer.Spawn(dirtyPlateContainer);
    }

    //called from client to server to pick up item
    [Command]
    public void CmdPickUpIngredient()
    {
        //set player's syncvar so clients can show the right ingredient
        //according to which item the sceneobject currently contains
        Debug.Log("NetworkedIngredientInteraction - " + networkedPlayerInteraction.detectedObject.tag + " was picked up!");

        if (networkedPlayerInteraction.detectedObject.tag == "DirtyPlate")
        {
            networkedPlayerInteraction.objectsInInventory.Add(networkedPlayerInteraction.detectedObject);
        }

        //destroy the scene object when it has been picked up
        NetworkServer.Destroy(networkedPlayerInteraction.detectedObject);

    }

    #endregion


    #region Triggers

    //TRIGGER ZONES
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "IngredientTableZone")
        {
            Debug.Log("NetworkedIngredientInteraction - Near the ingredient tray!");
            nearIngredientTray = true;
            trayParentZone = other.gameObject; //hit zone is the tray parent zone

            trayPositions = trayParentZone.GetComponent<IngredientTrayZones>().trayPositions;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "IngredientTableZone")
        {
            Debug.Log("NetworkedIngredientInteraction - Exited ingredient tray");
            nearIngredientTray = false;
        }
    }


    #endregion



}
