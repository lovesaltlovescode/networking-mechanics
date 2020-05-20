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

    //wash icon to set active/inactive when washing
    public Image washIcon;

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

    //counter for washing objects
    [SerializeField] private int washCount = 0;

    //Reference follow script
    //public FollowObject followObject;

    //Different states of the object
    public enum ObjectState
    {
        PickUppable,
        Droppable,
        Servable, //for final dishes
        PlaceInSink, //for when plate can be placed in the sink (trigger enter)
        Washable, //for when plate has been placed in the sink and can be washed
        Washing, //Set when player  has tapped the button to wash
        Washed //Set when plates are done washing after time elapsed
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

        //set picked up object to the same position as parent
        pickedUpObject.transform.position = pickedUpObject.transform.position = 
            new Vector3(playerPrefab.transform.position.x, pickedUpObject.transform.position.y, playerPrefab.transform.position.z);

        //Set state as droppable since this object is now in the player's inventory
        objectState = ObjectState.Droppable;

        //TODO: Check if it can be detected by other players even when inactive
        // If so, Change layer so that it is masked and not detected by other players

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
    }

    #endregion

    #region Wash Object

    //Function to place object in sink
    public void PlaceObjectInSink()
    {
        if(washCount == 0)
        {
            //first time pressing

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
            pickedUpObject.transform.position = sinkPos.transform.position;
            pickedUpObject.SetActive(true);

            //increase wash count
            washCount = 1;

            //set wash icon active
            washIcon.gameObject.SetActive(true);

            //Set state as washable
            objectState = ObjectState.Washable;

        }
    }

    //Function to wash object
    //Starts timer to wash object
    //After washed, spawn clean plate and destroy current object
    public void WashObject()
    {
        if (washCount == 1)
        {
            //second time this function is being called
            //start washing

            //change object state
            objectState = ObjectState.Washing;

            //Show completion gauge, downtime,
            //Slowly fill the wash icon until the time is up
            washIcon.gameObject.SetActive(false);


            //Once time is up, set state to washed
            objectState = ObjectState.Washed;

            //Set pickedupobject in sink inactive
            pickedUpObject.SetActive(false);

            //Set clean plate active
            cleanPlate.SetActive(true);

            //after washing, destroy the pickedup object
            Destroy(pickedUpObject);
        }
    }

    #endregion





    // Update is called once per frame
    void Update()
    {


    }



}
