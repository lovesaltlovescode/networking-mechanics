using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedCustomerInteraction : NetworkBehaviour
{
    [SerializeField] private NetworkedPlayerInteraction networkedPlayerInteraction;

    [SyncVar]
    public int customerGroupSize;

    [SyncVar]
    public float customerLastPatience;

    public CustomerWaitArea CustomerWaitAreaManager;
    public bool isPlayerInWaitArea = false; // set to true when the player enters the customer wait area

    [SyncVar]
    public float customerQueueingPatience;
    [SerializeField] private string gain20Points = "+20", gain10Points = "+10", gain5Points = "+5";

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    #region Detect methods

    //Check if player has detected a customer
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

    //Check if player has detected a dish
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

            if (networkedPlayerInteraction.detectedObject.tag == "Dish")
            {

                networkedPlayerInteraction.ChangePlayerState(PlayerState.CanPickUpDish);
            }
        }
    }

    #endregion

    #region Pick Up Customers

    //pick up customer
    public void PickUpCustomer()
    {
        //Get customer's group size
        CmdPickUpCustomer(networkedPlayerInteraction.detectedObject.GetComponent<CustomerBehaviour_Queueing>().groupSizeNum
            ,networkedPlayerInteraction.detectedObject.GetComponent<CustomerPatience>().currentPatience, networkedPlayerInteraction.detectedObject);

        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);
        //Destroy(networkedPlayerInteraction.detectedObject);

        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.customer);

        networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingCustomer);
        
        //allow tables and wait area to be detected
        ToggleWaitAreaAndTableDetection(true);



    }
    
    #region Networked

    [Command]
    public void CmdPickUpCustomer(int groupSize, float lastPatienceLevel, GameObject detectedObject)
    {
        customerGroupSize = groupSize;
        customerLastPatience = lastPatienceLevel;
        customerQueueingPatience = detectedObject.GetComponent<CustomerPatience>().currentPatience;
    }

    #endregion


    #endregion


    //toggle the detection of the wait area and table by raycast
    public void ToggleWaitAreaAndTableDetection(bool setDetectable)
    {
        //toggle table detection
        TableColliderManager.ToggleTableDetection(setDetectable);

        //toggle wait area detection
        CustomerWaitAreaManager.ToggleWaitAreaDetection(setDetectable);
    }


    #region Seat Customers

    //remove the beingHeld customer from inventory

    public void RemoveCustomerFromInventory()
    {
        //Debug.Log("Remove player inventory " + playerInventory.name);

        //remove the customer from the inventory
        //playerInventory = null;

        //stop holding the customer
        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing);

        networkedPlayerInteraction.ChangePlayerState(PlayerState.Default, true);
    }

    //checks whether the player is looking at a table / wait area / is in the wait area
    //called when the player state is HoldingCustomer
    public void CheckCanPutCustomerDown()
    {
        if (networkedPlayerInteraction.playerInventory.GetComponent<CustomerBehaviour_BeingHeld>() == null) //check whether the player is holding a customer
        {
            Debug.Log("player is not holding customer??");
            return;
        }

        //check if the player is looking at anything
        if (networkedPlayerInteraction.detectedObject != null)
        {

            //if the player is looking at a table
            if (networkedPlayerInteraction.detectedObject.GetComponent<TableScript>()) 
            {
                Debug.Log("player is looking at table");
                SeatCustomer(networkedPlayerInteraction.playerInventory, networkedPlayerInteraction.detectedObject);

            }

            //if the player is looking at the waiting area
            else if (networkedPlayerInteraction.detectedObject.GetComponent<CustomerWaitArea>() || isPlayerInWaitArea)
            {
                Debug.Log("player is looking at customer wait area");

                Debug.Log("CMD Player inventory " + networkedPlayerInteraction.playerInventory);
                PlaceCustomerDown(networkedPlayerInteraction.playerInventory);
                
            }
        }
        else if (isPlayerInWaitArea) //if the player is in the waiting area
        {
            Debug.Log("player is in customer wait area");

            Debug.Log("CMD Player inventory " + networkedPlayerInteraction.playerInventory);
            PlaceCustomerDown(networkedPlayerInteraction.playerInventory);
            
        }
    }

    public void PlaceCustomerDown(GameObject playerInventory)
    {

        Debug.Log("Local player inventory " + playerInventory);

        CmdPlaceCustomerDown(networkedPlayerInteraction.playerInventory);

        RemoveCustomerFromInventory();
    }


    [Command]
    public void CmdPlaceCustomerDown(GameObject playerInventory)
    {
        Debug.Log("CMD Inside Inventory" + networkedPlayerInteraction.playerInventory);


        CustomerWaitAreaManager.PutCustomerdown(networkedPlayerInteraction.playerInventory);

        RpcPlaceCustomerDown();

    }

    [ClientRpc]
    public void RpcPlaceCustomerDown()
    {
        //set the tables and wait area to undetectable
        ToggleWaitAreaAndTableDetection(false);
    }


    //Seat customer
    public void SeatCustomer(GameObject _playerInventory, GameObject _tableGameObj)
    {
        Debug.Log("Seat customer method called");

        //check if detected object is table
        if (!networkedPlayerInteraction.detectedObject.GetComponent<TableScript>())
        {
            
            return;
        }

        // Debug.Log("NetworkedCustomerInteraction - Seat customer");

        CmdSeatCustomer(_tableGameObj, _playerInventory);

    }

    //help
    #region Networked


    [Command]
    public void CmdSeatCustomer(GameObject detectedObject, GameObject playerInventory)
    {
       // Debug.Log("NetworkedCustomerInteraction - CmdSeatCustomer");
      //  Debug.Log("NetworkedCustomerInteraction - Detected object: " + detectedObject.tag);

        //get table's table script
        TableScript tableScript = detectedObject.GetComponent<TableScript>();

        var heldCustomer = networkedPlayerInteraction.playerInventory;

        //if table has enough seats
        if (tableScript.CheckSufficientSeats(heldCustomer.GetComponent<CustomerBehaviour_BeingHeld>().groupSizeNum))
        {
           // Debug.Log("NetworkedCustomerInteraction - Enough seats for customers");
            RpcSeatCustomer(playerInventory, detectedObject);

            //DECREASE
            GameManager.Instance.currentNumWaitingCustomers -= 1;
        }
        else
        {
           // Debug.Log("Not enough seats");

        }
    }

    [ClientRpc]
    public void RpcSeatCustomer(GameObject playerInventory, GameObject detectedObject)
    {
        if (!hasAuthority)
        {
            return;
        }

        RemoveCustomerFromInventory();

        GetQueueingCustomerPatience(detectedObject);
        

        //toggle layer undetectable
        ToggleWaitAreaAndTableDetection(false);

    }

    public void GetQueueingCustomerPatience(GameObject detectedObject)
    {
        //check queueing customer patience level when they were picked up
        float customerQueuedPatience = (customerQueueingPatience / CustomerPatienceStats.CustomerPatience_Queue) * 100;
        Debug.Log("Customer Queued Patience: " + customerQueuedPatience);

        if (customerQueuedPatience >= 50 && customerQueuedPatience > 0)
        {
            detectedObject.GetComponent<TableFeedback>().CustomerSeated(gain20Points);
            GameManager.Instance.AddServerScore(20);
            GameManager.Instance.IncrementMood(5);
        }
        else if (customerQueuedPatience >= 30 && customerQueuedPatience < 50)
        {
            detectedObject.GetComponent<TableFeedback>().CustomerSeated(gain10Points);
            GameManager.Instance.AddServerScore(10);
            GameManager.Instance.IncrementMood(2);
        }
        else if (customerQueuedPatience >= 20 || customerQueuedPatience < 30 && customerQueuedPatience > 0)
        {
            detectedObject.GetComponent<TableFeedback>().CustomerSeated(gain5Points);
            GameManager.Instance.AddServerScore(5);
        }
    }

    #endregion


    #endregion

    #region Take Orders

    //Taking customers orders
    public void CheckHandsEmpty()
    {
        //Debug.Log("NetworkedCustomerInteraction - CheckHandsEmpty");

        //check if player is looking at table
        if (!networkedPlayerInteraction.detectedObject.GetComponent<TableScript>())
        {
            //Debug.Log("NetworkedCustomerInteraction- Player is not looking at a table");
            return;
        }

        //get table script
        TableScript tableScript = networkedPlayerInteraction.detectedObject.GetComponent<TableScript>();

        CmdCheckHandsEmpty(networkedPlayerInteraction.detectedObject, networkedPlayerInteraction.IsInventoryFull());
        networkedPlayerInteraction.ChangePlayerState(PlayerState.Default);
    }

    #region Networked

    [Command]
    //check hands empty
    public void CmdCheckHandsEmpty(GameObject detectedObject, bool inventoryFull)
    {
        RpcCheckHandsEmpty(detectedObject);

    }

    [ClientRpc]
    public void RpcCheckHandsEmpty(GameObject detectedObject)
    {
        //Else, take order of customers at table
        detectedObject.GetComponent<TableScript>().ServerTakeOrder();

        //get current mood of customer and display points
        detectedObject.GetComponent<CustomerPatience>().CheckCustomerMood();
        detectedObject.GetComponent<TableFeedback>().CustomerOrderTaken();
    }

    #endregion

    #endregion

    #region Spawn Dishes

    ////Spawning dishes
    //[ServerCallback]
    //public void SpawnDish(int dish)
    //{
    //    ServerSpawnDish(dish);
    //}

    //[ServerCallback]
    //public void ServerSpawnDish(int dish)
    //{
    //    //spawn dish

    //    //spawn in the right positions
    //    for (int i = 0; i < GameManager.Instance.dishSpawnPoints.Length; i++)
    //    {
    //        if (GameManager.Instance.dishesOnCounter[i] == null)
    //        {
    //            //spawn according to which number is pressed
    //            switch (dish)
    //            {
    //                case 1:
    //                    var spawnedDish = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishSpawnPoints[i].position, Quaternion.identity, HeldItem.roastedChicWRiceBall, "Dish");
    //                    GameManager.Instance.dishesOnCounter[i] = spawnedDish;
    //                    break;

    //                case 2:
    //                    var spawnedDish2 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishSpawnPoints[i].position, Quaternion.identity, HeldItem.roastedChicWPlainRice, "Dish");
    //                    GameManager.Instance.dishesOnCounter[i] = spawnedDish2;
    //                    break;

    //                case 3:
    //                    var spawnedDish3 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishSpawnPoints[i].position, Quaternion.identity, HeldItem.roastedChicWRiceBallEgg, "Dish");
    //                    GameManager.Instance.dishesOnCounter[i] = spawnedDish3;
    //                    break;

    //                case 4:
    //                    var spawnedDish4 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishSpawnPoints[i].position, Quaternion.identity, HeldItem.roastedChicWPlainRiceEgg, "Dish");
    //                    GameManager.Instance.dishesOnCounter[i] = spawnedDish4;
    //                    break;

    //                case 5:
    //                    var spawnedDish5 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishSpawnPoints[i].position, Quaternion.identity, HeldItem.steamedChicWRiceBall, "Dish");
    //                    GameManager.Instance.dishesOnCounter[i] = spawnedDish5;
    //                    break;

    //                case 6:
    //                    var spawnedDish6 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishSpawnPoints[i].position, Quaternion.identity, HeldItem.steamedChicWPlainRice, "Dish");
    //                    GameManager.Instance.dishesOnCounter[i] = spawnedDish6;
    //                    break;

    //                case 7:
    //                    var spawnedDish7 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishSpawnPoints[i].position, Quaternion.identity, HeldItem.steamedChicWRiceBallEgg, "Dish");
    //                    GameManager.Instance.dishesOnCounter[i] = spawnedDish7;
    //                    break;

    //                case 8:
    //                    var spawnedDish8 = networkedPlayerInteraction.ServerSpawnObject(GameManager.Instance.dishSpawnPoints[i].position, Quaternion.identity, HeldItem.steamedChicWPlainRiceEgg, "Dish");
    //                    GameManager.Instance.dishesOnCounter[i] = spawnedDish8;
    //                    break;
    //            }

    //            //GameManager.Instance.dishCount += 1;
    //            return;
    //        }

    //    }

    //}


    #endregion

    #region Pick Up Dishes
    //Picking up dishes
    public void PickUpDish()
    {

        CheckDish(networkedPlayerInteraction.detectedObject.GetComponentInChildren<OrderScript>().dishLabel, PlayerState.HoldingOrder);
        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);

        ////Debug("Dish picked up: " + networkedPlayerInteraction.detectedObject.GetComponentInChildren<OrderScript>().dishLabel);

        if (networkedPlayerInteraction.playerInventory != null)
        {
            networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingOrder, true);
        }

    }

    #region Networked

    //check dish label

    public void CheckDish(ChickenRice.PossibleChickenRiceLabel chickenRiceLabel, PlayerState playerState)
    {

        switch (chickenRiceLabel)
        {
            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBall:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.roastedChicWRiceBall);
                networkedPlayerInteraction.ChangePlayerState(playerState);
                break;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRice:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.roastedChicWPlainRice);
                networkedPlayerInteraction.ChangePlayerState(playerState);
                break;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBallEgg:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.roastedChicWRiceBallEgg);
                networkedPlayerInteraction.ChangePlayerState(playerState);
                break;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRiceEgg:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.roastedChicWPlainRiceEgg);
                networkedPlayerInteraction.ChangePlayerState(playerState);
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBall:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.steamedChicWRiceBall);
                networkedPlayerInteraction.ChangePlayerState(playerState);
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRice:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.steamedChicWPlainRice);
                networkedPlayerInteraction.ChangePlayerState(playerState);
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBallEgg:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.steamedChicWRiceBallEgg);
                networkedPlayerInteraction.ChangePlayerState(playerState);
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRiceEgg:
                networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.steamedChicWPlainRiceEgg);
                networkedPlayerInteraction.ChangePlayerState(playerState);
                break;
        }

    }

    #endregion

    #endregion

    #region Serve Customers

    //Serving customers
    public void CheckCanPutDownOrder()
    {

        if (networkedPlayerInteraction.playerState != PlayerState.HoldingOrder)
        {
            //Debug("NetworkedCustomerInteraction - Player not holding a dish");
            return;
        }

        CmdCheckCanPutDownOrder(networkedPlayerInteraction.detectedObject, networkedPlayerInteraction.playerInventory);


    }

    [Command]
    public void CmdCheckCanPutDownOrder(GameObject detectedObject, GameObject playerInventory)
    {
        RpcCheckCanPutDownOrder(detectedObject, playerInventory);


    }

    [ClientRpc]
    public void RpcCheckCanPutDownOrder(GameObject detectedObject, GameObject playerInventory)
    {

        GameObject heldDish = networkedPlayerInteraction.playerInventory;
       // Debug.Log("NetworkedCustomerInteraction - Held dish is " + heldDish.GetComponent<OrderScript>().dishLabel);

        //if there is a detectedobject
        if (detectedObject)
        {
            //if looking at customer
            if (detectedObject.GetComponent<CustomerBehaviour_Seated>())
            {
               // Debug.Log("NetworkedCustomerInteraction - Looking at customer");
                if (ServingCustomer(heldDish, detectedObject)) //if order is right
                {
                    networkedPlayerInteraction.playerInventory = null;
                    detectedObject = null;
                    networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing);
                   // Debug.Log("Is inventory full:" + networkedPlayerInteraction.IsInventoryFull());
                    networkedPlayerInteraction.ChangePlayerState(PlayerState.Default, true);
                }

            }
            else
            {
               // Debug.Log("NetworkedCustomerInteraction - Not looking at customer that can be served");
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
            //Debug.Log("NetworkedCustomerInteraction - Serve order");
            return customer.GetComponent<CustomerBehaviour_Seated>().CheckOrder(dishObj);

        }

        return false;

    }//end serve method

    #endregion

}
