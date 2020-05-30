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
    public enum IngredientState
    {
        PickUppable,
        Droppable,
        PlaceOnIngredientTable, //for the ingredients to be placed on the table
        //Servable, //for final dishes to be served to customers
        UnInteractable //Object cannot be interacted with
    }

    public IngredientState ingredientState;

    // Start is called before the first frame update
    void Start()
    {
        ingredientState = IngredientState.PickUppable;
    }

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
        heldIngredient.SetActive(true);

        //Set pickedupobject inactive

    }

    //Function to drop the ingredient prefab as picked up object
    //Remove the spawned object from inventory
    //Sets icon inactive and held object inactive
    //Unparents pickedupobject
    public void DropObject()
    {

    }

    //Function to place the ingredient prefab on the ingredient table
    //Remove spawned object from inventory
    //Set icon inactive and held object inactive
    //Unparent from parent and move to table position
    //TODO: Check if there is space to place it on table
    public void PlaceOnTable()
    {
        throw new System.NotImplementedException();
    }


    // Update is called once per frame
    void Update()
    {

    }
   

}
