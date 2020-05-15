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

    [SerializeField] private FixedJoystick joystick = null;

    //interact button
    [SerializeField] private Button interactButton = null;

    //reference to inventory (slot UI)
    [SerializeField] private InventoryTemp inventoryTemp = null;

    //BOOL TO CHECK IF PLAYER CAN PICK UP DIFFERENT OBJECTS
    public bool canPickUpPlate = true;
    public bool canPickUpDirtyPlate = true;

    //ICONS OF DIFFERENT OBJECTS
    public GameObject plateIcon;
    public GameObject dirtyPlateIcon;

    void Start()
    {
        myBody = GetComponent<Rigidbody>();

        //Get button component and call function to interact
        interactButton.GetComponent<Button>().onClick.AddListener(InteractButton);
    }

    // Update is called once per frame
    void Update()
    {

        myBody.velocity = new Vector3(joystick.Horizontal * moveForce, myBody.velocity.y, joystick.Vertical * moveForce);

        //print(joystick.Horizontal);

        if(joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            transform.rotation = Quaternion.LookRotation(myBody.velocity);
        }

    }

    //TODO: player interact function
    //function that allows player to pick up and place objects
    //fimctopm that allows player to interact with objects
    public void InteractButton()
    {
        //If holding nothing
        if(inventoryTemp.isFull[0] == false)
        {
            if (canPickUpPlate == true)
            {
                //Pick up plate
                //instantiate plate icon as a child of the slot
                Instantiate(plateIcon, inventoryTemp.slots[0].transform, false);
                Debug.Log("Picked up a plate!");
                inventoryTemp.isFull[0] = true;
                canPickUpPlate = false;
            }
            //DIRTY PLATE
            else if (canPickUpDirtyPlate == true)
            {
                Instantiate(dirtyPlateIcon, inventoryTemp.slots[0].transform, false);
                Debug.Log("Picked up a dirty plate");
                inventoryTemp.isFull[0] = true;
                canPickUpDirtyPlate = false;
            }
        }
        
        else
        {
            Debug.Log("You're already holding something!");
        }



    }

    //do a switch check here for collided object tag
    private void OnTriggerEnter(Collider other)
    {

        //if enter any trigger, then able to interact
        //may change this in the future to check for any matching tags in an array of tags, if there's a matching tag, then the button is interactable
        interactButton.interactable = true;

        //on button click, pick up object and instantiate
        switch (other.gameObject.tag)
        {
            case "Plate":
                Debug.Log("Near a plate");
                canPickUpPlate = true;
                break;

            case "DirtyPlate":
                Debug.Log("Near a dirty plate");
                canPickUpDirtyPlate = true;
                break;

            //TODO: Add more items, objects, customers
        }

        

        
    }

    private void OnTriggerExit(Collider other)
    {
        //Do not allow player to interact when outside of any triggers
        interactButton.interactable = false;
    }
}
