using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles spawning of drink when player looks at the drink machine
/// drink spawns onto the counter position
/// player can either have or not have an object in their inventory
/// cooldown time after spawning
/// </summary>
public class DrinkInteraction : MonoBehaviour
{

    [Header("Cooldown")]
    public Image cooldownImg;
    public float cooldown = 5f;
    public bool isCooldown = false;



    public bool drinkMachineDetected; //checks if drink machine is detected
    public bool drinkDetected; //checks if drink is detected

    public Transform[] drinkPositions; //where the drink should be spawned
    public GameObject[] drinksOnCounter; //number of drinks on the counter

    public GameObject drinkPrefab; //prefab to be spawned
    public bool spawnedDrink; //if a drink has been spawned

    // Start is called before the first frame update
    void Start()
    {
        cooldownImg.fillAmount = 1;
    }

    public void SpawnDrink()
    {
        if (isCooldown)
        {
            return;
        }

        for(int i = 0; i < drinksOnCounter.Length; i++)
        {
            if(drinksOnCounter[i] == null)
            {
                var drink = Instantiate(drinkPrefab, drinkPositions[i].transform.position, Quaternion.identity);
                drinksOnCounter[i] = drink;

                spawnedDrink = true;
                isCooldown = true;
                cooldownImg.fillAmount = 0;
                return;
            }
        }

        
        

    }

    public void PickUpDrink(GameObject heldDrink, Transform attachPoint, List<GameObject> Inventory)
    {
        //if there's something in the inventory, return
        if (Inventory.Count == 1)
        {
            return;
        }

        Debug.Log("DrinkInteraction - Pick up drink");

        //Parent to attachment point and transform
        heldDrink.transform.parent = attachPoint.transform;
        heldDrink.transform.position = attachPoint.position;

        //Add to inventory
        Inventory.Add(heldDrink);

        //PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.HoldingDrink;

        //Change layer to Player/pickeup so cannot be detected by other players
        // detectedObject.layer = LayerMask.NameToLayer("PickedUp");

        //change held object to be the detected object
    }

    // Update is called once per frame
    void Update()
    {
        //if detected and is drink machine
        if (PlayerInteractionManager.detectedObject && PlayerInteractionManager.detectedObject.layer == 21)
        {
            drinkMachineDetected = true;
            //PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanSpawnDrink;
        }
        else
        {
            drinkMachineDetected = false;
        }

        //if drink
        if(PlayerInteractionManager.detectedObject && PlayerInteractionManager.detectedObject.layer == 22)
        {
            drinkDetected = true;

            if (!PlayerInteractionManager.IsInventoryFull())
            {
               // PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanPickUpDrink;
            }
        }
        else
        {
            drinkDetected = false;
        }

        //if cooldown
        if (isCooldown)
        {
            cooldownImg.fillAmount += 1 / cooldown * Time.deltaTime;
            if(cooldownImg.fillAmount >= 1)
            {
                cooldownImg.fillAmount = 1;
                isCooldown = false;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "DrinkZone")
        {
            drinkPositions = other.GetComponent<DrinkZones>().drinkPositions;
        }
    }
}
