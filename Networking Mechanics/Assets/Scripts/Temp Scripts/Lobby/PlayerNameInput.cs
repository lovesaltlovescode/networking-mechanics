using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#region Summary 
//Allow player to type in a player name, which can be accessed from other scripts, and can be changed later in the lobby
//Player name cannot contain less than 2 characters, and more than 8 characters
//TODO: Player name cannot contain crude words
//Each client will have their own player name, that will be synced through the network later in the lobby
#endregion

public class PlayerNameInput : MonoBehaviour
{

    //VARIABLES
    [Header("UI")]

    //Input field for users to type their name in
    [SerializeField] private TMP_InputField nameInputField = null; //Do not allow assigning from within inspector

    //Button that gets enabled when player name is valid
    [SerializeField] private Button continueButton = null;

    [SerializeField] private GameObject landingPagePanel = null;

    //String that contains the player's name
    public static string playerName;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PlayerNameInput: Active");

        //reset playername
        playerName = "";

        //Set button uninteractive
        SetPlayerName(playerName);

        nameInputField.characterLimit = 8; //Limit characters to 8
    }

    //On input field value changed , on start
    public void SetPlayerName(string playerName)
    {
        //Toggle interactivity of button


        if(nameInputField.text != "")
        {
            //There is a name entered
            Debug.Log("Valid name entered");

            //Continue button is interactable
            continueButton.interactable = true;

        }
        else
        {
            Debug.Log("Enter a valid name!");
            continueButton.interactable = false;
        }
    }

    public void SavePlayerName()
    {
        //When player presses on continue and name is valid
        //Save the text present in input field as the player name
        playerName = nameInputField.text;

        Debug.Log("Player name is: " + playerName);

        
        //Enable Host, Join Lobby UI 
        gameObject.SetActive(false);

        landingPagePanel.SetActive(true);

        Debug.Log("Moving to landing page...");
    }
}
