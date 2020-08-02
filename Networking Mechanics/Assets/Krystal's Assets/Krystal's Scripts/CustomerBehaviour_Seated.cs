﻿//Usage: attach to the parentobj of THE CUSTOMER SEATED PREFAB.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CustomerBehaviour_Seated : CustomerBehaviour
{
    private ChickenRice customersOrder = null;
    [SerializeField] private TableScript tableSeatedAt = null;
    private bool finishedEating = false;

    [SerializeField] private Transform dishSpawnPoint, orderIconPos;
    private OrderGeneration orderGenerationScript;

    public GameObject objectContainerPrefab;

    #region Getters and Setters
    public ChickenRice CustomersOrder
    {
        get { return customersOrder; }
        private set { customersOrder = value; }
    }

    public TableScript TableSeatedAt
    {
        get { return tableSeatedAt; }
        private set { tableSeatedAt = value; }
    }

    public bool FinishedEating
    {
        get { return finishedEating; }
        private set { finishedEating = value; }
    }
    #endregion

    //before the customer is visible, make sure to...
    private void Awake()
    {
        //disable the collider
        TriggerCustomerCollider(false, true);

        //ensure that the order icon is not visible
        orderIconPos.gameObject.SetActive(false);
        
        //get the order generation script
        orderGenerationScript = OrderGeneration.Instance;
    }


    //---------------------BEHAVIOUR WHEN SEATED---------------------

    //when customer has been brought to a table with enough seats, this method is called
    public void CustomerJustSeated(TableScript tableScript)
    {
        //animate the customer sitting down and browsing menu
        CustomerAnimScript.SitDownAnim();
        CustomerAnimScript.BrowseMenuAnim();
        Debug.Log("Animating customer sitting and browsing menu");

        //generate the customer's order
        GenerateOrder();

        //assign the table the customer is seated at as their table
        tableSeatedAt = tableScript;
    }

    //generates and assigns an order to the customer
    public void GenerateOrder()
    {
        customersOrder = orderGenerationScript.CreateNewOrder();

        if (customersOrder.OrderIcon != null)
        {
            //instantiate the order icon as the child of the orderIconPos obj
            GameObject orderIcon = Instantiate(customersOrder.OrderIcon, orderIconPos);
            //NetworkServer.Spawn(orderIcon);
        }

    }
    

    //after the customer's order has been taken, they will wait for their food
    public void DisplayOrderAndWait()
    {
        //animate the customer sitting idly and waiting for their food
        Debug.Log("Displaying customer order");
        CustomerAnimScript.WaitForFoodAnim();

        //display the customer's order
        orderIconPos.gameObject.SetActive(true);

        //enable their collider
        TriggerCustomerCollider(true, true);

        //if the customer waits too long for their food, they will SitAngrily() will be called
        TriggerPatienceMeter(true, CustomerPatienceStats.customerPatience_FoodWait, SitAngrily);
    }


    //when customer waits too long for their order, they will sit angrily
    public void SitAngrily()
    {
        Debug.Log("Sit angrily");
    }


    //check that the order being served to them is correct
    public bool CheckOrder(GameObject servedFood)
    {
        OrderScript servedFoodScript = servedFood.GetComponent<OrderScript>();

        Debug.Log("Checking if food served to customer is correct");

        if(servedFoodScript.DishLabel == customersOrder.ChickenRiceLabel)
        {
            //stop customer's patience meter
            TriggerPatienceMeter(false);

            //move the dish from the player to the dishspawnpoint of the customer
            servedFoodScript.ToggleIcon(false);
            servedFood.transform.parent = dishSpawnPoint;
            servedFood.transform.position = dishSpawnPoint.position;

            //animate the customer eating
            EatingFood();

            return true;
        }
        else
        { 
            WrongCustomer();

            return false;
        }
    }


    //customer leaving the restaurant. if angry, play angry anim
    [ServerCallback]
    public void LeaveRestaurant(bool isCustomerAngry)
    {
        //animate customer standing up
        CustomerAnimScript.LeaveAnim();
        Debug.Log("Standing from table");

        //if the customer is angry, play angry anim
        if (isCustomerAngry)
        {
            //animate the customer being angry
            Debug.Log("customer is angry!");
        }

        //customer fades out of existence
        Debug.Log("Customer fading out of existence");
        RpcLeaveRestaurant();

    }

    [ClientRpc]
    public void RpcLeaveRestaurant()
    {
        Destroy(this.gameObject, 5f);
        GameManager.Instance.currentNumWaitingCustomers -= 1;
    }


    //customer has been served the right food and is eating it
    [ServerCallback]
    public void EatingFood()
    {
        //disable the order icon
        orderIconPos.gameObject.SetActive(false);

        //enable eating animation
        CustomerFeedbackScript.PlayEatingPFX();
        CustomerAnimScript.StartEatingAnim();
        Debug.Log("Animating customer eating food");

        RpcEatingFood();
    }

    [ClientRpc]
    public void RpcEatingFood()
    {
        //eat for customerEatingDuration amount of time
        Invoke("CustomerFinishedFood", CustomerPatienceStats.customerEatingDuration);
    }


    //customer has been served the wrong food
    public void WrongCustomer()
    {
        Debug.Log("wrong order!!!!!!!!");
    }


    //function to call once customer finishes eating food
    [ServerCallback]
    public void CustomerFinishedFood()
    {
        //remove the food in front of the customer
        foreach (Transform child in dishSpawnPoint)
        {
            NetworkServer.Destroy(child.gameObject);
        }

        //Instantiate dirty dish in front of customer
        Debug.Log("Spawning dirty dishes");
        ServerSpawnDirtyDish();

        RpcCustomerFinishedFood();
    }

    [ClientRpc]
    public void RpcCustomerFinishedFood()
    {

        finishedEating = true;

        //disable eating animation
        CustomerFeedbackScript.PlayEatingPFX(false);
        CustomerAnimScript.StopEatingAnim();
        Debug.Log("Customer is done eating food");


        //all customers leave if they have all finished eating
        if (tableSeatedAt.CheckIfAllFinishedEating())
        {
            tableSeatedAt.EmptyTable();
        }
    }

    //spawn a dirty dish in front of the customer
    [ServerCallback]
    public void ServerSpawnDirtyDish()
    {
        GameObject dirtyDish = Instantiate(objectContainerPrefab, dishSpawnPoint.position, dishSpawnPoint.localRotation);

        dirtyDish.GetComponent<Rigidbody>().isKinematic = false;

        ObjectContainer plateContainer = dirtyDish.GetComponent<ObjectContainer>();

        //Instantiate the right held item
        plateContainer.SetObjToSpawn(HeldItem.dirtyplate);

        //Sync var
        plateContainer.objToSpawn = HeldItem.dirtyplate;

        dirtyDish.GetComponent<DirtyDishScript>().SetTableScript(tableSeatedAt);

        //Spawn on network
        NetworkServer.Spawn(dirtyDish);

        RpcSpawnDirtyDish(dirtyDish);

        
    }

    [ClientRpc]
    public void RpcSpawnDirtyDish(GameObject dirtyDish)
    {
        dirtyDish.layer = LayerMask.NameToLayer("TableItem");
    }
}