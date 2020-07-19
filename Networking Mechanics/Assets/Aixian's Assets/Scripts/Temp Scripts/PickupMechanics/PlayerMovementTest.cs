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
    public float moveForce = 10f;

    [SerializeField] private FixedJoystick joystick = null;



    void Start()
    {
        myBody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {

        //MOVE CHARACTER

        myBody.velocity = new Vector3(joystick.Horizontal * moveForce, myBody.velocity.y, joystick.Vertical * moveForce);

        //print(joystick.Horizontal);

        if (joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            transform.rotation = Quaternion.LookRotation(myBody.velocity);
        }
    }


}
