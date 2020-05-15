using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementTest : MonoBehaviour
{
    private Rigidbody myBody;
    public float moveForce = 25f;

    [SerializeField] private FixedJoystick joystick = null;
    

    void Start()
    {
        myBody = GetComponent<Rigidbody>();
        
        //TODO: get button component and call function to interact

    }

    // Update is called once per frame
    void Update()
    {

        myBody.velocity = new Vector3(joystick.Horizontal * moveForce, myBody.velocity.y, joystick.Vertical * moveForce);

        print(joystick.Horizontal);

        if(joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            transform.rotation = Quaternion.LookRotation(myBody.velocity);
        }

    }

    //TODO: player interact function
}
