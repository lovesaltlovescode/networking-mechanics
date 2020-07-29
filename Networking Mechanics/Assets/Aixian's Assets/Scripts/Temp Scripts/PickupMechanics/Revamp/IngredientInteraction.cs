using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles interaction with ingredient
/// pick up, put down, place on table
/// changes player state according to the conditions!!!
/// conditions: near ingredient, nothing in inventory
/// ONLY CHECKS FOR INGREDIENT LAYER -> check if detected object is ingredient
/// All the logic for checking which function to run will be done through PlayerStates
/// </summary>
public class IngredientInteraction : MonoBehaviour
{

    public bool ingredientDetected;

    public bool nearIngredientTray; //check if player is in the ingredient tray zone

    public bool nearIngredientShelves; //check if player is near the ingredient shelves

    [Header("Ingredient Tray")]

    //2 arrays, 1 array to store the gos on the tray, if there is nothing (by default) it is a null element
    //second array will store the transform for the traypositions, public array

    public Transform[] trayPositions; //array to contain all tray positions

    public GameObject[] ingredientsOnTray = new GameObject[4]; //array to contain all ingredients on the tray

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("IngredientInteraction - Script initialised");
    }


    //Pickup ingredient function
    public void PickUpIngredient(GameObject detectedIngredient, List<GameObject> Inventory, Transform attachPoint)
    {
       
        //if there's something in the inventory, return
        if(Inventory.Count == 1)
        {
            return;
        }

        Debug.Log("IngredientInteraction - Pick up ingredient");

        //Parent to attachment point and transform
        detectedIngredient.transform.parent = attachPoint.transform;
        detectedIngredient.transform.position = attachPoint.position;

        //Add to inventory
        Inventory.Add(detectedIngredient);

         //Change layer to Player/pickeup so cannot be detected by other players
        // detectedObject.layer = LayerMask.NameToLayer("PickedUp");

        Debug.Log("Ingredient Interaction - Player is holding " + PlayerInteractionManager.detectedObject);
    }

    //Drop ingredient function
    //If holding an ingredient -> held object
    public void DropIngredient(GameObject heldIngredient, List<GameObject> Inventory, Transform dropOffPoint)
    {

        //if near ingredient tray
        if (nearIngredientTray)
        {
            //function to loop through game object array, check for a null element, if an index is null
            for (int i = 0; i < ingredientsOnTray.Length; i++)
            {
                if(ingredientsOnTray[i] == null)
                {
                    //if the gameobject is null, assign it as held ingredient
                    ingredientsOnTray[i] = heldIngredient;
                    heldIngredient.transform.position = trayPositions[i].transform.position;
                    heldIngredient.transform.parent = trayPositions[i].transform;

                    //set layer to uninteractable
                    heldIngredient.layer = LayerMask.NameToLayer("UnInteractable");

                    //remove detected object, player should not be seeing this object anymore
                    PlayerInteractionManager.detectedObject = null;

                    //Generic functions

                    Debug.Log("IngredientInteraction - Drop ingredient");

                    //Remove from inventory
                    Inventory.Remove(heldIngredient);

                    //Set rotation back to 0
                    heldIngredient.transform.rotation = Quaternion.identity;

                    return;
                }
            }

        }
        else
        {
            //normal drop function
            heldIngredient.transform.position = dropOffPoint.position;
            //Generic function regardless of drop off location
            Debug.Log("IngredientInteraction - Drop ingredient");

            //Remove from inventory
            Inventory.Remove(heldIngredient);

            //unparent
            heldIngredient.transform.parent = null;

            //Set rotation back to 0
            heldIngredient.transform.rotation = Quaternion.identity;

        }

        



    }

    // Update is called once per frame
    void Update()
    {
       if(PlayerInteractionManager.detectedObject && PlayerInteractionManager.detectedObject.layer == 15)
        {
            ingredientDetected = true;
        }
        else
        {
            ingredientDetected = false;
        }

        CheckIngredientCriteria();
    }

    //handles all logic for changing the player state
    //only works if the object detected by radar is an ingredient layer
    public void CheckIngredientCriteria()
    {
        if (ingredientDetected)
        {
            //PICK UP CRITERIA
            //if inventory is not full, able to pick up
            if (!PlayerInteractionManager.IsInventoryFull())
            {
                print("IngredientInteraction - Can pick up ingredient!");
                //Switch the state
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanPickUpIngredient);
            }

            //DROP CRITERIA
            else if (PlayerInteractionManager.IsInventoryFull())
            {
                //if inventory is full, able to drop
                print("IngredientInteraction - Can drop ingredient");


                //set spawned prefab bools false so they are now just like any normal ingredient
                //removes them from being permanently detected object
                ShelfInteraction.spawnedEgg = false;
                ShelfInteraction.spawnedChicken = false;
                ShelfInteraction.spawnedCucumber = false;
                ShelfInteraction.spawnedRice = false;


                //switch the state
                if(!nearIngredientShelves)
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanDropIngredient);
            }
        }
        //if there is no detected object + the player isn't holding a customer, player state returns to default
        if (!PlayerInteractionManager.detectedObject && !WashInteraction.placedPlateInSink)
        {
            PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.Default);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //if enter the ingredient tray zone
        if (other.tag == "IngredientTableZone")
        {
            Debug.Log("IngredientInteraction - Near the ingredient tray!");
            nearIngredientTray = true;
        }

        if (other.tag == "ShelfZone")
        {
            //if player is in shelf zone, they cannot drop ingredients
            nearIngredientShelves = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "IngredientTableZone")
        {
            Debug.Log("IngredientInteraction - Exited ingredient tray!");
            nearIngredientTray = false;
        }

        if (other.tag == "ShelfZone")
        {
            //if player is in shelf zone, they cannot drop ingredients
            nearIngredientShelves = false;
        }
    }
}
