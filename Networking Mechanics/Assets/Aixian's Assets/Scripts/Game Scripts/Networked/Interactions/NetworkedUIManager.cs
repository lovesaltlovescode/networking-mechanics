using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates UI on button by checking tag of held object and detected object
/// Detected object will be grayed out icon
/// Held object will be coloured icon
/// Runs switch statement to check tag and which icon to display
/// Swaps out image sprite of a preloaded blank image on the  button
/// Takes care of wash timer, fill amount and icon
/// </summary>
public class NetworkedUIManager : MonoBehaviour
{
    public Image buttonIcon;

    //default sprite
    public Sprite defaultIcon;

    //DIFFERENT SPRITES
    [Header("Ingredient Icons")]
    public Sprite eggIcon;
    public Sprite chickenIcon;
    public Sprite cucumberIcon;
    public Sprite riceIcon;

    [Header("Drinks")]
    public Sprite fridgeIcon;
    public Sprite drinkIcon;

    [Header("Trash")]
    public Sprite trashIcon;
    public Sprite rottenIcon;



    //Table items
    [Header("Table Item Icons")]
    public Sprite dirtyPlateIcon;

    [Header("Wash icons")]
    //Washing plates
    public Sprite washIcon; //wash icon that is shown in sink zone, but will be grayed out when is washing starts
    public Image washTimerImage; //wash icon that will fill up slowly according to timer, above the washicon
    private float waitTime = 4f; //time to wait until image is filled
    public bool finishedWashing = false; //if true, spawn clean plate

    public NetworkedPlayerInteraction networkedPlayerInteraction;

    public NetworkedWashInteraction networkedWashInteraction;
         
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplayGrayIcon();
        DisplayDetectedObjectIcon();

        //wash dirty plates
        CheckWashTimer(); 
        



    }



    public void DisplayGrayIcon()
    {

        //if there is a detected object
        if (networkedPlayerInteraction.detectedObject)
        {

            //cannot change player state
            if(!networkedPlayerInteraction.CanChangePlayerState())
            {
                return;
            }

            //Debug.Log("UIManager: Detected object is " + networkedPlayerInteraction.detectedObject.tag);
            //Debug.Log("UIManager - Detected object is " + networkedPlayerInteraction.detectedObject.tag);
            //gray out the icon
            buttonIcon.color = Color.gray;

            switch (networkedPlayerInteraction.detectedObject.tag)
            {

                //shelves
                case "EggShelf":
                    buttonIcon.sprite = eggIcon;
                    break;

                case "ChickenShelf":
                    buttonIcon.sprite = chickenIcon;
                    break;

                case "CucumberShelf":
                    buttonIcon.sprite = cucumberIcon;
                    break;

                case "RiceTub":
                    buttonIcon.sprite = riceIcon;
                    break;

                //ingredients
                case "Rice":
                    buttonIcon.sprite = riceIcon;
                    break;

                case "Egg":
                    buttonIcon.sprite = eggIcon;
                    break;

                case "Chicken":
                    buttonIcon.sprite = chickenIcon;
                    break;

                case "Cucumber":
                    buttonIcon.sprite = cucumberIcon;
                    break;

                case "RottenIngredient":
                    buttonIcon.sprite = rottenIcon;
                    break;

                //drinks
                case "DrinkFridge":
                    if (networkedPlayerInteraction.playerState == PlayerState.CanSpawnDrink)
                    {
                        buttonIcon.color = Color.white;
                        buttonIcon.sprite = fridgeIcon;
                    }

                    else if (GameManager.Instance.isCooldown)
                    {
                        buttonIcon.color = Color.grey;
                        buttonIcon.sprite = fridgeIcon;
                    }
                    break;

                case "Drink":
                    buttonIcon.sprite = drinkIcon;
                    break;

                case "DirtyPlate":
                    buttonIcon.sprite = dirtyPlateIcon;
                    break;

            }

        }else if(!networkedPlayerInteraction.detectedObject)
        {
            //no detected object
            buttonIcon.sprite = defaultIcon;
            buttonIcon.color = Color.white;
            if(!networkedPlayerInteraction.playerInventory && GameManager.Instance.platesInSinkCount == 0)
            {
                networkedPlayerInteraction.ChangePlayerState(PlayerState.Default);
            }
        }

        
    }

    public void DisplayDetectedObjectIcon()
    {
        
        //if player is holding something
        if (networkedPlayerInteraction.playerInventory)
        {
            // ////Debug.Log("UIManager: Detected object is " + networkedPlayerInteraction.detectedObject.tag);
            //show the actual icon

            switch (networkedPlayerInteraction.playerInventory.tag)
            {
                case "Rice":
                    buttonIcon.sprite = riceIcon;

                    if (networkedPlayerInteraction.playerState != PlayerState.CanDropIngredient)
                    {
                        buttonIcon.color = Color.grey;

                        if (networkedPlayerInteraction.playerState == PlayerState.CanThrowIngredient)
                        {
                            buttonIcon.sprite = trashIcon;
                            buttonIcon.color = Color.white;
                        }
                    }
                    else
                    {
                        buttonIcon.color = Color.white;
                    }

                    break;

                case "Egg":
                    buttonIcon.sprite = eggIcon;

                    if (networkedPlayerInteraction.playerState != PlayerState.CanDropIngredient)
                    {
                        buttonIcon.color = Color.grey;

                        if (networkedPlayerInteraction.playerState == PlayerState.CanThrowIngredient)
                        {
                            buttonIcon.sprite = trashIcon;
                            buttonIcon.color = Color.white;
                        }
                    }
                    else
                    {
                        buttonIcon.color = Color.white;
                    }

                    break;

                case "Chicken":
                    buttonIcon.sprite = chickenIcon;

                    if (networkedPlayerInteraction.playerState != PlayerState.CanDropIngredient)
                    {
                        buttonIcon.color = Color.grey;

                        if (networkedPlayerInteraction.playerState == PlayerState.CanThrowIngredient)
                        {
                            buttonIcon.sprite = trashIcon;
                            buttonIcon.color = Color.white;
                        }
                    }
                    else
                    {
                        buttonIcon.color = Color.white;
                    }

                    break;

                case "Cucumber":
                    buttonIcon.sprite = cucumberIcon;

                    if (networkedPlayerInteraction.playerState != PlayerState.CanDropIngredient)
                    {
                        buttonIcon.color = Color.grey;

                        if(networkedPlayerInteraction.playerState == PlayerState.CanThrowIngredient)
                        {
                            buttonIcon.sprite = trashIcon;
                            buttonIcon.color = Color.white;
                        }
                    }
                    else
                    {
                        buttonIcon.color = Color.white;
                    }

                    break;


                //rotten ingredient
                case "RottenIngredient":
                    if (networkedPlayerInteraction.playerState == PlayerState.CanPickUpRottenIngredient)
                    {
                        buttonIcon.sprite = rottenIcon;
                        buttonIcon.color = Color.gray;
                    }
                    else if(networkedPlayerInteraction.playerState == PlayerState.HoldingRottenIngredient)
                    {
                        buttonIcon.sprite = rottenIcon;
                        buttonIcon.color = Color.white;
                    }
                    else if(networkedPlayerInteraction.playerState == PlayerState.CanThrowIngredient)
                    {
                        buttonIcon.sprite = trashIcon;
                        buttonIcon.color = Color.white;
                    }
                    break;

                //drinks
                case "Drink":
                    buttonIcon.sprite = drinkIcon;
                    if(networkedPlayerInteraction.playerState != PlayerState.CanUseDrink)
                    {
                        buttonIcon.color = Color.grey;
                    }
                    else
                    {
                        buttonIcon.color = Color.white;
                    }
                    break;

                case "DirtyPlate":
                    buttonIcon.sprite = dirtyPlateIcon;

                    if (networkedPlayerInteraction.playerState != PlayerState.CanPlacePlateInSink)
                    {
                        buttonIcon.color = Color.grey;
                    }
                    else
                    {
                        buttonIcon.color = Color.white;
                    }
                    break;
            }

        }
    }

    //If in sink zone and able to wash show washable icon
    //If start timer, set icon to gray, and start fillamount
    public void ToggleWashIcons()
    {
        switch (networkedPlayerInteraction.playerState)
        {
            case PlayerState.CanPlacePlateInSink:
                buttonIcon.color = Color.white;
                buttonIcon.sprite = washIcon;
                break;

            case PlayerState.CanWashPlate:
                buttonIcon.color = Color.white;
                buttonIcon.sprite = washIcon;
                ////Debug.Log("Player can wash plate");
                break;

            case PlayerState.WashingPlate:
                buttonIcon.sprite = washIcon;
                buttonIcon.color = Color.gray;
                break;

            case PlayerState.ExitedSink:
                networkedWashInteraction.showWashIcon = false;
                break;
        }
    }


    //check if the timer has been started to wash plates
    public void CheckWashTimer()
    {
        if (networkedWashInteraction.startTimer)
        {
            StartTimer();
        }
        else if (!networkedWashInteraction.startTimer)
        {
            washTimerImage.fillAmount = 0; //do not fill up image
        }


        if (networkedWashInteraction.showWashIcon)
        {
            ToggleWashIcons();
        }

        //if image is completely filled, reset fill to 0
        if (washTimerImage.fillAmount == 1)
        {
            networkedWashInteraction.FinishWashingPlate();
            washTimerImage.fillAmount = 0;

        }
    }

    //increase fill amount of image
    public void StartTimer()
    {
        //Increase fill amount over waittime seconds
        washTimerImage.fillAmount += 1.0f / waitTime * Time.deltaTime;
        //Debug.Log("UI Manager: washing plates");
    }
}
