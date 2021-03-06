﻿using System.Collections;
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

    public bool holdingDirtyPlate; //check if player is holding a dirty plate

    [Header("Plate states")]
    public bool placedPlateInSink; //if plate has been placed in sink
    public bool startTimer; //start washing the plate
    public bool stoppedWashingPlate; //when player exits sink zone

    public bool showWashIcon; //bool to check if wash icon should be shown (UI)
    public bool atSink; //bool to check if player is at the sink
    public bool canWash = false; //bool to check if player can wash immediately after placing down a plate

    [Header("Plate counts")]

    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if player is holding a dirty plate
        if (networkedPlayerInteraction.playerInventory &&
            networkedPlayerInteraction.playerInventory.tag == "DirtyPlate")
        {
            //Debug.Log("NetworkedWashInteraction - Player is holding a dirty plate!");
            holdingDirtyPlate = true;
        }
        else
        {
            holdingDirtyPlate = false;
        }

        //Debug.Log("Dirty plate count: " + GameManager.Instance.platesInSinkCount);
        //Debug.Log("Clean plate count: " + GameManager.Instance.cleanPlatesCount);

    }

    #region RemoteMethods

    public void PlacePlateInSink()
    {
        networkedPlayerInteraction.serverAnimationScript.StopGrabAnim();
        CmdPlacePlateInSink();

        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing);

        //change state to can wash
        if (canWash)
        {
            networkedPlayerInteraction.playerState = PlayerState.CanWashPlate;
        }
    }


    public void WashPlate()
    {
        networkedPlayerInteraction.serverAnimationScript.WashAnim();
        //set bool to start the timer on UI manager
        startTimer = true;

        //set stoppedwashing plate false
        stoppedWashingPlate = false;

        //set state to washing plate
        networkedPlayerInteraction.playerState = PlayerState.WashingPlate;
        CmdWashPlate();
    }

    [Command]
    public void CmdWashPlate()
    {
        RpcWashPlate();
    }

    [ClientRpc]
    public void RpcWashPlate()
    {
        networkedPlayerInteraction.audioSource.Stop();
        networkedPlayerInteraction.audioSource.PlayOneShot(networkedPlayerInteraction.washSFX);
    }

    public void FinishWashingPlate()
    {
        networkedPlayerInteraction.serverAnimationScript.StopWashAnim();
        CmdFinishWashingPlate(startTimer, networkedPlayerInteraction.playerState);
        //Debug.Log("NetworkedWashInteraction - Finished washing plate!");
    }

    #endregion

    #region Commands

    //Send command to server 
    [Command]
    public void CmdPlacePlateInSink()
    {
        //loop through plate in sink array
        //if the gameobject is null, assign heldplate to it
        for (int i = 0; i < GameManager.Instance.platesInSink.Length; i++)
        {
            if (GameManager.Instance.platesInSink[i] == null)
            {
                var sinkPos = GameManager.Instance.sinkPositions[i].position;
                var sinkRot = GameManager.Instance.sinkPositions[i].rotation;

                //increase count of plates
                //platesInSinkCount += 1;
                //Debug.Log("WashInteraction - One more plate in the sink!");
                //Debug.Log("NetworkedWashInteraction - Number of plates in sink: " + platesInSinkCount);

                //Generic functions
                GameObject dirtyPlateInSink = Instantiate(networkedPlayerInteraction.objectContainerPrefab, sinkPos, sinkRot);

                //Set the plate in sink to be the spawned object
                GameManager.Instance.platesInSink[i] = dirtyPlateInSink;
                //Debug.Log("NetworkedWashInteraction - Dirty plate in sink: " + dirtyPlateInSink);
                //Debug.Log("NetworkedWashInteraction - Dirty plates in sink: " + platesInSink[i]);

                dirtyPlateInSink.GetComponent<Rigidbody>().isKinematic = false;

                //get sceneobject script from the sceneobject prefab
                ObjectContainer dirtyPlateContainer = dirtyPlateInSink.GetComponent<ObjectContainer>();

                //instantiate the right ingredient as a child of the object
                dirtyPlateContainer.SetObjToSpawn(HeldItem.dirtyplate);

                ////sync var the helditem in scene object to the helditem in the player
                dirtyPlateContainer.objToSpawn = HeldItem.dirtyplate;

                //set player's sync var to nothing so clients won't see the ingredient anymore
                networkedPlayerInteraction.heldItem = HeldItem.nothing;


                NetworkServer.Spawn(dirtyPlateInSink);

                //clear the inventory after placing in sink
                networkedPlayerInteraction.playerInventory = null;
                //Debug.Log("CmdPlacePlateInSink called");

                RpcPlacePlateInSink(dirtyPlateInSink, i);
                return;
            }
        }
    }

    [ClientRpc]
    public void RpcPlacePlateInSink(GameObject dirtyPlateInSink, int i)
    {
        //Debug.Log("RpcPlacePlateInSink called");
        placedPlateInSink = true; //player has placed plate in sink
        holdingDirtyPlate = false;
        GameManager.Instance.platesInSink[i] = dirtyPlateInSink;

        //set layer to uninteractable
        dirtyPlateInSink.layer = LayerMask.NameToLayer("UnInteractable");

        //increase plate in sink count
        //GameManager.Instance.platesInSinkCount += 1;
    }


    //when done washing plate, reset timer and spawn clean plate
    [Command]
    public void CmdFinishWashingPlate(bool timer, PlayerState playerState)
    {
        //if starttimer is true
        if (timer)
        {
            //LOOP THROUGH PLATES IN THE SINK
            for (int i = GameManager.Instance.platesInSink.Length - 1; i >= 0; i--)
            {
                if (GameManager.Instance.platesInSink[i] != null)
                {
                    //Debug.Log("NetworkedWashInteraction - Plate:" + i);
                    //destroy dirty plates in the sink
                    //Debug.Log("NetworkedWashInteraction - Plate in sink: " + platesInSink[i]);
                    NetworkServer.Destroy(GameManager.Instance.platesInSink[i].gameObject);
                    //platesInSinkCount -= 1;
                    GameManager.Instance.platesInSink[i] = null;

                    //set starttimer to false
                    timer = false;
                    RpcFinishWashingPlate(timer, i, playerState);

                    return;
                }
            }
        }
    }

    [ClientRpc]
    public void RpcFinishWashingPlate(bool timer, int i, PlayerState playerState)
    {
        //Debug.Log("RPC Start");

        //show clean plate on client
        //get script from the prefab

        //assign starttimer as timer so that it will be false
        startTimer = timer;
        networkedPlayerInteraction.audioSource.Stop();

        playerState = PlayerState.FinishedWashingPlate;

        //reduce number of plates in sink
        GameManager.Instance.platesInSinkCount -= 1;
        //increase clean plates count
        GameManager.Instance.cleanPlatesCount += 1;

        GameManager.Instance.platesInSink[i] = null;

        //if there are still plates in the sink
        if (GameManager.Instance.platesInSinkCount != 0)
        {
            showWashIcon = true;
            placedPlateInSink = true;

            //allow player to wash plate
            networkedPlayerInteraction.ChangePlayerState(PlayerState.CanWashPlate);
            return;
        }
        else
        {
            //unable to wash anymore
            showWashIcon = false;
            placedPlateInSink = false;
        }

        //Debug.Log("RPC End");
        return;

    }
    #endregion

    #region Triggers

    //if enter sink zone and is holding dirty plate
    private void OnTriggerEnter(Collider other)
    {

        //if player is in sink zone
        if (other.tag == "SinkZone")
        {
            atSink = true;


            //Debug.Log("NetworkedWashInteraction - Entered sink zone!");

            //If there is a plate in the sink and players are not holding anything
            if (GameManager.Instance.platesInSinkCount >= 1 && !networkedPlayerInteraction.IsInventoryFull())
            {

                for (int i = 0; i < PlayerMovement.ActivePlayers.Count; i++)
                {
                    if (PlayerMovement.ActivePlayers[i].GetComponent<NetworkedWashInteraction>().atSink
                        && PlayerMovement.ActivePlayers[i].GetComponent<NetworkIdentity>().netId != gameObject.GetComponent<NetworkIdentity>().netId)
                    {
                        //Debug.LogError("Some player is at the sink!");
                        canWash = false;
                        showWashIcon = false;
                        return;
                    }
                }
                canWash = true;
                showWashIcon = true;
                networkedPlayerInteraction.ChangePlayerState(PlayerState.CanWashPlate);
            }

            //if player is holding a plate
            if (holdingDirtyPlate)
            {
                //player can place plate in the sink
                //Debug.Log("NetworkedWashInteraction - Player can place a plate in the sink!");
                networkedPlayerInteraction.ChangePlayerState(PlayerState.CanPlacePlateInSink, true);
            }

            //if player was washing plate, if they enter the sink zone again they can immediately wash
            //or if there are still plates in the sink
            else if (stoppedWashingPlate || GameManager.Instance.platesInSinkCount != 0)
            {
                networkedPlayerInteraction.ChangePlayerState(PlayerState.CanWashPlate);

                showWashIcon = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "SinkZone")
        {
            atSink = true;

            //If there is a plate in the sink and players are not holding anything
            if (GameManager.Instance.platesInSinkCount >= 1 && !networkedPlayerInteraction.IsInventoryFull()
                && networkedPlayerInteraction.playerState != PlayerState.WashingPlate && networkedPlayerInteraction.playerState != PlayerState.FinishedWashingPlate)
            {

                for (int i = 0; i < PlayerMovement.ActivePlayers.Count; i++)
                {
                    if (PlayerMovement.ActivePlayers[i].GetComponent<NetworkedWashInteraction>().atSink
                        && PlayerMovement.ActivePlayers[i].GetComponent<NetworkIdentity>().netId != gameObject.GetComponent<NetworkIdentity>().netId)
                    {
                       // Debug.LogError("Some player is at the sink!");
                        canWash = false;
                        showWashIcon = false;
                        return;
                    }
                }
                canWash = true;
                showWashIcon = true;
                networkedPlayerInteraction.ChangePlayerState(PlayerState.CanWashPlate);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if exit sink zone
        if (other.tag == "SinkZone")
        {
            networkedPlayerInteraction.serverAnimationScript.StopWashAnim();
            CmdStopWashingPrematurely();
            atSink = false;
            canWash = false;

            //Debug.Log("NetworkedWashInteraction - Player has exited the sink! Disable wash icon");

            showWashIcon = false; //hide wash icon

            if (holdingDirtyPlate || GameManager.Instance.platesInSinkCount != 0)
            {
                //player exited sink
                networkedPlayerInteraction.ChangePlayerState(PlayerState.ExitedSink);
                startTimer = false;
            }

            //if player was washing a dirty plate
            if (networkedPlayerInteraction.playerState == PlayerState.WashingPlate)
            {
                //change state
                networkedPlayerInteraction.ChangePlayerState(PlayerState.StoppedWashingPlate);

                //set bool true
                stoppedWashingPlate = true;
                startTimer = false;
            }
        }
    }

    [Command]
    public void CmdStopWashingPrematurely()
    {
        
        RpcStopWashingPrematurely();
    }

    [ClientRpc]
    public void RpcStopWashingPrematurely()
    {
        networkedPlayerInteraction.audioSource.Stop();
    }

    #endregion
}
