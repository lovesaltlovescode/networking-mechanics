using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

/// <summary>
/// Script to get the camera to follow the player
/// Each camera will have its own tag depending on which player it should follow
/// Based on tag, camera will follow the different active players
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    [SerializeField] private Transform followPlayerTrans;



    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();

    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayerTrans != null)
        {
            return;
        }
        else
        {
            LocatePlayer();
        }
    }

    //to locate the player to follow
    void LocatePlayer()
    {
        //depending on which player has client authority ie. who the local player is
        //the camera will follow that player on each device
        if (PlayerMovement.ActivePlayers.Count >= 1)
        {

            for(int i=0; i < PlayerMovement.ActivePlayers.Count; i++)
            {
                if (PlayerMovement.ActivePlayers[i].GetComponent<NetworkIdentity>().hasAuthority)
                {
                    followPlayerTrans = PlayerMovement.ActivePlayers[i].GetComponent<Transform>();
                    Debug.Log("CameraFollow - Looking for player" + PlayerMovement.ActivePlayers[i]);
                    vCam.Follow = followPlayerTrans;
                    vCam.LookAt = followPlayerTrans;
                    return;
                }
            }

            //if (PlayerMovement.ActivePlayers[0].GetComponent<NetworkIdentity>().hasAuthority)
            //{
            //    followPlayerTrans = PlayerMovement.ActivePlayers[0].GetComponent<Transform>();
            //    Debug.Log("CameraFollow - Looking for player" + PlayerMovement.ActivePlayers[0]);
            //    vCam.Follow = followPlayerTrans;
            //    vCam.LookAt = followPlayerTrans;
            //}

            //if (PlayerMovement.ActivePlayers[1].GetComponent<NetworkIdentity>().hasAuthority)
            //{
            //    followPlayerTrans = PlayerMovement.ActivePlayers[1].GetComponent<Transform>();
            //    Debug.Log("CameraFollow - Looking for player" + PlayerMovement.ActivePlayers[1]);
            //    vCam.Follow = followPlayerTrans;
            //    vCam.LookAt = followPlayerTrans;
            //}

            //TODO: ADD IN THE OTHER PLAYERS' CAMERA FOLLOW SCRIPTS
        }









    }
}
