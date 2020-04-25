using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

#region Summary

//Handle network management, using our own UI
//Inherit from network manager
//Responsible for checking connection status of clients and host

#endregion

public class NetworkManagerCustom : NetworkManager
{
    //private static NetworkManagerCustom networkManager;

    //VARIABLES

    [SerializeField] private int minPlayers = 3; //Minimum number of players needed to start the game


    //Scene name
    [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoom roomPlayerPrefab = null;

    //Create static public events to be accessed from main menu
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    //Create and store a list of room players currently in the room
    public List<NetworkRoom> RoomPlayers { get; } = new List<NetworkRoom>();

    //public void Update()
    //{

    //    //TODO: Figure out why menu scene gives directory instead
    //    Debug.Log("Current Scene: " + menuScene);

    //    Debug.Log(
    //$"Scene name: {SceneManager.GetActiveScene().path}, menuScene name: {menuScene}");

    //    if (SceneManager.GetActiveScene().path == menuScene)
    //    {
    //        Debug.Log("Currently in menu scene");
    //    }
    //}

    //public override void Awake()
    //{
    //    DontDestroyOnLoad(this);

    //    if (networkManager == null)
    //    {
    //        networkManager = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //On start server, register spawnable prefabs
    //less cumbersome than dragging and registering under inspector
    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    }

    //On start client, loop through spawnable prefabs
    //register prefabs 
    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        foreach(var prefab in spawnablePrefabs)
        {
            //Register each prefab with spawning system
            ClientScene.RegisterPrefab(prefab);
        }
    }


    //Called on client when connected to server
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn); //Pass in connection to server, and set client as ready, then connect to server

        OnClientConnected(); //Raise event, carry out whatever actions are needed
    }


    //Called on the client when it disconnects from the server
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        //if client disconnected, invoke event
        OnClientDisconnected(); //raise event, cary out whatever actions are needed
    }

    //Called on the server whenever a client connects to server
    public override void OnServerConnect(NetworkConnection conn)
    {
        //If there are too many players,  no new connections can be made
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();

            Debug.LogError("NetworkManagerCustom: No more players allowed to join");
            return;

        }

        //If already in the game scene, or out of menu scene, do not allow for new connections
        if (SceneManager.GetActiveScene().name != menuScene)
        {
            Debug.Log("NetworkManagerCustom: In game. No more players allowed to join");
        }
    }

    //Called on server when a client disconnects from server
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //Get room player script and remove from the list

        if(conn.identity != null)  //if the object has a network identity
        {
            var player = conn.identity.GetComponent<NetworkRoom>(); //Get network room script of disconnected player

            RoomPlayers.Remove(player); //Remove the player conn from the list of room players

            NotifyPlayersOfReadyState(); //Update everyone of ready status
        }

        base.OnServerDisconnect(conn); //Destroys player for connection, disconnects client properly
    }

    //When server/host is stopped
    //Perhaps when a delete room button is clicked? Which will result in kicking every one out of the room
    public override void OnStopServer()
    {
        RoomPlayers.Clear(); //Clear the list of existing players, ready for a new game
    }

    public void NotifyPlayersOfReadyState()
    {
        //Loop through the list of room players, and check if they are ready to start
        foreach (var player in RoomPlayers)
        {   
            //function in networkroom script
            player.HandleReadyToStart(IsReadyToStart()); 
        }
    }


    //Check if players are ready to start the game
    //Linked to network room script
    private bool IsReadyToStart()
    {
        if(numPlayers < minPlayers)
        {
            //Too little players, do not start
            return false;
        }

        foreach(var player in RoomPlayers)
        {
            //Loop through list of players
            //if even 1 player is not ready, do not start
            if(player.isReady == false)
            {
                return false;
            }
        }

        //else, ready to start
        return true;
    }

    //Called on server when new client connects, player prefab instantiated
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //if in menu scene, spawn in room player prefab and add as client
        if(SceneManager.GetActiveScene().name == menuScene)
        {
            //If there were no room players prior, then this client is the leader
            bool isLeader = RoomPlayers.Count == 0;


            NetworkRoom roomPlayerInstance = Instantiate(roomPlayerPrefab); //Reference network room script, instantiate prefab

            roomPlayerInstance.IsLeader = isLeader; //if we are the leader, then set is leader property to true (show start game btn)

            //So server knows that game object represents us with this connection ID
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }


    //Start game (When start button is pressed by leader)
    public void StartGame()
    {
        //if in menu scene
        if(SceneManager.GetActiveScene().name == menuScene)
        {
            //if not ready to start, do nothing
            if (!IsReadyToStart())
            {
                return;
            }

            //If ready to start
            Debug.Log("Network Manager Custom: Starting game...");
        }
    }
}
