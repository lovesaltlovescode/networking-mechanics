using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Find network manager when it is destroyed and add a function as an onclick event
//Confirm button,join available room
public class ConnectToServerButton : MonoBehaviour
{
    //reference to script
    [SerializeField] CustomNetworkDiscovery customDiscoveryScript;
    //reference to button
    [SerializeField] private Button thisButton;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("ConnectToServerButton: Script initialised");

        //custom network discovery script
        customDiscoveryScript = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<CustomNetworkDiscovery>();

        //button
        thisButton = gameObject.GetComponent<Button>();
        
        //connect to server function added as an event listener 
        //Only happens when the button is clickable
        thisButton.onClick.AddListener(customDiscoveryScript.ConnectToServer);

    }




}
