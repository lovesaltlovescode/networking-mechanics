using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// If player is holding a dirty plate and has entered sink zone, change state
/// handle changing of player states
/// </summary>

public class NetworkedWashInteraction : NetworkBehaviour
{
    //public GameObject objectContainerPrefab;

    [Header("Sink Positions")]
    private Transform[] sinkPositions; //array of sink positions STATIC
    private Transform[] cleanPlateSpawnPositions; //array of possible spawn positions for the clean plates STATIC
    public GameObject sinkParentZone; //parent sink zone, where the positions array will be retrieved from STATIC

    public GameObject[] platesInSink = new GameObject[2]; //array for plates in sink, null by default
    public GameObject[] cleanPlatesOnTable = new GameObject[2]; //array for the clean plates spawned on the table

    public bool holdingDirtyPlate; //check if player is holding a dirty plate

    [Header("Plate states")]
    public bool placedPlateInSink; //if plate has been placed in sink
    public bool startTimer; //start washing the plate
    public bool stoppedWashingPlate; //when player exits sink zone

    public bool showWashIcon; //bool to check if wash icon should be shown (UI)

    [Header("Plate counts")]
    public static int platesInSinkCount; //number of plates in the sink STATIC
    public int cleanPlatesCount; //number of clean plates on table

    //when the helditem changes, call onchangeingredient method
    [SyncVar(hook = nameof(OnChangeIngredient))]
    public HeldItem heldItem;

    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;
    [SerializeField] private NetworkedIngredientInteraction networkedIngredientInteraction;

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
        networkedIngredientInteraction = GetComponent<NetworkedIngredientInteraction>();
    }

    #region SyncVar

    void OnChangeIngredient(HeldItem oldItem, HeldItem newItem)
    {
        //Debug.Log("NetworkedIngredientInteraction - Starting coroutine!");
        StartCoroutine(ChangeIngredient(newItem));
    }

    IEnumerator ChangeIngredient(HeldItem newIngredient)
    {
        //If the player is holding something
        while (networkedPlayerInteraction.attachmentPoint.transform.childCount > 0)
        {
            //if player is holding nothing, destroy the existing child
            if (newIngredient == HeldItem.nothing)
            {
                Debug.Log("NetworkedWashInteraction - Destroying held object");
                Destroy(networkedPlayerInteraction.attachmentPoint.transform.GetChild(0).gameObject);
            }
            //if player is holding something, do nothing
            //Debug.Log("NetworkedIngredient - Inventory is full!");
            yield return null;
        }
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        //Check if player is holding a dirty plate
        if (networkedPlayerInteraction.IsInventoryFull() &&
            networkedPlayerInteraction.attachmentPoint.transform.GetChild(0).tag == "DirtyPlate")
        {
            Debug.Log("NetworkedWashInteraction - Player is holding a dirty plate!");
            holdingDirtyPlate = true;
        }
        else
        {
            holdingDirtyPlate = false;
        }

    }

    #region RemoteMethods

    public void PlacePlateInSink()
    {
        CmdPlacePlateInSink();

        //change state to can wash
        networkedPlayerInteraction.playerState = PlayerState.CanWashPlate;
    }

    //change to a command?
    public void WashPlate()
    {
        if (cleanPlatesCount == cleanPlateSpawnPositions.Length)
        {
            Debug.Log("NetworkedWashInteraction - Too many clean plates");
            return;
        }

        //set bool to start the timer on UI manager
        startTimer = true;

        //set stoppedwashing plate false
        stoppedWashingPlate = false;

        //set state to washing plate
        networkedPlayerInteraction.playerState = PlayerState.WashingPlate;
    }

    public void FinishWashingPlate()
    {
        CmdFinishWashingPlate();
        Debug.Log("NetworkedWashInteraction - Finished washing plate!");
    }

    #endregion

    #region Commands

    //Send command to server 
    [Command]
    public void CmdPlacePlateInSink()
    {
        //loop through plate in sink array
        //if the gameobject is null, assign heldplate to it
        for (int i = 0; i < platesInSink.Length; i++)
        {
            if (platesInSink[i] == null)
            {
                var sinkPos = sinkPositions[i].position;
                var sinkRot = sinkPositions[i].rotation;

                //increase count of plates
                platesInSinkCount += 1;
                Debug.Log("WashInteraction - One more plate in the sink!");
                Debug.Log("NetworkedWashInteraction - Number of plates in sink: " + platesInSinkCount);

                //Generic functions
                GameObject dirtyPlateInSink = Instantiate(networkedPlayerInteraction.objectContainerPrefab, sinkPos, sinkRot);

                //Set the plate in sink to be the spawned object
                platesInSink[i] = dirtyPlateInSink;
                Debug.Log("NetworkedWashInteraction - Dirty plate in sink: " + dirtyPlateInSink);
                Debug.Log("NetworkedWashInteraction - Dirty plates in sink: " + platesInSink[i]);

                dirtyPlateInSink.GetComponent<Rigidbody>().isKinematic = false;

                //get sceneobject script from the sceneobject prefab
                ObjectContainer dirtyPlateContainer = dirtyPlateInSink.GetComponent<ObjectContainer>();

                //instantiate the right ingredient as a child of the object
                dirtyPlateContainer.SetHeldItem(HeldItem.dirtyplate);

                ////sync var the helditem in scene object to the helditem in the player
                networkedIngredientInteraction.heldItem = heldItem;

                //set player's sync var to nothing so clients won't see the ingredient anymore
                heldItem = HeldItem.nothing;

                //spawn the scene object on network for everyone to see
                NetworkServer.Spawn(dirtyPlateInSink);

                //set layer to uninteractable
                dirtyPlateInSink.layer = LayerMask.NameToLayer("UnInteractable");


                //clear the inventory after placing in sink
                networkedPlayerInteraction.objectsInInventory.Clear();

                placedPlateInSink = true; //player has placed plate in sink
                holdingDirtyPlate = false;

                //change state to can wash
                showWashIcon = true;

                return;
            }
        }

    }


    //when done washing plate, reset timer and spawn clean plate
    [Command]
    public void CmdFinishWashingPlate()
    {
        //if starttimer is true
        if (startTimer)
        {
            //LOOP THROUGH PLATES IN THE SINK
            for (int i = platesInSink.Length - 1; i >= 0; i--)
            {
                if (platesInSink[i] != null)
                {
                    Debug.Log("NetworkedWashInteraction - Plate:" + i);
                    //destroy dirty plates in the sink
                    Debug.Log("NetworkedWashInteraction - Plate in sink: " + platesInSink[i]);
                    NetworkServer.Destroy(platesInSink[i].gameObject);
                    platesInSink[i] = null;
                    //reduce number of plates in sink
                    platesInSinkCount -= 1;

                    //LOOP THROUGH CLEAN PLATES ON TABLE
                    for (int x = 0; x < cleanPlatesOnTable.Length; x++)
                    {
                        //IF NO CLEAN PLATE
                        if (cleanPlatesOnTable[x] == null)
                        {
                            var platePos = cleanPlateSpawnPositions[x].position;

                            //Instantiate container at the spawn pos
                            GameObject cleanPlateOnTray = Instantiate(networkedPlayerInteraction.objectContainerPrefab, platePos, Quaternion.identity);

                            //set the cleanplate gameobject to be the plate on the tray
                            cleanPlatesOnTable[x] = cleanPlateOnTray;

                            //Set rigidbody as non-kinematic
                            cleanPlateOnTray.GetComponent<Rigidbody>().isKinematic = false;

                            //get script from the prefab
                            ObjectContainer objectContainer = cleanPlateOnTray.GetComponent<ObjectContainer>();

                            //Instantiate the right ingredient as a child of the object
                            objectContainer.SetHeldItem(HeldItem.cleanplate);

                            //set player's sync var to nothing so clients won't see the ingredient anymore
                            heldItem = HeldItem.nothing;

                            //spawn the scene object on network for everyone to see
                            NetworkServer.Spawn(cleanPlateOnTray);

                            //set layer to uninteractable
                            cleanPlateOnTray.layer = LayerMask.NameToLayer("UnInteractable");

                            //set starttimer to false
                            startTimer = false;

                            //if there are still plates in the sink
                            if (platesInSinkCount != 0)
                            {
                                showWashIcon = true;
                                placedPlateInSink = true;

                                //allow player to wash plate
                                networkedPlayerInteraction.playerState = PlayerState.CanWashPlate;
                                return;
                            }
                            else
                            {
                                //unable to wash anymore
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

    #endregion

    #region Triggers

    //if enter sink zone and is holding dirty plate
    private void OnTriggerEnter(Collider other)
    {
        //if player is in sink zone
        if (other.tag == "SinkZone")
        {
            Debug.Log("NetworkedWashInteraction - Entered sink zone!");
            sinkParentZone = other.gameObject; //assign the sink zone as the hit object

            //Get the cleanplate and sink positions from the SinkZones script
            cleanPlateSpawnPositions = sinkParentZone.GetComponent<SinkZones>().cleanPlateSpawnPositions;
            sinkPositions = sinkParentZone.GetComponent<SinkZones>().sinkPositions;

            //If there is a plate in the sink and players are not holding anything
            if(platesInSinkCount >= 1 && !networkedPlayerInteraction.IsInventoryFull())
            {
                networkedPlayerInteraction.playerState = PlayerState.CanWashPlate;
            }

            //if player is holding a plate
            if (holdingDirtyPlate)
            {
                //player can place plate in the sink
                Debug.Log("NetworkedWashInteraction - Player can place a plate in the sink!");
                networkedPlayerInteraction.playerState = PlayerState.CanPlacePlateInSink;
            }

            //if player was washing plate, if they enter the sink zone again they can immediately wash
            //or if there are still plates in the sink
            else if (stoppedWashingPlate || platesInSinkCount != 0)
            {
                networkedPlayerInteraction.playerState = PlayerState.CanWashPlate;

                showWashIcon = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if exit sink zone
        if (other.tag == "SinkZone")
        {
            Debug.Log("NetworkedWashInteraction - Player has exited the sink! Disable wash icon");

            showWashIcon = false; //hide wash icon

            if (holdingDirtyPlate || platesInSinkCount != 0)
            {
                //player exited sink
                networkedPlayerInteraction.playerState = PlayerState.ExitedSink;
                startTimer = false;
            }

            //if player was washing a dirty plate
            if (networkedPlayerInteraction.playerState == PlayerState.WashingPlate)
            {
                //change state
                networkedPlayerInteraction.playerState = PlayerState.StoppedWashingPlate;

                //set bool true
                stoppedWashingPlate = true;
                startTimer = false;
            }
        }
    }



    #endregion
}
