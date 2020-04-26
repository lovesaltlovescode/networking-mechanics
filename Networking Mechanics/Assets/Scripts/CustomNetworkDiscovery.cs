using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using Mirror;
using TMPro;


//Network discovery HUD
//Responsible for establishing server connections and advertising the server
//Clients join using this script, and are then brought to a new scene (lobby)

[RequireComponent(typeof(NetworkDiscovery))]
public class CustomNetworkDiscovery : MonoBehaviour
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>(); //list of discovered server's ip address

    public NetworkDiscovery networkDiscovery; 

    public CustomNetworkManager customNetworkManager; //enable and disable script

    //text for discovered servers
    [SerializeField] private TextMeshProUGUI discoveredServerText = null;

    //text for discovered server list
    [SerializeField] private TextMeshProUGUI discoveredServerAddress = null;

    //Bool to check if displayed found servers
    [SerializeField] private bool stopSearching; //if true, stop searching for servers

    //Do not run if
    public void Awake()
    {
        //if (NetworkManager.singleton == null)
        //    return; //if no network manager

        if (NetworkServer.active || NetworkClient.active)
            return; //if currently running server or client

    }

    public void Start()
    {
        Debug.Log("CustomNetworkDiscovery: Script running");

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update: Discovered Servers: " + discoveredServers.Count);

        
        
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
        NetworkManager.singleton.StartHost(); //start a new network manager host
        networkDiscovery.AdvertiseServer(); //advertises this new host in the list

        Debug.Log("Host server advertised");

        //Instantiate(playerPrefab);

        Debug.Log("Show lobby");
        //Go to lobby scene



    }


    //TODO: Always search for active servers (Update, coroutine) while in menu scene
    public void FindServers()
    {
        //Find available servers
        //Debug.Log("Discovered Servers Before Clear: " + discoveredServers.Count);
        //discoveredServers.Clear(); //clear existing servers
        networkDiscovery.StartDiscovery(); //search for more servers
        //Debug.Log("Searching for servers...");
        Debug.Log("Discovered Servers: " + discoveredServers.Count);

        if (stopSearching == true)
        {

            Debug.Log("Stop searching for servers...");
            return;
        }
        DisplayServers(); //this will be called separately, on button click
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
                Debug.Log("Connecting to: " + info.EndPoint.Address.ToString());
                Connect(info);
                stopSearching = true; //Stop searching for active servers now that we are connected
                
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
        NetworkManager.singleton.StartClient(info.uri); //Starts the client using the server that responded to search query
        Debug.Log("Starting client...\n Connected to: " + info);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {

        discoveredServers[info.serverId] = info; //pass in server id as the server response of the server
    }
}

