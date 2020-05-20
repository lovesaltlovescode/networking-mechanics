using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class for pickuppable object
//Will be on every pickuppable object
public class PickUppable : MonoBehaviour
{

    //VARIABLES THAT ALL OBJECTS WILL HAVE

    //object tag
    private string objectTag;

    //object icon to set active/inactive on the button
    public Image objectIcon;

    //wash icon to set active and gray
    public Image washIcon;

    //washing icon that fills according to timer
    public Image washingIcon;

    //bool to check if plate was being washed
    public bool wasWashing; //set true if player was interrupted from washing


    //Gameobject for clean plate to be set active/inactive
    public GameObject cleanPlate;

    //the game object that will be picked up and added to inventory
    //set inactive and parent to player
    public static GameObject pickedUpObject;
    //when the player detects an object, this object will be set as the hit object

    //the object that the player will be holding if they pick up the object
    //ie. a cup of rice on their head
    //set active/inactive depending on if the player is holding the object
    public GameObject heldObject;

    //List of objects in player's inventory, static and same throughout all scripts
    public static List<GameObject> objectsInInventory = new List<GameObject>();

    //reference to the player prefab to get its transform
    public GameObject playerPrefab;

    //reference to sink position
    public Transform sinkPos;

    //reference to ingredient table position
    public Transform tablePos;



    //Reference follow script
    //public FollowObject followObject;

    //Different states of the object
    public enum ObjectState
    {
        PickUppable,
        Droppable,
        PlaceOnIngredientTable, //for the ingredients to be placed on the table
        Servable, //for final dishes
        PlaceInSink, //for when plate can be placed in the sink (trigger enter)
        Washable, //for when plate has been placed in the sink and can be washed
        Washing, //Set when player  has tapped the button to wash
        Washed, //Set when plates are done washing after time elapsed
        StoppedWashing, //Set when player leaves the sink area and washing is interrupted
        UnInteractable //Object cannot be interacted with
    }

    public ObjectState objectState;


    // Start is called before the first frame update
    void Start()
    {
        objectState = ObjectState.PickUppable; //all objects start as pickuppables

    }

    #region PickUp Object

    //Function to pick up object, does not check for condition
    //Add closest object to the inventory
    //Instantiate a sprite icon and print a unique statement
    //called when the button is pressed
    public void PickUpObject()
    {
        //Add to inventory
        objectsInInventory.Add(pickedUpObject);

        //Activate the required icon
        objectIcon.gameObject.SetActive(true);

        //Print statement
        Debug.Log("PickUppable: Picked up " + pickedUpObject);

        //Set object on Player's head active
        heldObject.SetActive(true);

        //Set pickedUpObject inactive 
        pickedUpObject.SetActive(false);
        //parent to player object
        pickedUpObject.transform.parent = playerPrefab.transform;

        //set picked up object to the same position as parent at 0 y
        pickedUpObject.transform.position = pickedUpObject.transform.position =
            new Vector3(playerPrefab.transform.position.x, 0, playerPrefab.transform.position.z);

        //Set state as droppable since this object is now in the player's inventory
        objectState = ObjectState.Droppable;

        //TODO: Check if it can be detected by other players even when inactive
        // If so, Change layer so that it is masked and not detected by other players
        pickedUpObject.layer = LayerMask.NameToLayer("PickedUp");

    }

    #endregion

    #region Drop Object

    //Function to drop object, does not check for condition
    //only the actions to perform when player drops object
    //deactivate the icon 
    //drop the pickedUpObject on the top of the zone ie. table
    //remove from player's inventory
    public void DropObject()
    {
        //Remove from inventory
        objectsInInventory.Remove(pickedUpObject);

        //Deactivate icon
        objectIcon.gameObject.SetActive(false);

        //Print statement
        Debug.Log("PickUppable: Dropped " + pickedUpObject);

        //Deactivate heldobject
        heldObject.SetActive(false);

        //Unparent object 
        pickedUpObject.transform.parent = null; //no more parent
        pickedUpObject.SetActive(true);

        //Change state back to pickuppable
        objectState = ObjectState.PickUppable;

        //TODO: Change layer so it can be detected by other players
        pickedUpObject.layer = LayerMask.NameToLayer("Default");
    }

    #endregion

    #region Place Ingredient On Table

    //function to place ingredient on table
    public void PlaceIngredientOnTable()
    {
        //Remove item from inventory
        objectsInInventory.Remove(pickedUpObject);

        //set object icon inactive
        objectIcon.gameObject.SetActive(false);

        //set held item inactive
        heldObject.SetActive(false);

        //set pickedup object active and move to tablepos
        //unparent from table
        pickedUpObject.transform.position = tablePos.position;
        pickedUpObject.transform.parent = null;
        pickedUpObject.SetActive(true);

        //set layer mask to ingredientontable
        pickedUpObject.layer = LayerMask.NameToLayer("IngredientOnTable");

        //set state as uninteractable
        objectState = ObjectState.UnInteractable;

    }

    #endregion

    #region Wash Object

    //Function to place object in sink
    public void PlaceObjectInSink()
    {

        //Remove item from inventory
        objectsInInventory.Remove(pickedUpObject);

        //Set object icon inactive
        objectIcon.gameObject.SetActive(false);

        //Set held object inactive
        heldObject.SetActive(false);

        //Print statement
        Debug.Log("PickUppable: Ready to wash " + pickedUpObject);

        //Move pickedup object to the sink's position
        //if the position is null, that means this object shouldnt be washing, throw an error
        //Unparent from player
        pickedUpObject.transform.parent = null;
        pickedUpObject.transform.position = sinkPos.position;
        pickedUpObject.SetActive(true);

        //set wash icon active
        washIcon.gameObject.SetActive(true);

        //Set state as washable
        objectState = ObjectState.Washable;
        pickedUpObject.layer = LayerMask.NameToLayer("PlateOnSink");


    }

    //Function to wash object
    //Starts timer to wash object
    //After washed, spawn clean plate and destroy current object
    public void WashObject()
    {

        //set wash icon active
        //washIcon.gameObject.SetActive(true);

        //change wash icon to gray
        washIcon.color = Color.gray;



    }

    #endregion


    //check when to stop washing and handle what happens next
    public void CheckForStopWashing()
    {
        //if washing. check when to end it
        if (objectState == ObjectState.Washing)
        {
            //Once time is up, set state to washed
            if (washingIcon.fillAmount == 1f)
            {
                objectState = ObjectState.Washed;
            }
        }

        //if object has been washed
        if (pickedUpObject != null && objectState == ObjectState.Washed)
        {
            //Set pickedupobject in sink inactive
            pickedUpObject.SetActive(false);

            //Set clean plate active
            cleanPlate.SetActive(true);

            //after washing, destroy the pickedup object
            Destroy(pickedUpObject);

            //set washicon inactive and back to white
            washIcon.color = Color.white;
            washIcon.gameObject.SetActive(false);

            //set state of pickedupobject to pickuppable
            objectState = ObjectState.PickUppable;
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (gameObject.tag == "DirtyPlate")
        {

            CheckForStopWashing();

            if (pickedUpObject != null)
            {
                //if picked up object is at the sink
                if (pickedUpObject.transform.position == sinkPos.transform.position)
                {
                    //then it was being washed
                    wasWashing = true;
                }
            }

            //if player was interrupted from washing ie. left sink area
            //then reset wash icon colour and set it inactive
            if (objectState == ObjectState.StoppedWashing)
            {
                washIcon.color = Color.white;
                washIcon.gameObject.SetActive(false);
            }

            //if it is washable set icon active
            if (objectState == ObjectState.Washable)
            {
                washIcon.gameObject.SetActive(true);
            }
        }




    }



}