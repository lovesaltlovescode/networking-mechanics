using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Base class for ingredient shelves
//Ingredients will be spawned when players click the interact button while facing the shelf
//held object will appear, object icon will appear
//players can drop the ingredients and place them on the tray
//players can also trash ingredients

public class IngredientShelf : MonoBehaviour, I_Interactable
{

    //VARIABLES FOR SHELVES

    //Object follow script
    FollowObject followScript;


    //Ingredient prefab to be spawned
    //Picked up and added to inventory on button press
    public GameObject ingredientPrefab;

    //Picked up object (ingredient prefab set as this)
    [SerializeField] private GameObject pickedUpObject;

    //Ingredient icon to be set active/inactive on the button
    public Image ingredientIcon;

    //Object that player will be holding on their head
    //Set active/inactive depending on if player is holding an ingredient
    public GameObject heldIngredient;

    //Reference to player prefab
    public GameObject playerPrefab;

    //Reference to ingredient tray position
    public Transform trayPos;


    //States of the ingredient, perform different functions at each state
    //TODO: Move this to another script
    public enum IngredientState
    {
        Spawnable,
        Droppable,
        PlaceOnIngredientTable, //for the ingredients to be placed on the table
        //Servable, //for final dishes to be served to customers
        UnInteractable //Object cannot be interacted with
    }

    public IngredientState ingredientState;

    // Start is called before the first frame update
    void Start()
    {
        ingredientState = IngredientState.Spawnable;

        followScript = ingredientPrefab.GetComponent<FollowObject>();
    }

    #region HandleRenderers

    //Handle enabling or disabling renderers of picked up object accordingly

    public void EnableRenderer()
    {
        //Set meshrenderer enabled for optimisation
        //Loop through all renderer in children and enable it
        Renderer[] rend = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rend)
        {
            r.enabled = true;
        }
    }

    public void DisableRenderer()
    {
        //Set meshrenderer enabled for optimisation
        //Loop through all renderer in children and enable it
        Renderer[] rend = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rend)
        {
            r.enabled = false;
        }
    }
    #endregion

    //Function to spawn ingredient prefab as pickedup object
    //Add the spawned object to inventory
    //Sets icon active and held object active
    //Parents spawned object to player transform
    public void PickUpObject()
    {
        //spawn the ingredient prefab
        pickedUpObject = Instantiate(ingredientPrefab, transform.position, Quaternion.identity);

        //Add to inventory
        PickUppable.objectsInInventory.Add(pickedUpObject);

        //Print statement
        Debug.Log("IngredientShelf: Picked up " + pickedUpObject);

        //Set object on player's head active
        heldIngredient.GetComponent<Renderer>().enabled = true;

        //Set pickedupobject inactive
        DisableRenderer();

        followScript.enabled = true;

        pickedUpObject.transform.parent = playerPrefab.transform;

        ingredientState = IngredientState.Droppable;

        pickedUpObject.layer = LayerMask.NameToLayer("PickedUp");

    }

    //Function to drop the ingredient prefab as picked up object
    //Remove the spawned object from inventory
    //Sets icon inactive and held object inactive
    //Unparents pickedupobject
    public void DropObject()
    {
        //Remove from inventory
        PickUppable.objectsInInventory.Remove(pickedUpObject);

        //Deactivate icon
        ingredientIcon.gameObject.SetActive(false);

        //Print statement
        Debug.Log("IngredientShelf: Dropped " + pickedUpObject);

        //Deactivate held object
        heldIngredient.GetComponentInChildren<Renderer>().enabled = false;

        //Unparent object
        pickedUpObject.transform.parent = null; //no more parent
        EnableRenderer();

        followScript.enabled = false;

        //Change state back to pickuppable
        ingredientState = IngredientState.Spawnable;

        //set layer mask
        pickedUpObject.layer = LayerMask.NameToLayer("Default");
    }

    //Function to place the ingredient prefab on the ingredient table
    //Remove spawned object from inventory
    //Set icon inactive and held object inactive
    //Unparent from parent and move to table position
    //TODO: Check if there is space to place it on table
    public void PlaceOnTable()
    {
        //Remove item from inventory
        PickUppable.objectsInInventory.Remove(pickedUpObject);

        //set icon inactive
        ingredientIcon.gameObject.SetActive(false);

        //set held item inactive
        heldIngredient.GetComponentInChildren<Renderer>().enabled = false;

        //set pickedup object active and move to table pos
        //unparent from table
        EnableRenderer();
        pickedUpObject.transform.position = trayPos.position;
        pickedUpObject.transform.parent = null;

        followScript.enabled = false;

        //set layer mask
        pickedUpObject.layer = LayerMask.NameToLayer("IngredientOnTable");

        //set state as uninteractable
        ingredientState = IngredientState.UnInteractable;
    }


    // Update is called once per frame
    void Update()
    {

    }
   

}
