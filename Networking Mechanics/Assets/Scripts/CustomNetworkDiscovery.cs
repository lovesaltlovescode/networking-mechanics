using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror.Discovery;

#region Summary

//Network discovery HUD
//Combined with network manager
//Main menu + spawn player prefab script
//Main menu is the offline scene
//When host is started/client joins, change scene to the lobby scene and spawn their player prefabs
//Player prefab contains buttons
//Client joins via 6-digit code generated when host starts, no need to display which servers are active, but always be searching

#endregion

[RequireComponent(typeof(NetworkDiscovery))]
public class CustomNetworkDiscovery : NetworkManager
{

    #region Network Discovery Variables

    //list of discovered server's ip address and the response sent by server
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>(); 

    public NetworkDiscovery networkDiscovery;

    //text for discovered servers
    [SerializeField] private TextMeshProUGUI discoveredServerText = null;

    //text for discovered server list
    [SerializeField] private TextMeshProUGUI discoveredServerAddress = null;

    //Bool to check if displayed found servers
    [SerializeField] private bool stopSearching; //if true, stop searching for servers

    #endregion

    #region Network Manager Variables

    //Minimum number of players needed to start the game
    [SerializeField] private int minPlayers = 3; 

    //Scene name, menu scene
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoom roomPlayerPrefab = null;

    //Create and store a list of room players currently in the room
    public List<NetworkRoom> RoomPlayers { get; } = new List<NetworkRoom>();

    #endregion

    #region Editor

#if UNITY_EDITOR
    public override void OnValidate()
    {
        if (networkDiscovery == null)
        {
            networkDiscovery = GetComponent<NetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new UnityEngine.Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
        }
    }
#endif

    #endregion


    #region Network Discovery

    //Do not run if
    public override void Awake()
    {
        //if (NetworkManager.singleton == null)
        //    return; //if no network manager

        if (NetworkServer.active || NetworkClient.active)
            return; //if currently running server or client
    }

    public override void Start()
    {
        Debug.Log("CustomNetworkDiscovery: Script running");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update: Discovered Servers: " + discoveredServers.Count);

        if (Input.GetKeyDown(KeyCode.F))
        {
            FindServers();
        }
    }

    ////when start server button is clicked
    ////dedicated server, delete later
    //public void StartServerButton()
    //{

    //    discoveredServers.Clear();

    //    Debug.Log("Starting as server...");
    //    NetworkManager.singleton.StartServer();

    //    networkDiscovery.AdvertiseServer();

    //    Debug.Log("Dedicated server advertised");
    //}

    //When start host button is clicked
    public void StartHostButton()
    {
        discoveredServers.Clear(); //clear existing servers

        Debug.Log("Starting as host...");
        StartHost(); //start a new network manager host
        networkDiscovery.AdvertiseServer(); //advertises this new host in the list

        Debug.Log("Host server advertised");

        Instantiate(playerPrefab);
        //Go to lobby scene

        Debug.Log("Spawning empty player prefab");


    }


    //TODO: Always search for active servers (Update, coroutine) while in menu scene
    public void FindServers()
    {
        //Find available servers
        Debug.Log("Discovered Servers Before Clear: " + discoveredServers.Count);
        //discoveredServers.Clear(); //clear existing servers
        networkDiscovery.StartDiscovery(); //search for more servers
        Debug.Log("Searching for servers...");
        Debug.Log("Discovered Servers: " + discoveredServers.Count);

        if (stopSearching == true)
        {

            Debug.Log("Stop searching for servers...");
            return;
        }
        //DisplayServers(); this will be called separately, on button click
    }

    //display found servers in a list on button press, for debug purposes
    //will not be in final game, no need to display, only needed to find
    public void DisplayServers()
    {
        if (discoveredServers.Count != 0)
        {
            discoveredServerText.text = "Discovered Servers: " + discoveredServers.Count.ToString();
            Debug.Log("Discovered Servers: " + discoveredServers.Count.ToString());

            //for each server found in servers
            foreach (ServerResponse info in discoveredServers.Values)
            {
                Debug.Log("Found this server: " + info.EndPoint.Address.ToString());
                discoveredServerAddress.text = info.EndPoint.Address.ToString(); //IP Address of server found

                //TODO: AUtomatically connect to this ip address if the code input was correct
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Connecting to: " + info.EndPoint.Address.ToString());
                    Connect(info);
                    stopSearching = true; //Stop searching for active servers now that we are connected
                }
            }
        }

        //No servers found yet, not started host
        else
        {
            Debug.Log("No discovered servers found");
        }

    }

    void Connect(ServerResponse info)
    {
        StartClient(info.uri); //Starts the client using the server that responded to search query
        Debug.Log("Starting client...\n Connected to: " + info);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {

        discoveredServers[info.serverId] = info; //pass in server id as the server response of the server
    }

    #endregion



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

    //Called on SERVER when a client disconnects from server
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
        if (numPlayers < minPlayers)
        {
            //Too little players, do not start
            return false;
        }

        foreach (var player in RoomPlayers)
        {
            //Loop through list of players
            //if even 1 player is not ready, do not start
            if (player.isReady == false)
            {
                return false;
            }
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
