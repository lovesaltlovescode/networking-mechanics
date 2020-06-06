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
public class UIManager : MonoBehaviour
{
    public Image buttonIcon;

    //default sprite
    public Sprite defaultIcon;

    //DIFFERENT SPRITES
    public Sprite riceIcon;
    public Sprite eggIcon;

    //Table items
    public Sprite dirtyPlateIcon;

    //Washing plates
    public Sprite washIcon; //wash icon that is shown in sink zone, but will be grayed out when is washing starts
    public Image washTimerImage; //wash icon that will fill up slowly according to timer, above the washicon
    private float waitTime = 4f; //time to wait until image is filled
    public bool finishedWashing = false; //if true, spawn clean plate


    public SinkInteraction sinkInteraction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplayGrayIcon();
        DisplayHeldObjectIcon();

        if (sinkInteraction.startTimer)
        {
            StartTimer();
        }
        else if(!sinkInteraction.startTimer)
        {
            washTimerImage.fillAmount = 0; //do not fill up image
        }


        if (sinkInteraction.showWashIcon)
        {
            ToggleWashIcons();
        }

        //if image is completely filled
        if (washTimerImage.fillAmount == 1)
        {
            PlayerInteractionManager.playerState = PlayerInteractionManager.PlayerState.FinishedWashingPlate;
            washTimerImage.fillAmount = 0;
            
        }



    }

    public void DisplayGrayIcon()
    {

        //if there is a detected object
        if (PlayerInteractionManager.detectedObject)
        {
            Debug.Log("UIManager: Detected object is " + PlayerInteractionManager.detectedObject.tag);
            //gray out the icon
            buttonIcon.color = Color.gray;

            switch (PlayerInteractionManager.detectedObject.tag)
            {
                case "Rice":
                    buttonIcon.sprite = riceIcon;
                    break;

                case "Egg":
                    buttonIcon.sprite = eggIcon;
                    break;

                case "DirtyPlate":
                    buttonIcon.sprite = dirtyPlateIcon;
                    break;
            }
        }else if(!PlayerInteractionManager.detectedObject)
        {

            //no detected object
            buttonIcon.sprite = defaultIcon;
            buttonIcon.color = Color.white;
        }
        
    }

    public void DisplayHeldObjectIcon()
    {
        
        //if there is a held object
        if (PlayerInteractionManager.heldObject)
        {
            Debug.Log("UIManager: Held object is " + PlayerInteractionManager.heldObject.tag);
            //show the actual icon
            

            switch (PlayerInteractionManager.heldObject.tag)
            {
                case "Rice":
                    buttonIcon.sprite = riceIcon;

                    if(PlayerInteractionManager.playerState != PlayerInteractionManager.PlayerState.CanDropIngredient)
                    {
                        buttonIcon.color = Color.grey;
                    }
                    else
                    {
                        buttonIcon.color = Color.white;
                    }

                    break;

                case "Egg":
                    buttonIcon.sprite = eggIcon;

                    if (PlayerInteractionManager.playerState != PlayerInteractionManager.PlayerState.CanDropIngredient)
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

                    if(PlayerInteractionManager.playerState != PlayerInteractionManager.PlayerState.CanPlacePlateInSink)
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
        switch (PlayerInteractionManager.playerState)
        {
            case PlayerInteractionManager.PlayerState.CanPlacePlateInSink:
                buttonIcon.color = Color.white;
                buttonIcon.sprite = washIcon;
                break;

            case PlayerInteractionManager.PlayerState.CanWashPlate:
                buttonIcon.color = Color.white;
                buttonIcon.sprite = washIcon;
                break;

            case PlayerInteractionManager.PlayerState.WashingPlate:
                buttonIcon.sprite = washIcon;
                buttonIcon.color = Color.gray;
                break;

            case PlayerInteractionManager.PlayerState.ExitedSink:
                sinkInteraction.showWashIcon = false;
                break;
        }
    }

    //increase fill amount of image
    public void StartTimer()
    {
        //Increase fill amount over waittime seconds
        washTimerImage.fillAmount += 1.0f / waitTime * Time.deltaTime;
        Debug.Log("UI Manager: washing plates");
    }
}
