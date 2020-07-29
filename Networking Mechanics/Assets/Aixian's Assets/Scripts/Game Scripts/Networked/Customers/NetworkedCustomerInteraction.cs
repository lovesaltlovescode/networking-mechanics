using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedCustomerInteraction : NetworkBehaviour
{
    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    #region Remote Methods
    //Pick up customer
    public void PickUpCustomer()
    {
        Debug.Log("Picked up customer");
        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);
        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.customer);

        //if inventory full, change state to holding customer
        if (networkedPlayerInteraction.IsInventoryFull())
        {
            networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingCustomer);
        }

    }

    #endregion
}
