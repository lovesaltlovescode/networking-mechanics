using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{

    protected Joystick joystick;
    protected JoyButton joybutton;

    //Control direction
    public float moveSpeed;
    public float rotateSpeed;

    private Rigidbody rb;
    private Vector3 direction;

    //temp bool for jump
    protected bool jump;

    //Reference to UI 
    [SerializeField] private Canvas movementUI;


    // Start is called before the first frame update
    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<JoyButton>();

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        JoyStick();
    }

    void JoyStick()
    {

        if (!hasAuthority)
        {
            movementUI.enabled = false;
            return;
            
        }

        //Allows object to move according to joystick input
        rb.velocity = new Vector3(joystick.Horizontal * 50f,
            rb.velocity.y, joystick.Vertical * 50f);

        //rotates the object according to their current x, with the tangent of angle f (converted to deg), and the current z) 
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(joystick.Horizontal * 50f, joystick.Vertical * 50f) * Mathf.Rad2Deg, transform.eulerAngles.z);

        if (!jump && joybutton.pressed)
        {
            jump = true;
            rb.velocity = Vector3.up * 10f;
            //Jump

        }
        if (jump && !joybutton.pressed)
        {
            jump = false;
        }
    }

}
