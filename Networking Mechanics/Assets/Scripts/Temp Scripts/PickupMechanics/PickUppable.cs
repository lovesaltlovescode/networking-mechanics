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

    //Different states of the object
    public enum ObjectState
    {
        PickUppable,
        Droppable,
        Servable, //for final dishes
        Washable //for dirty dishes
    }

    public ObjectState objectState;


    // Start is called before the first frame update
    void Start()
    {
        objectState = ObjectState.PickUppable; //all objects start as pickuppables

    }

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
        //pickedUpObject.SetActive(false);
        //parent to player object
        pickedUpObject.transform.parent = playerPrefab.transform;
        //position the pickedUpObject behind the player
        pickedUpObject.tag = "FollowPlayer";

        //Set state as droppable since this object is now in the player's inventory
        objectState = ObjectState.Droppable;

        //TODO: Change layer so that it is masked and not detected by other players

    }

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

        //Unparent object and change tag back to the original tag
        pickedUpObject.transform.parent = null; //no more parent
        pickedUpObject.tag = FollowObject.originalTag;

        //Change state back to pickuppable
        objectState = ObjectState.PickUppable; 

        //TODO: Change layer so it can be detected by other players
    }

    // Update is called once per frame
    void Update()
    {


    }



}
