using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles spawning of ingredient prefabs when player looks at a shelf
/// CHanges player state if player can spawn an ingredient
/// Checks if player is in  the zone of the shelf and is looking at the shelf
/// If their inventory is empty, they can spawn an ingredient directly onto their heads
/// </summary>
public class ShelfInteraction : MonoBehaviour
{

    public bool shelfDetected; //if detected object is shelf
    public bool facingShelf; //checks if player is in the shelf zone

    [Header("Ingredient prefabs")]
    public GameObject eggPrefab;
    public GameObject chickenPreab;
    public GameObject cucumberPrefab;
    public GameObject ricePrefab;

    private GameObject spawnedEggPrefab;

    public bool spawnedEgg;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ShelfInteraction - Script Initialised");
    }

    //spawn egg when player state is can spawn egg
    public void SpawnEgg(GameObject heldIngredient, List<GameObject> Inventory, Transform attachPoint)
    {
        
        //attach to the player's attachment point
        spawnedEggPrefab = Instantiate(eggPrefab, attachPoint.position, Quaternion.identity);
        spawnedEggPrefab.transform.parent = attachPoint.transform;

        //add to inventory
        Inventory.Add(spawnedEggPrefab);

        heldIngredient = spawnedEggPrefab;
        Debug.Log("ShelfInteraction - Currently held ingredient is " + heldIngredient + "and spawned ingredient is " + spawnedEgg);

        spawnedEgg = true;

        //TODO: Check what is the detected object and change to the spawned prefab

    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerInteractionManager.detectedObject && PlayerInteractionManager.detectedObject.layer == 14)
        {
            shelfDetected = true;
        }
        else
        {
            shelfDetected = false;
        }

        if (spawnedEgg)
        {
            PlayerInteractionManager.detectedObject = spawnedEggPrefab;
            PlayerInteractionManager.heldObject = spawnedEggPrefab;
        }
    }


    //if enter shelf trigger and shelf detected is true
    //player is facing shelf
    private void OnTriggerEnter(Collider other)
    {

        if(other.tag == "ShelfZone")
        {
            //player in shelf zone, cannot drop anything here
            Debug.Log("ShelfInteraction - Player is in the shelf zone!");    

        }
        if (other.tag == "EggShelfZone" && shelfDetected)
        {
            Debug.Log("help");
            //entered egg shelf zone
            if (!PlayerInteractionManager.IsInventoryFull())
            {
                //if inventory not full
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanSpawnEgg;
                Debug.Log("ShelfInteraction - Player can spawn an egg!");
            }
        }


    }


    private void OnTriggerStay(Collider other)
    {
        
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "ShelfZone")
        {
            Debug.Log("Player exited shelf zone!");
            if (PlayerInteractionManager.IsInventoryFull())
            {
                //if inventory is full, player can now drop items
                Debug.Log("ShelfInteraction - Player can now drop items!");
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanDropIngredient;
            }
        }
    }
}
