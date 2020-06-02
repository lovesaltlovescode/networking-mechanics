using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Check for tag of heldobject
/// If heldobject is dirty plate and has entered sink zone, change player state
/// Handle changing of player state according to if player is in sink zone or not
/// </summary>

public class SinkInteraction : MonoBehaviour
{
    public Transform sinkPosition; // Position where the dirty plates will be placed for washing
    public GameObject cleanPlatePrefab; //Prefab for clean plate to spawn
    public Transform cleanPlateSpawnPosition; //Position for clean plate to spawn at

    public bool holdingDirtyPlate; //If player is holding a dirty plate, this will be true

    //Booleans to check which stage of washing player is at
    //public bool canWashPlate; //if plate is in the sink 
    //public bool wasWashingPlate; //if plate was being washed
    //public bool stoppedWashingPlate; //if player stopped washing the plate halfway (left trigger)
    //public bool finishedWashingPlate; //if player has finished washing plates

    public bool placedPlateInSink; //if plate has been placed in sink
    public bool startTimer; //start washing the plate
    public bool stoppedWashingPlate;

    public bool showWashIcon; //bool to check if wash icon should be shown

    public GameObject dirtyPlateInSink; //dirty plate being held

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("SinkInteraction - Script Initialised");

    }

    //Function to place plate in sink
    //ONLY IF CAN PLACE PLATE IN SINK STATE
    public void PlacePlateInSink(GameObject heldPlate, List<GameObject> Inventory)
    {
        heldPlate.transform.position = sinkPosition.position;
        
        //Set layer to uninteractable
        heldPlate.layer = LayerMask.NameToLayer("UnInteractable");

        //Remove detected object
        PlayerInteractionManager.detectedObject = null;

        Debug.Log("Sinkinteraction - Placing plate in sink");

        //unparent
        heldPlate.transform.parent = null;

        //Remove from inventory
        Inventory.Remove(heldPlate);

        //Set rotation back to 0
        heldPlate.transform.rotation = Quaternion.identity;

        //set held object to null, player is not holding anything
        PlayerInteractionManager.heldObject = null;

        placedPlateInSink = true; //player has placed plate in sink
        holdingDirtyPlate = false;
        PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanWashPlate;
        showWashIcon = true;
    }

    //Function to wash the plate
    //ONLY IF CAN WASH PLATE STATE
    public void WashDirtyPlate()
    {
        //set bool to start the timer on UI manager
        startTimer = true;

        //set stoppedwashingplate false
        stoppedWashingPlate = false;

        //Set state to is washing
        PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.WashingPlate;

        //On UI Manager, when start timer is true, wash icon turns gray and slowly fills up
        //Check every update for this, if it is not true, then the image fill amount remains at 0 always
        //once timer ends, change state to finished washing plate
    }

    //When finish washing plate, timer 0, spawn the prefab
    public void FinishWashingPlate()
    {
        if (startTimer)
        {

            //Destroy the dirty plate
            Destroy(dirtyPlateInSink);

            //Instantiate clean plate
            Instantiate(cleanPlatePrefab, cleanPlateSpawnPosition.position, Quaternion.identity);

            //set all bools to false
            startTimer = false;
            placedPlateInSink = false;
            showWashIcon = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInteractionManager.heldObject)
        {

            CheckForWashingCriteria();
            dirtyPlateInSink = PlayerInteractionManager.heldObject;

        }

    }

    //checks if player is able to wash by checking the tag of object
    //changes state of player accordingly
    public void CheckForWashingCriteria()
    {
        if(PlayerInteractionManager.heldObject.tag == "DirtyPlate")
        {
            Debug.Log("SinkInteraction - Player is holding a dirty plate!");
            holdingDirtyPlate = true;
        }
        else
        {
            holdingDirtyPlate = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //if it is the sink zone
        if(other.tag == "SinkZone")
        {
            Debug.Log("SinkInteraction - Player in sink zone!");

            //If player is holding a  plate
            if(holdingDirtyPlate)
            {
               Debug.Log("SinkInteraction - Player able to place plate in sink!");
               PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanPlacePlateInSink;
                
            }
            //if player was washing plate, then if they enter the sink zone again they can resume
            //TODO: CHANGE TO ELSE
            else if (stoppedWashingPlate)
            {
                Debug.Log("SinkInteraction - Player can resume washing plate!");

                //Change state to can wash plate
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanWashPlate;
                showWashIcon = true;
            }
            else
            {
                Debug.Log("SinkInteraction - Not holding plate, do nothing"); //delete later
            }
        }
    }


    public void OnTriggerExit(Collider other)
    {
        //if exit sink zone
        if(other.tag == "SinkZone")
        {
            Debug.Log("SinkInteraction - Exited sink zone");
            showWashIcon = false;

            //if holding a dirty plate
            if(holdingDirtyPlate)
            {
                //DELETE LATER
                Debug.Log("SinkInteraction - You should wash the plate!");
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.ExitedSink;
            }

            //if was washing a dirty plate
            if(PlayerInteractionManager.playerState == PlayerInteractionManager.PlayerState.WashingPlate)
            {
                //change state to stopped washing plate
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.StoppedWashingPlate;
                //set bool true
                stoppedWashingPlate = true;
                startTimer = false; //stop the timer
            }
        }
        else
        {
            Debug.Log("SinkInteraction - Not holding plate, do nothing"); //delete later
        }
    }
}
