using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VR_ArmSwinger : MonoBehaviour
{
    //variables
    [SerializeField] public InputDevice inputDeviceForXR;
    [SerializeField] private GameObject leftHand, rightHand;
    [SerializeField] private GameObject vrRig, vrCamera;
    [SerializeField] private Rigidbody vrRig_Rigidbody;

    [SerializeField] private Transform forwardDirectionTransform;
    [SerializeField] private float movementSpeed = 35f;
    private Vector3 newPosition;

    [Header("Boundaries")]
    [SerializeField] GameObject vrRig_Boundaries;
    private float minX, maxX, minZ, maxZ;

    [HideInInspector] public bool vrCanMove = true;

    private Vector3 position_PrevFrame_Rig, position_PrevFrame_LeftHand, position_PrevFrame_RightHand;
    private Vector3 position_CurrFrame_Rig, position_CurrFrame_LeftHand, position_CurrFrame_RightHand;

    private void Awake()
    {
        if (vrRig_Rigidbody == null)
        {
            vrRig_Rigidbody = vrRig.GetComponent<Rigidbody>();
        }

        GetBoundaries(vrRig_Boundaries);
    }

    private void Start()
    {
        UpdatePrevFrameVar();
    }

    private void Update()
    {
        if (vrCanMove)
        {
            UpdateCurrFrameVar();

            newPosition = ClampValues(vrRig.transform.position + MoveAmt());
        }

        UpdatePrevFrameVar(vrCanMove);
    }

    private void FixedUpdate()
    {
        if (vrCanMove)
        {
            MovePlayer(newPosition);
        }
    }


    //------------------------FORMULAE CALCULATING MOVEMENT-----------------------------
    private void MovePlayer(Vector3 positionToMoveTo)
    {
        vrRig_Rigidbody.MovePosition(positionToMoveTo);
    }
    //gets the boundaries of the player
    private void GetBoundaries(GameObject boundaryObj)
    {
        Bounds boundaryBounds = boundaryObj.GetComponent<Collider>().bounds;

        //get min and max x positions
        minX = boundaryBounds.min.x;
        maxX = boundaryBounds.max.x;

        //get min and max z positions
        minZ = boundaryBounds.min.z;
        maxZ = boundaryBounds.max.z;
    }

    private Vector3 ClampValues(Vector3 newPos)
    {
        Vector3 clampedPos = newPos;
        clampedPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        clampedPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

        return clampedPos;
    }

    //calculates the amount to move
    private Vector3 MoveAmt()
    {
        return GetForwardDirection().forward * HandSpeed() * movementSpeed * Time.deltaTime;
    }

    //calculates the speed of the movement of hands
    private float HandSpeed()
    {
        float distanceMoved_Player = Vector3.Distance(position_CurrFrame_Rig, position_PrevFrame_Rig);
        float distanceMoved_LeftHand = Vector3.Distance(position_CurrFrame_LeftHand, position_PrevFrame_LeftHand);
        float distanceMoved_RightHand = Vector3.Distance(position_CurrFrame_RightHand, position_PrevFrame_RightHand);

        return distanceMoved_LeftHand + distanceMoved_RightHand - 2 * distanceMoved_Player;
    }



    //----------------------RETURN VALUES-----------------------------------
    private Vector3 GetPos(GameObject obj)
    {
        return obj.transform.position;
    }

    private float GetYRotation(Transform source)
    {
        return source.eulerAngles.y;
    }

    private Transform GetForwardDirection()
    {
        forwardDirectionTransform.eulerAngles = new Vector3(0, GetYRotation(vrCamera.transform), 0);

        return forwardDirectionTransform.transform;
    }




    //---------------------UPDATE POSITIONS IN FRAMES-------------------------
    private void UpdatePrevFrameVar(bool updateRig = true)
    {
        position_PrevFrame_LeftHand = GetPos(leftHand);
        position_PrevFrame_RightHand = GetPos(rightHand);

        if(updateRig)
            position_PrevFrame_Rig = GetPos(vrRig);
    }

    private void UpdateCurrFrameVar()
    {
        position_CurrFrame_LeftHand = GetPos(leftHand);
        position_CurrFrame_RightHand = GetPos(rightHand);
        position_CurrFrame_Rig = GetPos(vrRig);
    }


}
