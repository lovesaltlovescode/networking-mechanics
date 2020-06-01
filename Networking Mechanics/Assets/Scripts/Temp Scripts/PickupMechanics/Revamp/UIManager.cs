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
/// </summary>
public class UIManager : MonoBehaviour
{
    public Image buttonIcon;

    //default sprite
    public Sprite defaultIcon;

    //DIFFERENT SPRITES
    public Sprite riceIcon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplayGrayIcon();
        DisplayHeldObjectIcon();
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
            buttonIcon.color = Color.white;

            switch (PlayerInteractionManager.heldObject.tag)
            {
                case "Rice":
                    buttonIcon.sprite = riceIcon;
                    break;
            }
        }
    }
}
