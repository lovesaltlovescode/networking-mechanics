using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Discovery
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkDiscoveryHUD")]
    [HelpURL("https://mirror-networking.com/docs/Components/NetworkDiscovery.html")]
    [RequireComponent(typeof(NetworkDiscovery))]
    public class NetworkDiscoveryHUD : MonoBehaviour
    {
        readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>(); //list of discovered server's ip address
        Vector2 scrollViewPos = Vector2.zero;

        public NetworkDiscovery networkDiscovery;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (networkDiscovery == null)
            {
                networkDiscovery = GetComponent<NetworkDiscovery>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
                UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
            }
        }
#endif

        void OnGUI()
        {
            if (NetworkManager.singleton == null)
                return; //if no network manager
             
            if (NetworkServer.active || NetworkClient.active)
                return; //if currently running server or client

            if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
                DrawGUI(); //if not connected and no connections active
        }

        void DrawGUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Find Servers"))
            {
                discoveredServers.Clear(); //clear existing servers
                networkDiscovery.StartDiscovery(); //search for more servers

                Debug.Log("NetworkDiscoveryHUD: Searching for servers...");
                Debug.Log("NetworkDiscoveryHUD: Discovered Servers: " + discoveredServers.Count);
            }

            // LAN Host
            if (GUILayout.Button("Start Host"))
            {
                discoveredServers.Clear(); //clear existing servers
                NetworkManager.singleton.StartHost(); //start a new network manager host
                networkDiscovery.AdvertiseServer(); //advertises this new host in the list
            }

            // Dedicated server
            if (GUILayout.Button("Start Server"))
            {
                discoveredServers.Clear();
                NetworkManager.singleton.StartServer();

                networkDiscovery.AdvertiseServer();
            }

            GUILayout.EndHorizontal();

            // show list of found server

            GUILayout.Label($"Discovered Servers [{discoveredServers.Count}]:");

            // servers
            scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);
            Debug.Log("Discovered Servers: " + discoveredServers.Count.ToString());

            foreach (ServerResponse info in discoveredServers.Values)
            {
                Debug.Log("Found this server: " + info.EndPoint.Address.ToString());
                if (GUILayout.Button(info.EndPoint.Address.ToString()))

                    Connect(info); //Connect and start the client according to the server that sent it
                     Debug.Log("Connecting to: " + info.EndPoint.Address.ToString());
            }

               

            GUILayout.EndScrollView();
        }

        void Connect(ServerResponse info) 
        { 
            NetworkManager.singleton.StartClient(info.uri); //Starts the client using the server that responded to search query
            Debug.Log("Starting client...\n Connected to: " + info);
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info; //pass in server id as the server response of the server
        }
    }
}
