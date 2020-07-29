using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// check for tag of heldobject
/// if heldobject is a dirty plate and player has entered sink zone, change player state
/// handle changing of player state according to if player is in sink zone or not
/// </summary>
public class WashInteraction : MonoBehaviour
{
    [Header("Sink Positions")]
    [HideInInspector] public static GameObject sinkParentObj;

    public Transform[] cleanPlateSpawnPositions; //array of possible spawn positions for the clean plates

    [Header("Plate game objects")]
    public GameObject cleanPlatePrefab; //clean plate to spawn

    public GameObject[] platesInSink = new GameObject[2]; //array for plates in sink, null by default
    public GameObject[] cleanPlatesOnTable = new GameObject[2]; //array for the clean plates spawned on the table

    [Header("Plate states")]
    public static bool placedPlateInSink; //if plate has been placed in sink
    public bool startTimer; //start washing the plate
    public bool stoppedWashingPlate; //when player exits sink zone

    public bool holdingDirtyPlate; //check the tag of held object, if its a dirty plate this is true

    public bool showWashIcon; //bool to check if wash icon should be shown (UI)

    public static int platesInSinkCount; //number of plates in the sink

    public int cleanPlatesCount; //number of clean plates on table

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("WashInteraction - Script initialised");
    }

    //plate plate in sink 
    //only if PLACE PLATE IN SINK state
    public void PlacePlateInSink(GameObject heldPlate, List<GameObject> Inventory)
    {
        Transform[] sinkPositions = new Transform[2];

        if (sinkParentObj.GetComponent<SinkScript_Temp>() != null)
        {
            sinkPositions = sinkParentObj.GetComponent<SinkScript_Temp>().sinkPositions;
        } else
        {
            Debug.Log("SinkScript not present");
        }
        

        //loop through plate in sink array
        //if the gameobject is null, assign heldplate to it
        for (int i = 0; i < platesInSink.Length; i++)
        {
            if(platesInSink[i] == null)
            {
                platesInSink[i] = heldPlate;
                heldPlate.transform.position = sinkPositions[i].position;

                //increase count of plates
                platesInSinkCount += 1;
                Debug.Log("WashInteraction - One more plate in the sink!");

                //Generic functions

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

                //change state to can wash
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanWashPlate);
                showWashIcon = true;

                return;
            }
        }
        
    }

    //function to wash the plate
    //only if CAN WASH PLATE state
    public void WashDirtyPlate()
    {
        if(cleanPlatesCount == 2)
        {
            Debug.Log("WashInteraction - Too many clean plates");
            return;
        }

        //set bool to start the timer on UI manager
        startTimer = true;

        //set stoppedwashing plate false
        stoppedWashingPlate = false;

        //set state to washing plate
        PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.WashingPlate);
    }

    //when done washing plate, reset timer and spawn clean plate
    public void FinishWashingPlate()
    {
        //if starttimer is true, destroy the dirty plate in the sink (washed)
        if (startTimer)
        {
            for (int i = platesInSink.Length - 1; i >= 0; i--)
            {
                if (platesInSink[i] != null)
                {
                    Destroy(platesInSink[i].gameObject); 
                    platesInSink[i] = null;
                    //reduce number of plates in sink
                    platesInSinkCount -= 1;

                    for(int x = 0; x < cleanPlatesOnTable.Length; x++)
                    {
                        //if there is no clean plate in that position, instantiate one
                        if(cleanPlatesOnTable[x] == null)
                        {
                            var cleanPlate = Instantiate(cleanPlatePrefab, cleanPlateSpawnPositions[x].position, Quaternion.identity);
                            cleanPlatesOnTable[x] = cleanPlate;
                            cleanPlatesCount += 1;

                            //set all bools to false
                            startTimer = false;

                            //if there are still plates in the sink
                            if (platesInSinkCount != 0)
                            {
                                showWashIcon = true;
                                placedPlateInSink = true;

                                //continue washing plate
                                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanWashPlate);
                                return;
                            }
                            else
                            {
                                showWashIcon = false;
                                placedPlateInSink = false;
                            }
                            return;
                        }
                        
                    }
                }

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

    //checks if player is able to wash by checking tag of heldobject
    public void CheckForWashingCriteria()
    {
        if(PlayerInteractionManager.detectedObject.tag == "DirtyPlate")
        {
            Debug.Log("WashInteraction - Player is holding a dirty plate!");
            holdingDirtyPlate = true;
        }
        else
        {
            holdingDirtyPlate = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //if player is in sink zone
        if(other.tag == "SinkZone")
        {
            //if player is holding a plate
            if (holdingDirtyPlate)
            {
                sinkParentObj = other.gameObject;

                //player can place plate in the sink
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanPlacePlateInSink);

            }

            //if player was washing plate, if they enter the sink zone again they can immediately wash
            //or if there are still plates in the sink
            else if(stoppedWashingPlate || platesInSinkCount != 0)
            {
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanWashPlate);

                showWashIcon = true;
            }

            


        }
    }

    public void OnTriggerExit(Collider other)
    {
        //if exit sink zone
        if(other.tag == "SinkZone")
        {

            Debug.Log("WashInteraction - Player has exited the sink! Disable wash icon");

            showWashIcon = false; //hide wash icon

            if(holdingDirtyPlate || platesInSinkCount != 0)
            {
                //player exited sink
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.ExitedSink);
                startTimer = false;
            }

            //if player was washing a dirty plate
            if(PlayerInteractionManager.playerState == PlayerInteractionManager.PlayerState.WashingPlate)
            {
                //change state
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.StoppedWashingPlate);

                //set bool true
                stoppedWashingPlate = true;
                startTimer = false;
            }
        }
    }
}
