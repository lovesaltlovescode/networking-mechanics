using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

#region Summary

//Checks for players in lobby, and allows them to set their ready state
//Lobby UI shows for host, clients will join via this UI
//Clients and host can set their ready state
//When ready, host can start the game
//Clients can leave the room
//Host can destroy the room
#endregion
public class NetworkRoom : NetworkBehaviour
{

    //VARIABLES
    [Header("UI")]

    [SerializeField] private GameObject lobbyUI = null; //Only enable for host, so there's not one for every single player
    [SerializeField] private TMP_Text[] playerWaitTexts = new TMP_Text[5]; //Array to contain placeholder text when waiting for players
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[5]; //Array to contain player ready states
    [SerializeField] private Image[] playerImages = new Image[5]; //Images for players


    [SerializeField] private Button leaveRoomButton = null; //Disable if host, clients can leave room
    [SerializeField] private Button deleteRoomButton = null; //Enable for host, deletes whole room
    [SerializeField] private Button startGameButton = null; //Only enable for host, so they can decide when to start the game

    //SYNCED VARIABLES (Hooks)

    //Sync player names, loading on default, and when the players join, replace with their name
    //when value is changed on server, update the UI accordingly
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string displayName = "Loading...";

    //Sync ready state among all players
    //when value is changed on server, update UI accordingly
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool isReady = false;


    //Check if player is the host, and the lobby leader
    [SerializeField] private bool isLeader = false;


    //Property for IsLeader, accessible from outside script
    public bool IsLeader
    {
        set
        {
            isLeader = value; //True if isleader is true

            //toggle buttons on/off based on if player is leader
            startGameButton.gameObject.SetActive(value);

            //set start game button uninteractable
            startGameButton.interactable = false;

            deleteRoomButton.gameObject.SetActive(value);
            leaveRoomButton.gameObject.SetActive(!value); //set inactive if leader

        }
    }


    public static CustomNetworkManager networkRoomManager; //Network manager object

    //Property
    private CustomNetworkManager NetworkRoomManager
    {
        get
        {
            if (networkRoomManager != null)
            {
                return networkRoomManager; //If there is a network room manager, then return that object
            }

            //If its null, then just get it
            return networkRoomManager = NetworkManager.singleton as CustomNetworkManager;
            //Cast network manager as a singleton to get our custom network manager
        }

    }

    //Called on client with authority, before on start client
    public override void OnStartAuthority()
    {
        //Set player name via command
        //CmdSetDisplayName(PlayerNameInput.playerName); //Reference static string player name from player name input script

        lobbyUI.SetActive(true);
    }


    //Called on every network behaviour on client (when script is active)
    public override void OnStartClient()
    {
        NetworkRoomManager.RoomPlayers.Add(this); //add this instance of the script to the list of room players

        UpdateDisplay(); //Update UI
    }

    //Invoked on clients, so if someone disconnects
    //remove them from the list
    //used to invoke effects when a client is destroyed/ do specific client cleanup
    public override void OnNetworkDestroy()
    {
        NetworkRoomManager.RoomPlayers.Remove(this); //remove this instance of the script from list of room players
    }

    //Hooks for the synced name and ready variables, update UI accordingly (lambda)

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();


    public void Start()
    {
        StartCoroutine(UpdateDisplay(2.0f)); //Update display every 2 seconds
    }

    IEnumerator UpdateDisplay(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime); //run every ___ seconds
            UpdateDisplay();
            //Debug.Log("Coroutine running: " + Time.time);
        }
    }



    //Update UI according to changed name and ready status

    public void UpdateDisplay()
    {


        //If this object does not belong to us
        if (!hasAuthority)
        {
            //Loop through and find the one that belongs to us
            foreach (var player in NetworkRoomManager.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    //Update display for that player
                    player.UpdateDisplay();

                    break; //break after updating
                }
            }

            return;
        }

        //Update UI
        for (int i = 0; i < playerReadyTexts.Length; i++)
        {
            //Loop through all player name and readystatus and set as empty/loading, cleared text for a new round
            playerWaitTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
            playerImages[i].GetComponent<Image>().enabled = false;
        }

        for (int i = 0; i < NetworkRoomManager.RoomPlayers.Count; i++)
        {
            //Change placeholder texts to empty now that players have joined
            playerWaitTexts[i].text = string.Empty;

            //Change image colours to white now that players have joined
            playerImages[i].GetComponent<Image>().enabled = true;



            //if player set as ready, change text to green, else change to red
            if (NetworkRoomManager.RoomPlayers[i].isReady)
            {
                //Green ready colour
                float r = 0.02f, g = 0.45f, b = 0.041f, a = 1f;
                playerReadyTexts[i].text = "Ready";
                playerReadyTexts[i].color = Color.white;
            }
            else
            {
                //Red not ready colour
                float r = 0.6f, g = 0.02f, b = 0.02f, a = 1f;
                playerReadyTexts[i].text = "Not Ready";
                playerReadyTexts[i].color = Color.red;
            }

        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader)
        {
            //if not leader, dont do anything
            //doesnt matter to us
            return;
        }

        //if leader and ready to start, the game button will be interactable
        startGameButton.interactable = readyToStart;
    }

    [ClientRpc]
    //From server to client, run on client
    //When leave room button clicked
    //TODO: When host clicks it, popup appears warning them the room will be destroyed
    public void RpcDestroyRoom()
    {
        if (!isLeader)
        {
            Debug.Log("Not leader, do not do anything");
            return;

        }

        //clients all have to leave
        NetworkRoomManager.StopHost();
        //NetworkRoomManager.StopClient();
        //Debug.Log("Stop host");
        NetworkRoomManager.RoomPlayers.Clear(); //Clear out existing room players
        NetworkRoomManager.RoomPlayers.Remove(this);
        //Reset existing code to 0, 
        //since 0 cannot be used to connect, players cannot connect to the room


    }

    //from client to server, run on server

    public void LeaveRoom()
    {
        //if leader, not supposed to be runnning this
        if (isLeader)
        {
            Debug.Log("Is leader, do not do anything");
            return; //do nothing
        }


        NetworkRoomManager.RoomPlayers.Remove(this); //remove this player from list of players
        NetworkRoomManager.StopClient(); //stop client, runs onstopclient
        Debug.Log("Not room leader, exiting room");
        NetworkRoomManager.RoomPlayers.Clear();
    }


    //COMMANDS: Sent from clients to server ie. initiate some action
    //RPC: Sent from server to clients ie. tell all clients to do something as a result of the action, the clients will do this locally on their own copy

    [Command]
    //Called from client on server, server will set player ready
    //Due to sync var hook, when ready state changes on server, it is propagated to all clients, and updates accordingly
    //Triggered when pressing the ready button
    public void CmdReadyUp()
    {
        isReady = !isReady; //toggle ready status on/off
        NetworkRoomManager.NotifyPlayersOfReadyState();
    }


    [Command]
    //Called from client on server, server will start game
    //Triggered when pressing the start button
    public void CmdStartGame()
    {
        //Make sure this player is the leader
        //Make sure the connection (netID) of this player matches the leader's (first player)
        if (NetworkRoomManager.RoomPlayers[0].connectionToClient != connectionToClient)
        {
            return;
        }

        //TODO: start game

        Debug.Log("Ready to start game");

        NetworkRoomManager.StartGame();
    }


}
