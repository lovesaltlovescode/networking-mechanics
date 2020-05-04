using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Find network manager when it is destroyed and add a function as an onclick event
//Try again button, when no available rooms are found, try to search again
public class TryAgainButton : MonoBehaviour
{

    //reference to script
    [SerializeField] CustomNetworkDiscovery customDiscoveryScript;
    //reference to button
    [SerializeField] private Button thisButton;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TryAgainButton: Script initialised");

        //custom network discovery script
        customDiscoveryScript = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<CustomNetworkDiscovery>();

        //button
        thisButton = gameObject.GetComponent<Button>();

        //connect to server function added as an event listener 
        //Only happens when the button is clickable
        thisButton.onClick.AddListener(customDiscoveryScript.FindServers);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
