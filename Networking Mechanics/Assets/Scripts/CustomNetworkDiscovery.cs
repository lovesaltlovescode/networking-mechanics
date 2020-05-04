using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//Network discovery HUD
//Responsible for establishing server connections and advertising the server
//Clients join using this script, and are then brought to a new scene (lobby)
//If there are no available rooms to connect to, clients have to try again
//If there available rooms, clients can choose to connect

[RequireComponent(typeof(NetworkDiscovery))]
public class CustomNetworkDiscovery : MonoBehaviour
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>(); //list of discovered server's ip address

    public NetworkDiscovery networkDiscovery; 

    public CustomNetworkManager customNetworkManager; //enable and disable script

    //text for whether any server is available to connect
    [SerializeField] private TextMeshProUGUI connectToRoom;

    //button to connect to server
    [SerializeField] private Button connectButton;

    //button to try again
    [SerializeField] private Button tryAgain;

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
        if (SceneManager.GetActiveScene().name == "Scene_Map_Shop")
        {
            //if in game scene, do not enable network discovery
            Debug.Log("in game scene, remove network discovery");
            this.enabled = false;
        }
    }

        
    //When start host button is clicked
    public void StartHost()
    {
        discoveredServers.Clear(); //clear existing servers

        Debug.Log("Starting as host...");
        NetworkManager.singleton.StartHost(); //start a new network manager host
        networkDiscovery.AdvertiseServer(); //advertises this new host in the list

        Debug.Log("Host server advertised");

        Debug.Log("Show lobby");



    }


    //TODO: Always search for active servers (Update, coroutine) while in menu scene
    public void FindServers()
    {
        //Find available servers
        networkDiscovery.StartDiscovery(); //search for more servers
        Debug.Log("Discovered Servers: " + discoveredServers.Count);

        if(discoveredServers.Count == 0)
        {
            Debug.Log("No rooms found. Try again?");
            connectToRoom.text = "No rooms found. Try again?";
            tryAgain.gameObject.SetActive(true);
            connectButton.gameObject.SetActive(false);
        }
        else
        {
            connectToRoom.text = $"Found {discoveredServers.Count} rooms. Join this room?";
            connectButton.gameObject.SetActive(true);
            tryAgain.gameObject.SetActive(false);
        }

    }

    //display found servers in a list on button press, for debug purposes
    //will not be in final game, no need to display, only needed to find
    public void ConnectToServer()
    {
        if (discoveredServers.Count != 0)
        {
            //discoveredServerText.text = "Discovered Servers: " + discoveredServers.Count.ToString();
            //Debug.Log("Discovered Servers: " + discoveredServers.Count.ToString());

            //for each server found in servers
            foreach (ServerResponse info in discoveredServers.Values)
            {
                Debug.Log("Found this server: " + info.EndPoint.Address.ToString());
                //discoveredServerAddress.text = info.EndPoint.Address.ToString(); //IP Address of server found

                Debug.Log("Connecting to: " + info.EndPoint.Address.ToString());
                Connect(info);
                
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

