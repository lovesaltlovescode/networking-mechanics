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

    public Transform trayPosition; //TODO: Change to an array of predetermine dpositions for dropping off the ingredient

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("IngredientInteraction - Script initialised");
    }


    //Pickup ingredient function
    public void PickUpIngredient(GameObject detectedIngredient, List<GameObject> Inventory, Transform attachPoint)
    {
       
        Debug.Log("IngredientInteraction - Pick up ingredient");

        //Parent to attachment point and transform
        detectedIngredient.transform.parent = attachPoint.transform;
        detectedIngredient.transform.position = attachPoint.position;

        //Add to inventory
        Inventory.Add(detectedIngredient);

         //Change layer to Player/pickeup so cannot be detected by other players
        // detectedObject.layer = LayerMask.NameToLayer("PickedUp");

        //change held object to be the detected object
        PlayerInteractionManager.heldObject = detectedIngredient;
        Debug.Log("Ingredient Interaction - Player is holding " + PlayerInteractionManager.heldObject);
    }

    //Drop ingredient function
    //If holding an ingredient -> held object
    public void DropIngredient(GameObject heldIngredient, List<GameObject> Inventory, Transform dropOffPoint)
    {
        //if near table zone, then change the transform
        if (nearIngredientTray)
        {
            heldIngredient.transform.position = trayPosition.position;
            

            //set layer to uninteractable
            heldIngredient.layer = LayerMask.NameToLayer("UnInteractable");

            //remove detected object, player should not be seeing this object anymore
            PlayerInteractionManager.detectedObject = null;

        }
        else
        {
            //set transform to dropoff point
            heldIngredient.transform.position = dropOffPoint.position;
            
        }

        //Generic function regardless of drop off location
        Debug.Log("IngredientInteraction - Drop ingredient");

        //unparent
        heldIngredient.transform.parent = null;

        //Remove from inventory
        Inventory.Remove(heldIngredient);

        //Set rotation back to 0
        heldIngredient.transform.rotation = Quaternion.identity;
        
        //set held object to null, player is not holding anything
        PlayerInteractionManager.heldObject = null;

        //Change layer back to ingredient


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
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanPickUpIngredient;
            }

            //DROP CRITERIA
            else if (PlayerInteractionManager.IsInventoryFull())
            {
                //if inventory is full, able to drop
                print("IngredientInteraction - Can drop ingredient");
                //switch the state
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanDropIngredient;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //if enter the ingredient tray zone
        if(other.tag == "IngredientTableZone")
        {
            Debug.Log("IngredientInteraction - Near the ingredient tray!");
            nearIngredientTray = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "IngredientTableZone")
        {
            Debug.Log("IngredientInteraction - Exited ingredient tray!");
            nearIngredientTray = false;
        }
    }
}
