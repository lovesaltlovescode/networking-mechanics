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


    void Start()
    {
        myBody = GetComponent<Rigidbody>();

        //Get button component and call function to interact
        //interactButton.GetComponent<Button>().onClick.AddListener(InteractButton);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        //playerPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2, gameObject.transform.position.z);

        //MOVE CHARACTER

        myBody.velocity = new Vector3(joystick.Horizontal * moveForce, myBody.velocity.y, joystick.Vertical * moveForce);

        //print(joystick.Horizontal);

        if (joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            transform.rotation = Quaternion.LookRotation(myBody.velocity);
        }
    }


}
