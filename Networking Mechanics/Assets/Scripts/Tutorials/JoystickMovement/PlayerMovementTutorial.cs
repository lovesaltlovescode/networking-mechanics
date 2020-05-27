using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;


public class PlayerMovementTutorial : MonoBehaviour
{
    private Rigidbody myBody;
    public float moveSpeed = 10f;

    [SerializeField] private FixedJoystick joystick = null;

    private Animator anim;


    void Start()
    {
        myBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        Move();

    }

    private void Move()
    {

        //MOVE CHARACTER

        myBody.velocity = new Vector3(joystick.Horizontal * moveSpeed,
            myBody.velocity.y, joystick.Vertical * moveSpeed); //x, y, z

        //print(joystick.Horizontal);

        //Pressing the joystick at one value at least
        if (joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            BlendRun(0.6f); //Run state

            //Makes gameobject look towards direction specified, 
            //in this case, the movement direction of the player
            transform.rotation = Quaternion.LookRotation(myBody.velocity);
        }

        else
        {
            BlendRun(0f); //Idle state   
        }
    }

    //function to make player run (blend tree)
    public void BlendRun(float blend)
    {
        anim.SetFloat("Blend", blend);
    }



}