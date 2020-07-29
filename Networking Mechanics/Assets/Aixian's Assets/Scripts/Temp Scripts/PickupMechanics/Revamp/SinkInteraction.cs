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
    [Header("Sink positions")]
    public Transform sinkPosition; // Position where the dirty plates will be placed for washing
    public Transform sinkPosition2; //Position where the 2nd set of dirty plates will be placed for washing

    public Transform cleanPlateSpawnPosition; //Position for clean plate to spawn at

    [Header("Plate game objects")]
    public GameObject cleanPlatePrefab; //Prefab for clean plate to spawn

    public List<GameObject> platesBeingHeld; //dirty plate being held

    [Header("Plate states")]

    public static bool placedPlateInSink; //if plate has been placed in sink
    public bool startTimer; //start washing the plate
    public bool stoppedWashingPlate;

    public bool holdingDirtyPlate; //If player is holding a dirty plate, this will be true

    public bool showWashIcon; //bool to check if wash icon should be shown

    public int platesInSink; //number of plates in sink, maximum of 3

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("SinkInteraction - Script Initialised");

    }

    //Function to place plate in sink
    //ONLY IF CAN PLACE PLATE IN SINK STATE
    public void PlacePlateInSink(GameObject heldPlate, List<GameObject> Inventory)
    {
        //If more than 2 plates in sink, do not allow placement
        if (platesInSink == 2)
        {
            return;
        }

        //check how many plates there are in the sink
        switch (platesInSink)
        {
            case 0:
                heldPlate.transform.position = sinkPosition.position;
                break;
            case 1:
                heldPlate.transform.position = sinkPosition2.position;
                break;
        }

        platesBeingHeld.Add(heldPlate);

        #region Generic functions

        //Generic function regardless of positions
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

        placedPlateInSink = true; //player has placed plate in sink
        holdingDirtyPlate = false;
        PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanWashPlate;
        platesInSink += 1; //increase number of plates in the sink
        showWashIcon = true;
        #endregion



    }

    //check how many plates there in the sink

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
            //loop through all plates being held and destroy the first one
            //TODO: Restructure properly....
            if (platesInSink == 2)
            {
                Destroy(platesBeingHeld[1]);
                platesBeingHeld.Remove(platesBeingHeld[1]);
                //reduce number of plates in sink
                platesInSink -= 1;
            }
            else if (platesInSink == 1)
            {
                Destroy(platesBeingHeld[0]);
                platesBeingHeld.Remove(platesBeingHeld[0]);
                //reduce number of plates in sink
                platesInSink -= 1;

                if (!platesBeingHeld[0])
                {
                    Destroy(platesBeingHeld[1]);
                    platesBeingHeld.Remove(platesBeingHeld[1]);
                    //reduce number of plates in sink
                    platesInSink -= 1;
                }
            }


            //Instantiate clean plate
            Instantiate(cleanPlatePrefab, cleanPlateSpawnPosition.position, Quaternion.identity);

            //set all bools to false
            startTimer = false;


            if (platesInSink != 0)
            {
                showWashIcon = true;
                placedPlateInSink = true;
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanWashPlate;
            }
            else
            {
                showWashIcon = false;
                placedPlateInSink = false;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInteractionManager.detectedObject)
        {

            CheckForWashingCriteria();


        }

    }

    //checks if player is able to wash by checking the tag of object
    //changes state of player accordingly
    public void CheckForWashingCriteria()
    {
        if (PlayerInteractionManager.detectedObject.tag == "DirtyPlate")
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
        if (other.tag == "SinkZone" && PlayerInteractionManager.CanChangePlayerState())
        {
            Debug.Log("SinkInteraction - Player in sink zone!");

            //If player is holding a  plate
            if (holdingDirtyPlate)
            {
                Debug.Log("SinkInteraction - Player able to place plate in sink!");
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.CanPlacePlateInSink;

            }
            //if player was washing plate, then if they enter the sink zone again they can resume
            //TODO: CHANGE TO ELSE
            else if (stoppedWashingPlate || platesInSink != 0 && PlayerInteractionManager.CanChangePlayerState())
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
        if (other.tag == "SinkZone" && PlayerInteractionManager.CanChangePlayerState())
        {
            Debug.Log("SinkInteraction - Exited sink zone");
            showWashIcon = false;

            //if holding a dirty plate
            if (holdingDirtyPlate || platesInSink != 0)
            {
                //DELETE LATER
                Debug.Log("SinkInteraction - You should wash the plate!");
                PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.ExitedSink;
                startTimer = false; //stop the timer
            }

            //if was washing a dirty plate
            if (PlayerInteractionManager.playerState == PlayerInteractionManager.PlayerState.WashingPlate)
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

