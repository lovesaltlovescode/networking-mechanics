using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

//move player, let player use interact button
//integrate pickup items script
//if collided item is an object, pick up by pressing the button
//do the check if inventory is full when pressed the button
//if inventory full, debug log
//if inventory empty, then pick up and instantiate the respective icon
public class PlayerMovementTest : MonoBehaviour
{
    private Rigidbody myBody;
    public float moveForce = 25f;

    public static Vector3 playerPos; //position of player

    [SerializeField] private FixedJoystick joystick = null;

    //interact button
    [SerializeField] private Button interactButton = null;

    ////reference to inventory(slot UI)
    ////[SerializeField] private InventoryTemp inventoryTemp = null;

    ////BOOL TO CHECK IF PLAYER CAN DROP OBJECTS
    ////[SerializeField] private bool canDropItem = false;

    ////BOOL TO CHECK IF PLAYER CAN PICK UP DIFFERENT OBJECTS
    ////[SerializeField]  private bool canPickUpPlate = false;
    ////[SerializeField] private bool canPickUpDirtyPlate = false;

    ////BOOL TO CHECK IF PLAYER HAS PICKED UP OBJECTS
    ////IF TRUE, DESTROY IT
    ////[SerializeField] private bool hasPickedUpItem = false;

    ////ICONS OF DIFFERENT OBJECTS
    ////public GameObject plateIcon;
    ////public GameObject dirtyPlateIcon;

    void Start()
    {
        myBody = GetComponent<Rigidbody>();

        //Get button component and call function to interact
        //interactButton.GetComponent<Button>().onClick.AddListener(InteractButton);
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2, gameObject.transform.position.z);

        //MOVE CHARACTER

        myBody.velocity = new Vector3(joystick.Horizontal * moveForce, myBody.velocity.y, joystick.Vertical * moveForce);

        //print(joystick.Horizontal);

        if(joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            transform.rotation = Quaternion.LookRotation(myBody.velocity);
        }

    }

    
    //Player interact function
    //If holding nothing and near object, pick it up and destroy the object
    //If holding something and in drop zone, drop it and instantiate an object
    //public void InteractButton()
    //{
    //    //If holding nothing
    //    if(inventoryTemp.isFull[0] == false)
    //    {
    //        if (canPickUpPlate == true)
    //        {
    //            //Pick up plate
    //            //instantiate plate icon as a child of the slot
    //            Instantiate(plateIcon, inventoryTemp.slots[0].transform, false);
    //            Debug.Log("Picked up a plate!");
    //            inventoryTemp.isFull[0] = true;
    //            hasPickedUpItem = true;
    //            canPickUpPlate = false;
    //        }
    //        //DIRTY PLATE
    //        else if (canPickUpDirtyPlate == true)
    //        {
    //            Instantiate(dirtyPlateIcon, inventoryTemp.slots[0].transform, false);
    //            Debug.Log("Picked up a dirty plate");
    //            inventoryTemp.isFull[0] = true;
    //            hasPickedUpItem = true;
    //            canPickUpDirtyPlate = false;
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("You're holding something!");
    //    }
        
    //    //If in designated area, and holding something
    //    if(canDropItem == true)
    //    {
    //        if(inventoryTemp.isFull[0] == true)
    //        {
    //            //in designated areas, able to drop
    //            //for each child in the interact button
    //            foreach (Transform child in interactButton.transform)
    //            {
    //                //for each item in the slot, drop it
    //                //destroy the icon
    //                GameObject.Destroy(child.gameObject);

    //                //call spawndroppeditem function
    //                child.GetComponent<SpawnItemTemp>().SpawnDroppedItem();

    //                Debug.Log("Dropping an item");
    //                //set as empty
    //                inventoryTemp.isFull[0] = false;

    //            }
    //        }
            
    //    }
    //    //if in designated area and nothing to drop
    //    //if in dropzone, and there is an ingredient in the floor
    //    //then able to pick up
    //    else if(canDropItem == false && inventoryTemp.isFull[0] == false)
    //    {
    //        Debug.Log("There's nothing to drop!");
    //    }



    //}

    //do a switch check here for collided object tag
    //private void OnTriggerEnter(Collider other)
    //{

    //    //if enter any trigger, then able to interact
    //    //may change this in the future to check for any matching tags in an array of tags, if there's a matching tag, then the button is interactable
    //    interactButton.interactable = true;


    //    //on button click, pick up object and instantiate
    //    switch (other.gameObject.tag)
    //    {

    //        //CHECK FOR ZONES
    //        case "DropZone":
    //            Debug.Log("Able to drop items!");
    //            for(int i = 0; i < inventoryTemp.slots.Length; i++)
    //            {
    //                if(inventoryTemp.isFull[i] == true)
    //                {
    //                    //can drop
    //                    canDropItem = true;
    //                }
    //            }
    //            break;

    //        //CHECK FOR OBJECT

    //        case "Plate":
    //            Debug.Log("Near a plate");
    //            for(int i = 0; i < inventoryTemp.slots.Length; i++)
    //            {
    //                if(inventoryTemp.isFull[i] == false)
    //                {
    //                    //if not full able to pick up
    //                    canPickUpPlate = true;
    //                }
    //            }

    //            break;

    //        case "DirtyPlate":
    //            Debug.Log("Near a dirty plate");
    //            for (int i = 0; i < inventoryTemp.slots.Length; i++)
    //            {
    //                if (inventoryTemp.isFull[i] == false)
    //                {
    //                    //if not full able to pick up
    //                    canPickUpDirtyPlate = true;
    //                }
    //            }

    //            break;

    //        //TODO: Add more items, objects, customers
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{

    //    if(other.gameObject.tag != "DropZone")
    //    {
    //        if (hasPickedUpItem == true)
    //        {
    //            interactButton.interactable = false;
    //            Destroy(other.gameObject);
    //            hasPickedUpItem = false;
    //        }
    //    }
        
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    //Do not allow player to interact when outside of any triggers
    //    interactButton.interactable = false;

    //    //Do not allow player to pick up
    //    switch (other.gameObject.tag)
    //    {
    //        //ZONES
    //        case "DropZone":
    //            canDropItem = false;
    //            break;

    //        //OBJECTS
    //        case "Plate":
    //            canPickUpPlate = false;
    //            break;

    //        case "DirtyPlate":
    //            canPickUpDirtyPlate = false;
    //            break;

    //            //TODO: Add more items, objects, customers
    //    }
    //}
}
