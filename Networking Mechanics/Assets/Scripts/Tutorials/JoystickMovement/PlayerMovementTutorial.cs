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
public class PlayerMovementTutorial : MonoBehaviour
{
    private Rigidbody myBody;
    public float moveForce = 25f;

    public static Vector3 playerPos; //position of player

    [SerializeField] private FixedJoystick joystick = null;

    private PlayerAnimation anim;


    void Start()
    {
        myBody = GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<PlayerAnimation>();

    }

    // Update is called once per frame
    void Update()
    {
        Move();

    }

    void Move()
    {
        //playerPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2, gameObject.transform.position.z);

        //MOVE CHARACTER

        myBody.velocity = new Vector3(joystick.Horizontal * moveForce, myBody.velocity.y, joystick.Vertical * moveForce); //x, y, z

        //print(joystick.Horizontal);

        //Pressing the joystick at one value at least
        if (joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            anim.Move(0.6f); //Run state

            // Debug.Log("Joystick Horizontal: " + joystick.Horizontal);
            // Debug.Log("Joystick Vertical: " + joystick.Vertical);

            //Makes gameobject look towards direction specified, in this case, the movement direction of the player
            transform.rotation = Quaternion.LookRotation(myBody.velocity);
        }

        else
        {
            anim.Move(0f); //Idle state   
        }
    }


}