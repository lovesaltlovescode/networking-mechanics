using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Mirror;

public class Temp_CreateAndSendOrder : NetworkBehaviour
{
    [SerializeField] private Transform UISpawnPos;

    private ChickenRice currentOrder;
    private GameObject orderIcon;

    public void CreateOrder()
    {

        if (!hasAuthority)
        {
            return;
        }

        if (currentOrder != null)
        {
            CmdRemoveOrder();
        }


        CmdCreateOrder();

        

    }

    [Command]
    public void CmdCreateOrder()
    {
        currentOrder = OrderGeneration.Instance.CreateNewOrder();
        Debug.Log(currentOrder.ChickenRiceLabel);

        orderIcon = Instantiate(currentOrder.OrderIcon, UISpawnPos.position, UISpawnPos.rotation);
        NetworkServer.Spawn(orderIcon);
        Debug.Log(orderIcon.name);
    }

    [Command]
    public void CmdRemoveOrder()
    {
        NetworkServer.Destroy(orderIcon.gameObject);
        Debug.Log("ordericon has been destroyed");

        currentOrder = null;
        Debug.Log("current order is now null");
    }


    public void SendOrder()
    {
        if (currentOrder != null)
        {
            
            CmdRemoveOrder();
        }
    }

    [Command]
    public void CmdSendOrder()
    {
        Temp_VRSpawnOrder.Instance.AddOrderToList(currentOrder.RoastedChic, currentOrder.RicePlain, currentOrder.HaveEgg);
    }
}