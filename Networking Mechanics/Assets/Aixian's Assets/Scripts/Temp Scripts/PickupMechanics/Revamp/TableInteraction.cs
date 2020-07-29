using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles interaction with shop tables eg. collecting dirty plates, money, maybe serving dishes
/// Allows player to pick up the dirty plates if they are near the table / detected object
/// Only cares about objects on the TableItem layer
/// Changes what the player is currentlyholding (variable) and the player state accordingly
/// Conditions: Near an object of TableItem layer + not holding anything
/// 
/// </summary>
public class TableInteraction : MonoBehaviour
{
    public bool tableItemDetected;

    void Start()
    {
        Debug.Log("TableInteraction - Script initialised");
    }

    //pick up table item function
    public void PickUpTableItem(GameObject detectedItem, List<GameObject> Inventory, Transform attachPoint)
    {

        if(detectedItem.tag == "DirtyPlate")
        {
            //if its a dirty plate, check how many plates there are in the sink
            if(WashInteraction.platesInSinkCount == 2)
            {
                //do not allow pick up
                Debug.Log("TableInteraction - Unable to pick up plate! Wash the rest first!");
                return;
            }
        }

        //only able to pick up if inventory count is 0
        if(PlayerInteractionManager.objectsInInventory.Count != 0)
        {
            return;
        }
        Debug.Log("TableInteraction - Pick up table item");

        //Parent to attachment point and transform
        detectedItem.transform.parent = attachPoint.transform;
        detectedItem.transform.position = attachPoint.position;

        //Add to inventory
        Inventory.Add(detectedItem);

        //Change layer to Player/pickeup so cannot be detected by other players
        // detectedObject.layer = LayerMask.NameToLayer("PickedUp");

        //change held object to be the detected object
        PlayerInteractionManager.detectedObject = detectedItem;
        Debug.Log("Table interaction - Player is holding " + PlayerInteractionManager.detectedObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInteractionManager.detectedObject && PlayerInteractionManager.detectedObject.layer == 16)
        {
            tableItemDetected = true;
        }
        else
        {
            tableItemDetected = false;
        }

        CheckTableItemCriteria();
    }


    //Player must be near a table item layer and have nothing in their inventory
    public void CheckTableItemCriteria()
    {
        if (tableItemDetected)
        {
            //if inventory not full
            if (!PlayerInteractionManager.IsInventoryFull())
            {
                //change player state
                Debug.Log("TableInteraction - Can pick up table item!");
                PlayerInteractionManager.ChangePlayerState(PlayerInteractionManager.PlayerState.CanPickUpDirtyPlate);
            }
            else
            {
                //if inventory is full
                Debug.Log("TableInteraction - Inventory is full, unable to pick up!");
                
            }
        }
    }

}
