using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Networked player interaction
/// Player raycast for detected object
/// Depending on which object is is, change the function
/// </summary>
/// 
//State of player
public enum PlayerState
{
    Default,

    CanDropIngredient,
    CanPickUpIngredient,

    //Spawning ingredients from shelf
    CanSpawnChicken,
    CanSpawnEgg,
    CanSpawnCucumber,
}

public class NetworkedPlayerInteraction : NetworkBehaviour
{
    //RAYCAST VARIABLES
    [Header("Raycast Variables")]
    public float raycastLength = 1.5f; //how far the raycast extends

    [SerializeField] private float distFromObject; //distance from the object looking at

    //mask these layers, they do not need to be raycasted
    //Player, environment, zones and uninteractable layers
    private int layerMask = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 13;

    //Detected object, object player is looking at
    public GameObject detectedObject;

    public List<GameObject> objectsInInventory = new List<GameObject>();

    public PlayerState playerState;

    //different scripts to reference
    [SerializeField] private NetworkedIngredientInteraction networkedIngredientInteraction;

    private void Awake()
    {
        networkedIngredientInteraction = GetComponent<NetworkedIngredientInteraction>();
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
                networkedIngredientInteraction.UpdateIngredient(HeldIngredient.chicken);
                break;

            case PlayerState.CanSpawnEgg:
                Debug.Log("NetworkedPlayerInteraction - Spawn a chicken!");
                networkedIngredientInteraction.UpdateIngredient(HeldIngredient.egg);
                break;

            case PlayerState.CanSpawnCucumber:
                Debug.Log("NetworkedPlayerInteraction - Spawn a chicken!");
                networkedIngredientInteraction.UpdateIngredient(HeldIngredient.cucumber);
                break;

            case PlayerState.CanDropIngredient:
                Debug.Log("NetworkedPlayerInteraction - Drop the ingredient!");
                networkedIngredientInteraction.DropIngredient();
                break;

            case PlayerState.CanPickUpIngredient:
                Debug.Log("NetworkedPlayerInteraction - Pick up the ingredient!");
                networkedIngredientInteraction.PickUpIngredient();
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

        if (!detectedObject && !IsInventoryFull())
        {
            playerState = PlayerState.Default;
        }

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

        //if (playerState == PlayerState.FinishedWashingPlate)
        //{
        //    washInteraction.FinishWashingPlate();
        //}
    }
}
