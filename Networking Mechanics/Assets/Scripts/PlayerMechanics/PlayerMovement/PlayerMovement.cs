using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody myBody;
    public float moveForce = 25f;

    [SerializeField] private FixedJoystick joystick = null;
    
    //private player animation
    public Canvas movementUI;

    #region

#if UNITY_EDITOR

    public void Awake()
    {
        Debug.Log("In editor!");
        movementUI.enabled = false;

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
        myBody = GetComponent<Rigidbody>();
        
        //TODO: get button component and call function to interact

    }

    // Update is called once per frame
    void Update()
    {

        if (!hasAuthority)
        {
            movementUI.enabled = false;
            return;
        }

        myBody.velocity = new Vector3(joystick.Horizontal * moveForce, myBody.velocity.y, joystick.Vertical * moveForce);

        print(joystick.Horizontal);

        if(joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            transform.rotation = Quaternion.LookRotation(myBody.velocity);
        }

    }

    //TODO: player interact function
}
