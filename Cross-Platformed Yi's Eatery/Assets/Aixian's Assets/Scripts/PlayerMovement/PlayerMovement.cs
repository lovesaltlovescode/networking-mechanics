using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public Rigidbody controllerBody;
    public float moveForce = 8f;

    [SerializeField] private FixedJoystick joystick = null;

    //A list of all active players in the game scene
    //To be accessed in the scripts that require specific players
    public static List<GameObject> ActivePlayers = new List<GameObject>();

    //A list of all active players' ID in the game scene
    public static List<string> PlayerIDs = new List<string>();

    [SerializeField] private Vector3 playerStartPos;
    [SerializeField] private Quaternion playerStartRot;

    //movement UI canvas, toggle on off for clients
    public GameObject UI;

    [SerializeField] private ServerAnimationManager serverAnimationScript;
    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    private void Awake()
    {
        serverAnimationScript = GetComponent<ServerAnimationManager>();
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        playerStartPos = gameObject.transform.position;
        playerStartRot = gameObject.transform.rotation;
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
        if (!hasAuthority)
        {
            //Debug.Log("Player does not have authority");
            return;
        }

        MovePlayer();

        if (LevelTimer.Instance.hasLevelEnded)
        {
            gameObject.transform.position = playerStartPos;
            gameObject.transform.rotation = playerStartRot;
            return;
        }

        if(controllerBody.velocity.magnitude <= 0)
        {
            serverAnimationScript.StopRunAnim();
        }
    }

    //function to move the player
    void MovePlayer()
    {

        if (!hasAuthority || !LevelTimer.Instance.levelStarted)
        {
            //Debug.Log("Player does not have authority");
            return;
        }

        controllerBody.velocity = new Vector3(joystick.Horizontal * moveForce, controllerBody.velocity.y, joystick.Vertical * moveForce);


        UI.gameObject.SetActive(true);

        if (joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            transform.rotation = Quaternion.LookRotation(controllerBody.velocity);
        }

        if (networkedPlayerInteraction.heldItem != HeldItem.nothing)
        {
            serverAnimationScript.GrabRunAnim();
        }
        else
        {
            serverAnimationScript.RunAnim();
        }
    }

}
