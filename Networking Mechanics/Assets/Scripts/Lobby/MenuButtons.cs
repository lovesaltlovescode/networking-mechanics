using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#region Summary

//Script to control active state of buttons and text changes according to the network discovery status
//When scene is reloaded, will find the custom network discovery script
//Add functions from that script as an event listener for the buttons

#endregion

public class MenuButtons : MonoBehaviour
{
    //Button to host room
    [SerializeField] private Button hostRoomButton = null;

    //Button to find rooms
    [SerializeField] private Button findRoomButton = null;

    //Button to try again, if no rooms are found
    [SerializeField] private Button tryAgainButton = null;

    //Button to connect to room, if room is found
    [SerializeField] private Button joinRoomButton = null;

    //Text for connection to server, toggle between connect and try again
    [SerializeField] private TextMeshProUGUI connectText = null;

    //Reference to custom network discovery script
    [SerializeField] CustomNetworkDiscovery customNetworkDiscovery;

    #region PlatformDependencies


#if UNITY_ANDROID

    public void Awake()
    {
        Debug.Log("On android!");
        hostRoomButton.gameObject.SetActive(false);
    }

#elif UNITY_EDITOR

    public void Awake()
    {
        Debug.Log("In editor!");
        hostRoomButton.gameObject.SetActive(true);
    }

#elif UNITY_STANDALONE_WIN

    public void Awake()
    {
        Debug.Log("Standalone windows!");
        hostRoomButton.gameObject.SetActive(false);
    }

#endif

    #endregion
    //Disable host button for android users (clients)



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("MenuButtons: Script initialised.");

        //custom network discovery script
        customNetworkDiscovery = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<CustomNetworkDiscovery>();

        //Add event listeners for all the buttons
        hostRoomButton.onClick.AddListener(customNetworkDiscovery.StartHost);
        findRoomButton.onClick.AddListener(customNetworkDiscovery.FindServers);

        tryAgainButton.onClick.AddListener(customNetworkDiscovery.FindServers);
        joinRoomButton.onClick.AddListener(customNetworkDiscovery.ConnectToServer);
    }

    // Update is called once per frame
    void Update()
    {
        CheckRoomStatus();
    }

    void CheckRoomStatus()
    {
        if(customNetworkDiscovery.roomFound == false)
        {
            //enable try again text, button
            connectText.text = "No rooms found. Try again?";
            //Debug.Log("Unable to find rooms");
            tryAgainButton.gameObject.SetActive(true);
            joinRoomButton.gameObject.SetActive(false);
        }
        else if(customNetworkDiscovery.roomFound == true)
        {
            //enable connect text, button
            connectText.text = "Found a room! Join this room?";
            //Debug.Log("Found a room!");
            tryAgainButton.gameObject.SetActive(false);
            joinRoomButton.gameObject.SetActive(true);
        }
    }
}
