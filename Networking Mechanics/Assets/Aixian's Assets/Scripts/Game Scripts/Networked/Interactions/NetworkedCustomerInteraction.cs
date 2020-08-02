﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedCustomerInteraction : NetworkBehaviour
{
    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    [SyncVar]
    public int customerGroupSize;


    //Check if player has detected a plate
    public void DetectCustomer()
    {
        if (!hasAuthority)
        {
            return;
        }

        //if player is not holding anything
        if (!networkedPlayerInteraction.playerInventory)
        {
            //Debug.Log("NetworkedIngredientInteraction - Able to pick up plate!");

            if (networkedPlayerInteraction.detectedObject.tag == "Customer" && networkedPlayerInteraction.detectedObject.layer == LayerMask.NameToLayer("Queue"))
            {
                networkedPlayerInteraction.ChangePlayerState(PlayerState.CanPickUpCustomer);
            }
        }
    }

    //Check if player has detected a plate
    public void DetectDish()
    {
        if (!hasAuthority)
        {
            return;
        }

        //if player is not holding anything
        if (!networkedPlayerInteraction.playerInventory)
        {
            //Debug.Log("NetworkedIngredientInteraction - Able to pick up plate!");

            if(networkedPlayerInteraction.detectedObject.tag == "Dish")
            {
                networkedPlayerInteraction.playerState = PlayerState.CanPickUpDish;
            }
        }
    }



    //for now, spawn orders on keypress
    private void Update()
    {

        if (!hasAuthority)
        {
            return;
        }

        networkedPlayerInteraction.DetectObject(networkedPlayerInteraction.detectedObject, 19, DetectCustomer);
        networkedPlayerInteraction.DetectObject(networkedPlayerInteraction.detectedObject, 25, DetectDish);

        //SPAWN DISHES
        //get the input
        var input = Input.inputString;

        //ignore null input to avoid unnecessary computation
        if (!string.IsNullOrEmpty(input))
        {
            switch (input)
            {
                case "1":
                    Debug.Log("Spawn RoastedChicWRiceBall");
                    SpawnDish(1);
                    break;

                case "2":
                    Debug.Log("Spawn RoastedChicWPlainRice");
                    SpawnDish(2);
                    break;

                case "3":
                    Debug.Log("Spawn RoastedChicWRiceBallEgg");
                    SpawnDish(3);
                    break;

                case "4":
                    Debug.Log("Spawn RoastedChicWPlainRiceEgg");
                    SpawnDish(4);
                    break;

                case "5":
                    Debug.Log("Spawn SteamedChicWRiceBall");
                    SpawnDish(5);
                    break;

                case "6":
                    Debug.Log("Spawn SteamedChicWPlainRice");
                    SpawnDish(6);
                    break;

                case "7":
                    Debug.Log("Spawn SteamedChicWRiceBallEgg");
                    SpawnDish(7);
                    break;

                case "8":
                    Debug.Log("Spawn SteamedChicWPlainRiceEgg");
                    SpawnDish(8);
                    break;

            }
        }

    }

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    #region Remote Methods
    //Pick up customer
    public void PickUpCustomer()
    {
        //Get customer's group size
        CmdPickUpCustomer(networkedPlayerInteraction.detectedObject.GetComponent<CustomerBehaviour_Queueing>().groupSizeNum);

        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);

        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.customer);

        //networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingCustomer);


    }

    //Seat customer
    public void SeatCustomer()
    {
        Debug.Log("NetworkedCustomerInteraction - Seat customer");

        CmdSeatCustomer(networkedPlayerInteraction.detectedObject, networkedPlayerInteraction.playerInventory);

    }

    //Taking customers orders
    public void CheckHandsEmpty()
    {
        Debug.Log("NetworkedCustomerInteraction - CheckHandsEmpty");
        CmdCheckHandsEmpty(networkedPlayerInteraction.detectedObject, networkedPlayerInteraction.IsInventoryFull());
    }

    //Spawning dishes
    [ServerCallback]
    public void SpawnDish(int dish)
    {
        ServerSpawnDish(dish);
    }

    //Picking up dishes
    public void PickUpDish()
    {
        networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingOrder);
        CmdCheckDish(networkedPlayerInteraction.detectedObject.GetComponentInChildren<OrderScript>().dishLabel, PlayerState.HoldingOrder);
        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);

        Debug.Log("Dish picked up: " + networkedPlayerInteraction.detectedObject.GetComponentInChildren<OrderScript>().dishLabel);
        
    }

    //Serving customers
    public void CheckCanPutDownOrder()
    {
        
        if (networkedPlayerInteraction.playerState != PlayerState.HoldingOrder)
        {
            Debug.Log("NetworkedCustomerInteraction - Player not holding a dish");
            return;
        }

        GameObject heldDish = networkedPlayerInteraction.playerInventory;
        Debug.Log("NetworkedCustomerInteraction - Held dish is " + heldDish.GetComponentInChildren<OrderScript>().dishLabel);

        //if there is a detectedobject
        if (networkedPlayerInteraction.detectedObject)
        {
            //if looking at customer
            if (networkedPlayerInteraction.detectedObject.GetComponent<CustomerBehaviour_Seated>())
            {
                Debug.Log("NetworkedCustomerInteraction - Looking at customer");
                if(ServingCustomer(heldDish, networkedPlayerInteraction.detectedObject))
                {
                    networkedPlayerInteraction.playerInventory = null;
                    networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing);
                    Debug.Log("Is inventory full:" + networkedPlayerInteraction.IsInventoryFull());
                    networkedPlayerInteraction.ChangePlayerState(PlayerState.Default, true);
                }

            }
            else
            {
                Debug.Log("NetworkedCustomerInteraction - Not looking at customer that can be served");
            }
        }
    }

    //check if the order is correct if the player is facing a customer
    public bool ServingCustomer(GameObject dishObj, GameObject customer)
    {
        //if the gameobj the player is looking at is indeed a customer,
        if (customer.GetComponent<CustomerBehaviour_Seated>())
        {
            //if the order being served is what the customer wanted,
            Debug.Log("NetworkedCustomerInteraction - Serve order");
            return customer.GetComponent<CustomerBehaviour_Seated>().CheckOrder(dishObj);

        }

        return false;

    }//end serve method

    #endregion

    #region Commands

    #region Pick Up Customers

    [Command]
    public void CmdPickUpCustomer(int groupSize)
    {
        customerGroupSize = groupSize;
        RpcPickUpCustomer();
    }

    [ClientRpc]
    public void RpcPickUpCustomer()
    {
        networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingCustomer);
        TableColliderManager.ToggleTableDetection(true);
    }

    #endregion

    #region Seat Customers

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
            RpcSeatCustomer(playerInventory);
        }
        else
        {
            Debug.Log("Not enough seats");

        }
    }

    [ClientRpc]
    public void RpcSeatCustomer(GameObject playerInventory)
    {
        TableColliderManager.ToggleTableDetection(false);

        //remove from inventory
        playerInventory = null;

        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing); //stop holding customer
        networkedPlayerInteraction.ChangePlayerState(PlayerState.Default, true);
    }

    #endregion

    #region Take Orders

    [Command]
    //check hands empty
    public void CmdCheckHandsEmpty(GameObject detectedObject, bool inventoryFull)
    {
        //check if player is looking at table
        if (!detectedObject.GetComponent<TableScript>())
        {
            Debug.Log("NetworkedCustomerInteraction- Player is not looking at a table");
            return;
        }

        //get table script
        TableScript tableScript = detectedObject.GetComponent<TableScript>();

        //if player's hands are full, do not take order
        if (inventoryFull)
        {
            Debug.Log("NetworkedCustomerInteraction- Player's hands are full, do not take order");
            tableScript.TableFeedbackScript.HandsFullFeedback();
            return;
        }

        RpcCheckHandsEmpty(detectedObject);
        ////Else, take order of customers at table
        //tableScript.TakeOrder();
        //networkedPlayerInteraction.ChangePlayerState(PlayerState.Default);
    }

    [ClientRpc]
    public void RpcCheckHandsEmpty(GameObject detectedObject)
    {
        //Else, take order of customers at table
        detectedObject.GetComponent<TableScript>().ServerTakeOrder();
        networkedPlayerInteraction.ChangePlayerState(PlayerState.Default);
    }


    #endregion

    #region Spawn Dishes

    [ServerCallback]
    public void ServerSpawnDish(int dish)
    {
        //spawn dish
        
        //spawn in the right positions
        for(int i = 0; i < GameManager.Instance.dishPositions.Length; i++)
        {
            if(GameManager.Instance.dishesOnCounter[i] == null)
            {
                //spawn according to which number is pressed
                switch (dish)
                {
                    case 1:
                        var spawnedDish = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishPositions[i].position, Quaternion.identity, HeldItem.roastedChicWRiceBall, "Dish");
                        GameManager.Instance.dishesOnCounter[i] = spawnedDish;
                        break;

                    case 2:
                        var spawnedDish2 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishPositions[i].position, Quaternion.identity, HeldItem.roastedChicWPlainRice, "Dish");
                        GameManager.Instance.dishesOnCounter[i] = spawnedDish2;
                        break;

                    case 3:
                        var spawnedDish3 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishPositions[i].position, Quaternion.identity, HeldItem.roastedChicWRiceBallEgg, "Dish");
                        GameManager.Instance.dishesOnCounter[i] = spawnedDish3;
                        break;

                    case 4:
                        var spawnedDish4 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishPositions[i].position, Quaternion.identity, HeldItem.roastedChicWPlainRiceEgg, "Dish");
                        GameManager.Instance.dishesOnCounter[i] = spawnedDish4;
                        break;

                    case 5:
                        var spawnedDish5 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishPositions[i].position, Quaternion.identity, HeldItem.steamedChicWRiceBall, "Dish");
                        GameManager.Instance.dishesOnCounter[i] = spawnedDish5;
                        break;

                    case 6:
                        var spawnedDish6 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishPositions[i].position, Quaternion.identity, HeldItem.steamedChicWPlainRice, "Dish");
                        GameManager.Instance.dishesOnCounter[i] = spawnedDish6;
                        break;

                    case 7:
                        var spawnedDish7 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishPositions[i].position, Quaternion.identity, HeldItem.steamedChicWRiceBallEgg, "Dish");
                        GameManager.Instance.dishesOnCounter[i] = spawnedDish7;
                        break;

                    case 8:
                        var spawnedDish8 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishPositions[i].position, Quaternion.identity, HeldItem.steamedChicWPlainRiceEgg, "Dish");
                        GameManager.Instance.dishesOnCounter[i] = spawnedDish8;
                        break;
                }

                //GameManager.Instance.dishCount += 1;
                return;
            }
            
        }
        
    }


    #endregion

    #region Pick Up Dishes

    //check dish label
    [Command]
    public void CmdCheckDish(ChickenRice.PossibleChickenRiceLabel chickenRiceLabel, PlayerState playerState)
    {

        RpcCheckDish(chickenRiceLabel, playerState);
        
    }

    [ClientRpc]
    public void RpcCheckDish(ChickenRice.PossibleChickenRiceLabel chickenRiceLabel, PlayerState playerState)
    {

        switch (chickenRiceLabel)
        {
            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBall:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.roastedChicWRiceBall);
                networkedPlayerInteraction.ChangePlayerState(playerState);
                break;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRice:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.roastedChicWPlainRice);
                networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingOrder);
                break;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBallEgg:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.roastedChicWRiceBallEgg);
                networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingOrder);
                break;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRiceEgg:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.roastedChicWPlainRiceEgg);
                networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingOrder);
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBall:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.steamedChicWRiceBall);
                networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingOrder);
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBallEgg:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.steamedChicWRiceBallEgg);
                networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingOrder);
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRiceEgg:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.steamedChicWPlainRiceEgg);
                networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingOrder);
                break;
        }

    }

    #endregion

    #region Serving Customer Orders

    [Command]
    public void CmdCheckCanPutDownOrder(GameObject detectedObject, GameObject playerInventory, PlayerState playerState)
    {
        
    }

    #endregion
    #endregion

}
