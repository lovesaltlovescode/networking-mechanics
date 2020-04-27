using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Find network manager when it is destroyed and add a function as an onclick event
public class FindServerButton : MonoBehaviour
{
    //reference to script
    [SerializeField] CustomNetworkDiscovery customDiscoveryScript;

    //reference to button
    [SerializeField] private Button thisButton;

    // Start is called before the first frame update
    void Start()
    {
        customDiscoveryScript = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<CustomNetworkDiscovery>();

        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(customDiscoveryScript.FindServers);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
