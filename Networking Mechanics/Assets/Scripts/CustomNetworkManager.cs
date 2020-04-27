using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

#region Summary

//custom network manager, handles spawning of room player prefab
//registers spawnable prefabs

#endregion
public class CustomNetworkManager : NetworkManager
{

    [SerializeField] private int minPlayers = 3; //Minimum number of players needed to start the game


    //Scene name
    [Header("Scenes")]
    [Scene][SerializeField] private string menuScene = string.Empty;


    [Header("Room")]
    [SerializeField] private NetworkRoom roomPlayerPrefab = null;

    //Create static public events to be accessed from main menu
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    //Create and store a list of room players currently in the room
    public List<NetworkRoom> RoomPlayers { get; } = new List<NetworkRoom>();



    #region Network Management

    //Handles what happens when server/client have connected to the network
    //Spawns player prefab in a new scene

    //On start server, register all spawnable prefabs
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

        foreach (var prefab in spawnablePrefabs)
        {
            //Register each prefab with spawning system
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public void Update()
    {
        Debug.Log("Current room players are: " + RoomPlayers.Count);
    }

    //Called on CLIENT when connected to server
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn); //Pass in connection to server, and set client as ready, then connect to server

        //OnClientConnected(); //Raise event, carry out whatever actions are needed
    }


    //Called on the CLIENT when it disconnects from the server
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn); //Stops client

        //if client disconnected, invoke event
        //OnClientDisconnected(); //raise event, cary out whatever actions are needed
    }

    //Called on the SERVER whenever a client connects to server
    public override void OnServerConnect(NetworkConnection conn)
    {
        //If there are too many players,  no new connections can be made
        //max connections defined in the inspector, including the host
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect(); //disconnect that client automatically

            Debug.LogError("NetworkManagerCustom: No more players allowed to join");
            return;

        }

        //If already in the game scene (game has started), do not allow for new connections
        if (SceneManager.GetActiveScene().name != menuScene)
        {
            Debug.Log("NetworkManagerCustom: In game. No more players allowed to join");
        }
    }

    //Called on SERVER when a client disconnects from server, automatically
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //Get room player script and remove from the list

        if (conn.identity != null)  //if the object has a network identity
        {
            var player = conn.identity.GetComponent<NetworkRoom>(); //Get network room script of disconnected player

            RoomPlayers.Remove(player); //Remove the player conn from the list of room players

            NotifyPlayersOfReadyState(); //Update everyone of ready status
        }

        base.OnServerDisconnect(conn); //Destroys player for connection, disconnects client properly
        Debug.Log("Removed player");
    }

    //When server/host is stopped
    //Perhaps when a delete room button is clicked? Which will result in kicking every one out of the room
    public override void OnStopServer()
    {
        RoomPlayers.Clear(); //Clear the list of existing players, ready for a new game
    }

    public override void OnStopHost()
    {
        RoomPlayers.Clear();
        Debug.Log("Custom Network Manager: Stopped host");
        StopClient();
    }

    public override void OnStopClient()
    {
        Debug.Log("Custom Network Manager: Stopped client");
        SceneManager.LoadScene(0); //reload scene for client
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        //Loop through the list of room players, and check if they are ready to start
        foreach (var player in RoomPlayers)
        {
            //function in networkroom script
            //player.HandleReadyToStart(IsReadyToStart());
        }
    }

    //Check if players are ready to start the game
    //Linked to network room script
    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers)
        {
            //Too little players, do not start
            return false;
        }

        foreach (var player in RoomPlayers)
        {
            //Loop through list of players
            //if even 1 player is not ready, do not start
            //if (player.isReady == false)
            //{
            //    return false;
            //}
        }

        //else, ready to start
        return true;
    }

    //Called on SERVER when new client connects, player prefab instantiated
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //if in menu scene, spawn in room player prefab and add as client
        //TODO: Only spawn in lobby scene, maybe use a sprite as a temporary placeholder for the characters
        //TODO: Sprites of different assigned colours depending on order of joining, different designated spawn points
        //TODO: What happens when disconnect? Do they retain the same order
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            //If there were no room players prior, then this client is the leader
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoom roomPlayerInstance = Instantiate(roomPlayerPrefab); //Create an instance of network room instantiate room player prefab

            roomPlayerInstance.IsLeader = isLeader; //if we are the leader, then set is leader property to true (show start game btn)

            //So server knows that game object represents us with this connection ID
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }


    //Start game (When start button is pressed by leader)
    public void StartGame()
    {
        //if in menu scene
        //TODO: If in lobby scene
        if (SceneManager.GetActiveScene().name == menuScene)
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

    //Change scene when host/client joins, change to lobby scene
    //Spawn clients using a spawn system, according to the order they come in

    ////called on server when a scene is completed
    ////al clients will now have a spawn system and it is owned by the server
    //public override void OnServerSceneChanged(string sceneName)
    //{
    //    //if scene is one of the levels
    //    if (sceneName.StartsWith("Scene_Map"))
    //    {
    //        //spawn in the player's spawn system
    //        GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);

    //        NetworkServer.Spawn(playerSpawnSystemInstance); //if you do not pass in conn, it means the server owns it
    //    }
    //}

    #endregion
}
