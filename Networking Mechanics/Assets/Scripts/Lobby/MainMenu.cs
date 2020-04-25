using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

#region Summary

//After clicking continue button, players will end up in the landing page
//Players can either choose to host a lobby (network manager start host)
//Or join an existing lobby 
//TODO: Join lobby using room name

#endregion
public class MainMenu : MonoBehaviour
{
    [SerializeField] private CustomNetworkManager networkManager;

    //[Header("UI")]
    //[SerializeField] private GameObject landingPagePanel = null;
    //[SerializeField] private TMP_InputField ipAddressInputField = null; //Input ipAddress, TODO: room name
    //[SerializeField] private GameObject ipInputPanel = null;
    ////[SerializeField] private Button hostButton = null;
    //[SerializeField] private Button joinButton = null;

    //[SerializeField] private TextMeshProUGUI connectionStatus = null;

    private void OnEnable()
    {
        //Subscribe to events
        NetworkManagerCustom.OnClientConnected += HandleClientConnected;
        NetworkManagerCustom.OnClientDisconnected += HandleClientDisconnected;

        networkManager = FindObjectOfType<CustomNetworkManager>();
    }

    private void OnDisable()
    {
        //Unsubscribe from events
        NetworkManagerCustom.OnClientConnected -= HandleClientConnected;
        NetworkManagerCustom.OnClientDisconnected -= HandleClientDisconnected;
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("MainMenu: Active");
    }

    ////Host a room when host button is pressed
    //public void HostLobby()
    //{
    //    //Start as host
    //    networkManager.StartHost();

    //    //Set landing page false
    //   // landingPagePanel.SetActive(false);

    //    Debug.Log("MainMenu: Found host");
    //}

    ////When button pressed
    ////Room (IP) address input panel is set active
    ////Only if the address is valid, can they connect
    //public void JoinLobby()
    //{

    //    string userInput = ipAddressInputField.text; //store value of input field in a string

    //    //compare user input with the address
    //    if(userInput == networkManager.networkAddress)
    //    {
    //        Debug.Log("MainMenu: Valid IP address entered");

    //        connectionStatus.text = "Joining room...";

    //        networkManager.StartClient();

    //        Debug.Log("MainMenu: Client joined");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("MainMenu: Wrong IP address, try again");

    //        connectionStatus.text = "Room not found";
    //    }


    //}

    //Handle client connected
    //Re-enable button
    private void HandleClientConnected()
    {
        Debug.Log("Client connected");


    }

    //Handle client disconnected
    //Return to landing page
    //Re-enable button
    private void HandleClientDisconnected()
    {
        Debug.Log("Client disconnected");
    }
}
