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

    [Header("Ingredient prefabs")]
    public GameObject eggPrefab;
    public GameObject chickenPrefab;
    public GameObject cucumberPrefab;
    public GameObject ricePrefab;

    private GameObject spawnedEggPrefab;
    private GameObject spawnedChickenPrefab;
    private GameObject spawnedCucumberPrefab;
    private GameObject spawnedRicePrefab;

    //bool to check if player spawned any ingredient
    //while this is true, the detected object/held object will always be the prefab spawned
    public static bool spawnedEgg;
    public static bool spawnedChicken;
    public static bool spawnedCucumber;
    public static bool spawnedRice;

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
        Debug.Log("ShelfInteraction - Currently held ingredient is " + heldIngredient + "and spawned ingredient is " + spawnedEggPrefab);

        spawnedEgg = true;

    }

    //spawn chicken when player state is can spawn chicken
    public void SpawnChicken(GameObject heldIngredient, List<GameObject> Inventory, Transform attachPoint)
    {
        //attach to the player's attachment point
        spawnedChickenPrefab = Instantiate(chickenPrefab, attachPoint.position, Quaternion.identity);
        spawnedChickenPrefab.transform.parent = attachPoint.transform;

        //add to inventory
        Inventory.Add(spawnedChickenPrefab);

        heldIngredient = spawnedChickenPrefab;
        Debug.Log("ShelfInteraction - Currently held ingredient is " + heldIngredient + "and spawned ingredient is " + spawnedChickenPrefab);

        spawnedChicken = true;
    }

    //spawn cucumber when player state is can spawn cucumber
    public void SpawnCucumber(GameObject heldIngredient, List<GameObject> Inventory, Transform attachPoint)
    {
        //attach to the player's attachment point
        spawnedCucumberPrefab = Instantiate(cucumberPrefab, attachPoint.position, Quaternion.identity);
        spawnedCucumberPrefab.transform.parent = attachPoint.transform;

        //add to inventory
        Inventory.Add(spawnedCucumberPrefab);

        heldIngredient = spawnedCucumberPrefab;
        Debug.Log("ShelfInteraction - Currently held ingredient is " + heldIngredient + "and spawned ingredient is " + spawnedCucumberPrefab);

        spawnedCucumber = true;
    }

    //spawn rice when player state is can spawn rice
    public void SpawnRice(GameObject heldIngredient, List<GameObject> Inventory, Transform attachPoint)
    {
        //attach to the player's attachment point
        spawnedRicePrefab = Instantiate(ricePrefab, attachPoint.position, Quaternion.identity);
        spawnedRicePrefab.transform.parent = attachPoint.transform;

        //add to inventory
        Inventory.Add(spawnedRicePrefab);

        heldIngredient = spawnedRicePrefab;
        Debug.Log("ShelfInteraction - Currently held ingredient is " + heldIngredient + "and spawned ingredient is " + spawnedRicePrefab);

        spawnedRice = true;
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

        CheckSpawnedIngredient();
    }

    //handle all booleans to check if ingredient is spawned
    public void CheckSpawnedIngredient()
    {
        if (spawnedEgg)
        {
            Debug.Log("ShelfInteraction: Spawned egg is true");
            PlayerInteractionManager.detectedObject = spawnedEggPrefab;
            spawnedChicken = false;
            spawnedCucumber = false;
            spawnedRice = false;
        }

        if (spawnedChicken)
        {
            Debug.Log("ShelfInteraction: Spawned chicken is true");
            PlayerInteractionManager.detectedObject = spawnedChickenPrefab;
            spawnedCucumber = false;
            spawnedRice = false;
            spawnedEgg = false;
        }

        if (spawnedCucumber)
        {
            Debug.Log("ShelfInteraction: Spawned cucumber is true");
            PlayerInteractionManager.detectedObject = spawnedCucumberPrefab;
            spawnedChicken = false;
            spawnedRice = false;
            spawnedEgg = false;
        }

        if (spawnedRice)
        {
            Debug.Log("ShelfInteraction: Spawned rice is true");
            PlayerInteractionManager.detectedObject = spawnedRicePrefab;
            spawnedChicken = false;
            spawnedCucumber = false;
            spawnedEgg = false;
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
            if (PlayerInteractionManager.IsInventoryFull())
            {
                //if inventory full, then set state to default
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.Default);
            }

        }

    }

    public void OnTriggerStay(Collider other)
    {
        if (shelfDetected)
        {
            //if player inventory is not full
            if (!PlayerInteractionManager.IsInventoryFull())
            {
                var detectedObject = PlayerInteractionManager.detectedObject.tag;

                if (other.tag == "EggShelfZone")
                {
                    if(detectedObject == "EggShelf")
                    {
                        PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanSpawnEgg);
                        Debug.Log("ShelfInteraction - Player can spawn an egg!");
                    }
                    
                }

                else if (other.tag == "ChickenShelfZone" && detectedObject == "ChickenShelf")
                {
                    PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanSpawnChicken);
                    Debug.Log("ShelfInteraction - Player can spawn a chicken!");
                }

                else if (other.tag == "CucumberShelfZone" && detectedObject == "CucumberShelf")
                {
                    PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanSpawnCucumber);
                    Debug.Log("ShelfInteraction - Player can spawn a cucumber!");
                }

                else if (other.tag == "RiceTubZone" && detectedObject == "RiceTub")
                {
                    PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanSpawnRice);
                    Debug.Log("ShelfInteraction - Player can spawn some rice!");
                }
            }
        }
        
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
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanDropIngredient);
            }
        }



    }
}
