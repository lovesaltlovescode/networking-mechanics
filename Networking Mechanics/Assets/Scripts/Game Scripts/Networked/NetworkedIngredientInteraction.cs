using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Handles changing of player states
/// Depending on what the player is looking at, state changes
/// Spawns ingredients
/// TODO: Only spawn ingredient if player is in the zone
/// </summary>
/// 

//enums of ingredients that could be spawned, placed outside class so it can be accessed elsewhere
public enum HeldIngredient
{
    nothing,
    egg,
    chicken,
    cucumber
}

public class NetworkedIngredientInteraction : NetworkBehaviour
{

    [Header("Ingredient Tray")]
    //2 arrays, 1 array to store the gos on the tray, if there is nothing (by default) it is a null element
    //second array will store the transform for the traypositions, public array

    //TODO: ADD IN STATIC LATER
    public static GameObject trayParentZone; //Tray object that contains all ingredient tray positions

    private static Transform[] trayPositions; //array to contain all tray positions

    public static GameObject[] ingredientsOnTray = new GameObject[4]; //array to contain all ingredients on the tray


    //player attachment point
    public GameObject attachmentPoint;
    //player drop point, where items should be dropped
    public GameObject dropPoint;

    [Header("Spawnable Objects")]
    public GameObject objectContainerPrefab;

    //PREFABS to be spawned
    public GameObject cucumberPrefab;
    public GameObject eggPrefab;
    public GameObject chickenPrefab;

    //when the helditem changes, call onchangeingredient method
    [SyncVar(hook = nameof(OnChangeIngredient))]
    public HeldIngredient heldIngredient;

    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    [Header("Booleans")]

    public bool ingredientDetected;

    public bool nearIngredientTray; //check if player is in the ingredient tray zone

    public bool nearIngredientShelves; //check if player is near the ingredient shelves

    //if detectedobj is an ingredient shelf, this is true
    public bool detectedShelf = false;

    
    void OnChangeIngredient(HeldIngredient oldIngredient, HeldIngredient newIngredient)
    {
        //Debug.Log("NetworkedIngredientInteraction - Starting coroutine!");
        StartCoroutine(ChangeIngredient(newIngredient));
    }

    IEnumerator ChangeIngredient(HeldIngredient newIngredient)
    {
        //If the player is holding something
        while (attachmentPoint.transform.childCount > 0)
        {
            //if player is holding nothing, destroy the existing child
            if(newIngredient == HeldIngredient.nothing)
            {
                Debug.Log("NetworkedIngredientInteraction - Destroying held object");
                Destroy(attachmentPoint.transform.GetChild(0).gameObject);
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
                var chicken = Instantiate(chickenPrefab, attachmentPoint.transform);
                networkedPlayerInteraction.objectsInInventory.Add(chicken);
                break;

            case HeldIngredient.egg:
                var egg = Instantiate(eggPrefab, attachmentPoint.transform);
                networkedPlayerInteraction.objectsInInventory.Add(egg);
                break;

            case HeldIngredient.cucumber:
                var cucumber = Instantiate(cucumberPrefab, attachmentPoint.transform);
                networkedPlayerInteraction.objectsInInventory.Add(cucumber);
                break;

        }
    }

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
            if(attachmentPoint.transform.childCount == 0)
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

            //if player is holding something
            if(networkedPlayerInteraction.objectsInInventory.Count > 0)
            {
                Debug.Log("NetworkedIngredientInteraction - Able to drop ingredient!");
                networkedPlayerInteraction.playerState = PlayerState.CanDropIngredient;
            }

            
        }
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
            for(int i = 0; i < ingredientsOnTray.Length; i++)
            {
                if(ingredientsOnTray[i] == null)
                {
                    //if null, assign it as held ingredient
                    var detectedIngredient = networkedPlayerInteraction.detectedObject;
                    Debug.Log("Ingredienttray - detected ingredient " + detectedIngredient);

                    Vector3 trayPos = trayPositions[i].transform.position;
                    Debug.Log("Ingredienttray - tray pos " + trayPos);

                    Quaternion trayRot = trayPositions[i].transform.rotation;
                    Debug.Log("Ingredienttray - tray rot" + trayRot);

                    //Generic drop functions
                    GameObject trayIngredient = Instantiate(objectContainerPrefab, trayPos, trayRot);

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
            Vector3 pos = dropPoint.transform.position;
            Quaternion rot = dropPoint.transform.rotation;
            GameObject newContainer = Instantiate(objectContainerPrefab, pos, rot);

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

    //called from client to server to pick up ingredient
    [Command]
    public void CmdPickUpIngredient()
    {
        //set player's syncvar so clients can show the right ingredient
        //according to which item the sceneobject currently contains
        Debug.Log("NetworkedIngredientInteraction - " + networkedPlayerInteraction.detectedObject.tag + " was picked up!");

        //destroy the scene object when it has been picked up
        NetworkServer.Destroy(networkedPlayerInteraction.detectedObject);
    }

    //TRIGGER ZONES
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "IngredientTableZone")
        {
            Debug.Log("NetworkedIngredientInteraction - Near the ingredient tray!");
            nearIngredientTray = true;
            trayParentZone = other.gameObject; //hit zone is the tray parent zone
            
            trayPositions = trayParentZone.GetComponent<IngredientTrayZones>().trayPositions;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "IngredientTableZone")
        {
            Debug.Log("NetworkedIngredientInteraction - Exited ingredient tray");
            nearIngredientTray = false;
        }
    }


}
