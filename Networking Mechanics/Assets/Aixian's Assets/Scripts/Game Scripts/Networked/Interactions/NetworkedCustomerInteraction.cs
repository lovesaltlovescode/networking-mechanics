using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedCustomerInteraction : NetworkBehaviour
{
    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    public int customerGroupSize;

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    #region Remote Methods
    //Pick up customer
    public void PickUpCustomer()
    {
        //CmdPickUpCustomer(networkedPlayerInteraction.detectedObject);
        //CmdChangeHeldCustomer(HeldItem.customer);

        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.customer);
        //CmdPickUpCustomer();
        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);
        //CmdChangeHeldCustomer();

        if (isServer)
        {
            RpcPickUpCustomer();
        }

        //if inventory full, change state to holding customer
        if (networkedPlayerInteraction.IsInventoryFull())
        {
            networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingCustomer);
        }

    }

    //Seat customer
    public void SeatCustomer()
    {
        Debug.Log("NetworkedCustomerInteraction - Seat customer");

        CmdSeatCustomer(networkedPlayerInteraction.detectedObject, networkedPlayerInteraction.playerInventory);

    }


    #region Commands

    [Command]
    public void CmdPickUpCustomer()
    {
        customerGroupSize = networkedPlayerInteraction.detectedObject.GetComponent<CustomerBehaviour_Queueing>().groupSizeNum;
    }

    [Command]
    public void CmdChangeHeldCustomer()
    {
        networkedPlayerInteraction.heldItem = HeldItem.customer;
        var heldCustomerSize = networkedPlayerInteraction.customer.GetComponent<CustomerBehaviour_BeingHeld>().groupSizeNum;
        Debug.Log("Customer instantiated -" + heldCustomerSize);

        var customerSize = customerGroupSize;
        Debug.Log("Customer initial size: " + customerSize);

        heldCustomerSize = customerSize;
        Debug.Log("Held customer size: " + heldCustomerSize);
    }


    [ClientRpc]
    public void RpcPickUpCustomer()
    {
        TableColliderManager.ToggleTableDetection(true);
    }


    #endregion

    [Command]
    public void CmdSeatCustomer(GameObject detectedObject, GameObject playerInventory)
    {
        Debug.Log("NetworkedCustomerInteraction - CmdSeatCustomer");
        Debug.Log("NetworkedCustomerInteraction - Detected object: " + detectedObject.tag);

        //check if detected object is table
        if (!detectedObject.GetComponent<TableScript>())
        {
            Debug.Log("NetworkedCustomerInteraction- Player is not looking at a table");
            return;
        }

        

        //get table's table script
        TableScript tableScript = detectedObject.GetComponent<TableScript>();
        Debug.Log(tableScript);

        var heldCustomer = networkedPlayerInteraction.attachmentPoint.transform.GetChild(0);

        //if table has enough seats
        if (tableScript.CheckSufficientSeats(heldCustomer.GetComponent<CustomerBehaviour_BeingHeld>().groupSizeNum))
        {
            Debug.Log("NetworkedCustomerInteraction - Enough seats for customers");
            RpcSeatCustomer();

            //remove from inventory
            playerInventory = null;

            networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing); //stop holding customer
        }
    }

    [ClientRpc]
    public void RpcSeatCustomer()
    {
        TableColliderManager.ToggleTableDetection(false);
        networkedPlayerInteraction.ChangePlayerState(PlayerState.Default, true);
    }

    #endregion
}
