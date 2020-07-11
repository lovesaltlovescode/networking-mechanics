using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class for pickuppable object
//Will be on every pickuppable object

//Also handles the spawning of ingredients for the ingredient shelf
//Only applies for ingredient shelf layer objects
public class PickUppable : MonoBehaviour, I_Interactable
{

    #region Global Variables

    //VARIABLES THAT ALL OBJECTS WILL HAVE
    [Header("Global Variables")]

    //Object follow script
    FollowObject followScript;

    //object icon to set active/inactive on the button
    public Image objectIcon;

    //public GameObject ingredientObject; //to assign the ingredient prefab to this

    //the object that the player will be holding if they pick up the object
    //ie. a cup of rice on their head
    //set active/inactive depending on if the player is holding the object
    public GameObject heldObject;

    //List of objects in player's inventory, static and same throughout all scripts
    public static List<GameObject> objectsInInventory = new List<GameObject>();

    //reference to the player prefab to get its transform
    public GameObject playerPrefab;

    #endregion


    #region DirtyPlates Variables
    [Header("Dirty Plates Variables")]

    //VARIABLES THAT ONLY DIRTYPLATES WILL HAVE

    //wash icon to set active and gray
    public Image washIcon;

    //washing icon that fills according to timer
    public Image washingIcon;

    //bool to check if plate was being washed
    public bool wasWashing; //set true if player was interrupted from washing

    //Gameobject for clean plate to be set active/inactive
    public GameObject cleanPlate;



    //reference to sink position
    public Transform sinkPos;

    #endregion

    //#region IngredientShelf Variables

    //[Header("Ingredient Shelf Variables")]

    ////INGREDIENT SHELF THAT SPAWNS INGREDIENTS

    //public GameObject ingredientPrefab; //ingredient to spawn according to the ingredient shelf

    //#endregion

    #region Ingredient Variables

    [Header("Ingredient Variables")]

    //INGREDIENTS THAT ARE SPAWNED FROM SHELF

    public Transform trayPos; //Position of ingredient tray
    //TODO: Change to an array of positions, loop through, check against a bool and place down if bool is false...

    #endregion

    //Different states of the object
    public enum ObjectState
    {
        Spawnable, //for ingredient shelf

        PickUppable,
        Droppable,

        PlaceOnIngredientTable, //for the ingredients to be placed on the table
        Servable, //for final dishes to be served to customers

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
        if(gameObject.layer == 14)
        {
            objectState = ObjectState.Spawnable; //if ingredient shelf, start as spawnable
        }
        else
        {

            objectState = ObjectState.PickUppable; //start as pickuppable
        }

        //followScript = gameObject.GetComponent<FollowObject>(); //Reference follow object script

        heldObject.GetComponentInChildren<Renderer>().enabled = false;

        if(gameObject.layer == 15)
        {
            //if ingredient, start as droppable
            objectState = ObjectState.Droppable;
        }
        

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

    //#region Spawn Object
    ////Function to spawn ingredient prefab as pickedup object
    ////pickedup object will then function as normal
    //public void SpawnIngredient()
    //{

    //    ingredientObject = Instantiate(ingredientPrefab, transform.position, Quaternion.identity);
    //    ingredientObject.transform.parent = playerPrefab.transform;

    //}
    //#endregion

    #region PickUp Object

    //Function to pick up object, does not check for condition
    //Add pickuppable object to inventory
    //Sets icon active and held object active
    //parents pickedupobject to player
    public void PickUpObject()
    {
        //Add to inventory
        objectsInInventory.Add(gameObject);

        //Activate the required icon
        objectIcon.gameObject.SetActive(true);

        //Print statement
        Debug.Log("PickUppable: Picked up " + gameObject);

        //Set object on Player's head active
        heldObject.GetComponentInChildren<Renderer>().enabled = true;

        //if ingredient shelf do not disable it
        if(gameObject.layer != 14)
        {
            DisableRenderer();
            followScript.enabled = true;
        }

        //parent to player object
        transform.parent = playerPrefab.transform;        

        //Set state as droppable since this object is now in the player's inventory
        objectState = ObjectState.Droppable;

        //TODO: Check if it can be detected by other players even when inactive
        // If so, Change layer so that it is masked and not detected by other players
        gameObject.layer = LayerMask.NameToLayer("PickedUp");

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
        objectsInInventory.Remove(gameObject);

        //Deactivate icon
        objectIcon.gameObject.SetActive(false);

        //Print statement
        Debug.Log("PickUppable: Dropped " + gameObject);

        //Deactivate heldobject
        heldObject.GetComponentInChildren<Renderer>().enabled = false;

        //Unparent object 
        transform.parent = null; //no more parent

        EnableRenderer();

        if (followScript)
        {
            followScript.enabled = false;
        }
        


        //Change state back to pickuppable
        objectState = ObjectState.PickUppable;

        //TODO: Change layer so it can be detected by other players
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    #endregion

    #region Place Ingredient On Table

    //function to place ingredient on table
    public void PlaceOnTable()
    {
        //Remove item from inventory
        objectsInInventory.Remove(gameObject);

        //set object icon inactive
        objectIcon.gameObject.SetActive(false);

        //set held item inactive
        heldObject.GetComponentInChildren<Renderer>().enabled = false;

        //set pickedup object active and move to tablepos
        //unparent from table
        EnableRenderer();
        gameObject.transform.position = trayPos.position;
        gameObject.transform.parent = null;

        followScript.enabled = false;
        

        //set layer mask to ingredientontable
        //pickedUpObject.layer = LayerMask.NameToLayer("ObjectOnTable");

        //set state as uninteractable
        objectState = ObjectState.UnInteractable;

    }

    #endregion 

    #region Wash Object

    //Function to place object in sink
    public void PlaceInSink()
    {

        //Remove item from inventory
        objectsInInventory.Remove(gameObject);

        //Set object icon inactive
        objectIcon.gameObject.SetActive(false);

        //Set held object inactive
        heldObject.GetComponentInChildren<Renderer>().enabled = false;

        //Print statement
        Debug.Log("PickUppable: Ready to wash " + gameObject);

        //Move pickedup object to the sink's position
        //if the position is null, that means this object shouldnt be washing, throw an error
        //Unparent from player
        EnableRenderer();
        gameObject.transform.parent = null;
        gameObject.transform.position = sinkPos.position;

        followScript.enabled = false;

        //set wash icon active
        washIcon.gameObject.SetActive(true);

        //Set state as washable
        objectState = ObjectState.Washable;
        gameObject.layer = LayerMask.NameToLayer("PlateOnSink");


    }

    //Function to wash object
    //change wash icon to gray
    public void WashObject() => washIcon.color = Color.gray;

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
        if (objectState == ObjectState.Washed)
        {
            //Set pickedupobject in sink inactive
            DisableRenderer();

            //Set clean plate active
            cleanPlate.SetActive(true);

            //after washing, destroy the pickedup object
            Destroy(gameObject);

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

        if(objectState == ObjectState.Washable)
        {

            //change collider size
            GetComponent<BoxCollider>().size = new Vector3(2.140998f, 1.3f, 1f);

        }

        if (gameObject.tag == "DirtyPlate")
        {

            CheckForStopWashing();

            if (gameObject != null)
            {
                //if picked up object is at the sink
                if (gameObject.transform.position == sinkPos.transform.position)
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

        if(gameObject != null)
        {

            gameObject.transform.rotation = Quaternion.identity;
        }


    }



}