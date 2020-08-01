using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

/// <summary>
/// Customer being held on top of player's head
/// get the group size num variable from the customerr
/// </summary>

public class CustomerBehaviour_BeingHeld : NetworkBehaviour
{
    [Header("Group Size Icon Variables")]
    [SerializeField] private TextMeshProUGUI groupSizeText;
    [SerializeField] private GameObject groupSizeIcon;

    //group size variables
    [SyncVar]
    public int groupSizeNum;

    // Start is called before the first frame update
    void Start()
    {
        groupSizeNum = GetComponentInParent<NetworkedCustomerInteraction>().customerGroupSize;
        HeldCustomer_ShowGroupSizeIcon(groupSizeNum);
    }


    //TODO: How to prevent if more than one player is holding customer
    //How to sync group size number
    [ServerCallback]
    public void FindPlayerPickingUpCustomer()
    {
        for(int i = 0; i <  PlayerMovement.ActivePlayers.Count; i++)
        {
            if(PlayerMovement.ActivePlayers[i].GetComponent<NetworkedPlayerInteraction>().detectedObject &&
                PlayerMovement.ActivePlayers[i].GetComponent<NetworkedPlayerInteraction>().detectedObject.tag == "Customer")
            {
                groupSizeNum = PlayerMovement.ActivePlayers[i].GetComponent<NetworkedPlayerInteraction>().detectedObject.GetComponent<CustomerBehaviour_Queueing>().groupSizeNum;

                RpcUpdateGroupSizeNum(groupSizeNum);
                return;
            }
            else
            {
                return;
            }
        }
    }

    [ClientRpc]
    public void RpcUpdateGroupSizeNum(int updatedSize)
    {
        groupSizeNum = updatedSize;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("CustomerBehaviour_BeingHeld - Group size: " + groupSizeNum);
    }

    //display the number of people in the group of customer queueing 
    public void HeldCustomer_ShowGroupSizeIcon(int groupSize)
    {
        //update the group size indicator to display the groupsize
        groupSizeText.text = groupSize.ToString();

        //enable the group icon display in the hierarchy
        HeldCustomer_EnableGroupSizeIcon(true);
    }

    //disable / enable the groupsizeIcon in the hierarchy
    public void HeldCustomer_EnableGroupSizeIcon(bool shouldEnable)
    {
        //set the group size icon false by default
        groupSizeIcon.SetActive(shouldEnable);
    }
}
