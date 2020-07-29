using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public Rigidbody controllerBody;
    public float moveForce = 10f;

    [SerializeField] private FixedJoystick joystick = null;

    //A list of all active players in the game scene
    //To be accessed in the scripts that require specific players
    public static List<GameObject> ActivePlayers = new List<GameObject>();

    //A list of all active players' ID in the game scene
    public static List<string> PlayerIDs = new List<string>();

    //movement UI canvas, toggle on off for clients
    public GameObject UI;


    //    #region Platform Specifics

    //#if UNITY_EDITOR

    //    public void Awake()
    //    {
    //        Debug.Log("In editor!");
    //        //movementUI.enabled = false;

    //    }

    //#elif UNITY_STANDALONE_WIN

    //    public void Awake()
    //    {
    //        //Debug.Log("Standalone windows!");
    //    }


    //#elif UNITY_ANDROID

    //    public void Awake()
    //    {
    //        Debug.Log("On android!");
    //    }
    //#endif

    //#endregion

    private void Start()
    {
        ActivePlayers.Add(gameObject);
        PlayerIDs.Add(gameObject.GetComponent<NetworkIdentity>().netId.ToString());

        for (int i = 0; i < ActivePlayers.Count; i++)
        {
            Debug.Log($"There are {ActivePlayers.Count} active players");
            Debug.Log($"Player Movement - Player {i} is {ActivePlayers[i]}");
            Debug.Log($"Player Movement - Player {i}'s Net ID is {PlayerIDs[i]}");
        }
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
            //Debug.Log("Player does not have authority");
            return;
        }

        controllerBody.velocity = new Vector3(joystick.Horizontal * moveForce, controllerBody.velocity.y, joystick.Vertical * moveForce);

        //print(joystick.Horizontal);


        UI.gameObject.SetActive(true);

        if (joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            transform.rotation = Quaternion.LookRotation(controllerBody.velocity);
        }
    }

}
