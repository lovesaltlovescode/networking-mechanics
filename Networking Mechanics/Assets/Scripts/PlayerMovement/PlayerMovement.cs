using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public Rigidbody xiaoBenBody;
    public float moveForce = 10f;

    [SerializeField] private FixedJoystick joystick = null;
    
    //movement UI canvas, toggle on off for clients
    public Canvas movementUI;


    #region Platform Specifics

#if UNITY_EDITOR

    public void Awake()
    {
        Debug.Log("In editor!");
        //movementUI.enabled = false;

    }

#elif UNITY_STANDALONE_WIN

    public void Awake()
    {
        //Debug.Log("Standalone windows!");
    }


#elif UNITY_ANDROID

    public void Awake()
    {
        Debug.Log("On android!");
    }
#endif

#endregion

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        MovePlayer();
    }

    //function to move the player
    void MovePlayer()
    {

        if (!hasAuthority)
        {
            Debug.Log("Player does not have authority");
            //movementUI.enabled = false;
            return;
        }

        xiaoBenBody.velocity = new Vector3(joystick.Horizontal * moveForce, xiaoBenBody.velocity.y, joystick.Vertical * moveForce);

        //print(joystick.Horizontal);

        if (joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            transform.rotation = Quaternion.LookRotation(xiaoBenBody.velocity);
        }
    }

}
