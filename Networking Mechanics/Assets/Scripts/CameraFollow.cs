using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Script to get the camera to follow the player
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    [SerializeField] private Transform followPlayerTrans;

    // Start is called before the first frame update
    void Start()
    {
        

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
            vCam = GetComponent<CinemachineVirtualCamera>();

            followPlayerTrans = GameObject.Find("Player(Clone)").GetComponent<Transform>();
            Debug.Log("CameraFollow - Finding player");
            vCam.LookAt = followPlayerTrans;
            vCam.Follow = followPlayerTrans;
            return;
        }
    }
}
