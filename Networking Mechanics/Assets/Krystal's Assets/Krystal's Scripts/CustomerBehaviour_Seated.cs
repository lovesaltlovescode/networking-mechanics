//Usage: attach to the parentobj of THE CUSTOMER SEATED PREFAB.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CustomerBehaviour_Seated : CustomerBehaviour
{
    #region Variables and Properties

    public ChickenRice customersOrder = null;
    [SerializeField] private TableScript tableSeatedAt = null;
    private bool finishedEating = false;

    [SerializeField] private Transform dishSpawnPoint, orderIconPos;
    private OrderGeneration orderGenerationScript;

    public GameObject objectContainerPrefab;


    //[SerializeField] private GameObject roastedPlain, roastedPlain_egg, roastedBall, roastedBall_egg;
    //[SerializeField] private GameObject steamedPlain, steamedPlain_egg, steamedBall, steamedBall_egg;

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

    #endregion

    #region Initialisation

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

    #endregion

    //---------------------BEHAVIOUR WHEN SEATED---------------------

    #region Just Seated

    //when customer has been brought to a table with enough seats, this method is called
    public void CustomerJustSeated(TableScript tableScript)
    {

        //assign the table the customer is seated at as their table
        tableSeatedAt = tableScript;


        //animate the customer sitting down and browsing menu
        CustomerAnimScript.SitDownAnim();
        CustomerAnimScript.BrowseMenuAnim();
        Debug.Log("Animating customer sitting and browsing menu");

        if (isServer)
        {

            //RpcCustomerJustSeated();
            //generate the customer's order
            GenerateOrder();
        }

    }


    //generates and assigns an order to the customer
    //Local method, send command, 

    [ServerCallback]
    public void GenerateOrder()
    {
        RpcGenerateOrder(Random.value > 0.5f, Random.value > 0.5f, Random.value > 0.5f);
    }

    [ClientRpc]
    public void RpcGenerateOrder(bool roastedChic, bool ricePlain, bool haveEgg)
    {

        customersOrder = orderGenerationScript.CreateCustomOrder(roastedChic, ricePlain, haveEgg);

        if (customersOrder.OrderIcon != null)
        {
            //instantiate the order icon as the child of the orderIconPos obj

            GameObject orderIcon = Instantiate(customersOrder.OrderIcon, orderIconPos);
            //NetworkServer.Spawn(orderIcon);
        }
    }


    #endregion

    #region Waiting and Ordering

    //after the customer's order has been taken, they will wait for their food
    [ServerCallback]
    public void DisplayOrderAndWait()
    {
        //animate the customer sitting idly and waiting for their food
        Debug.Log("Displaying customer order");

        RpcDisplayOrderAndWait();
    }

    [ClientRpc]
    public void RpcDisplayOrderAndWait()
    {

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

        if (servedFoodScript.DishLabel == customersOrder.ChickenRiceLabel)
        {
            //stop customer's patience meter
            TriggerPatienceMeter(false);


            //animate the customer eating
            EatingFood(servedFood, servedFoodScript);

            return true;
        }
        else
        {
            WrongCustomer();

            return false;
        }
    }

    #region Networked

    ////check that the order being served to them is correct
    //[ServerCallback]
    //public bool CheckOrder(GameObject servedFood)
    //{
    //    OrderScript servedFoodScript = servedFood.GetComponent<OrderScript>();
    //    Debug.Log("Served food is " + servedFood);
    //    Debug.Log("Served food is " + servedFood);
    //    Debug.Log("Held dish script is" + servedFoodScript);

    //    RpcCheckOrder(servedFood);

    //    if (serveFood)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }

    //}

    //[ClientRpc]
    //public void RpcCheckOrder(GameObject servedFood)
    //{
    //    Debug.Log("Served food is " + servedFood);
    //    //Debug.Log("Served food script is " + servedFood.GetComponent<OrderScript>());
    //    Debug.Log("Checking if food served to customer is correct");

    //    if (servedFood.GetComponent<OrderScript>().DishLabel == customersOrder.ChickenRiceLabel)
    //    {
    //        ServeRightFood(servedFood, servedFood.GetComponent<OrderScript>());
    //        serveFood = true;
    //    }
    //    else
    //    {
    //        WrongCustomer();
    //        serveFood = false;
    //    }
    //}

    #endregion


    //customer has been served the wrong food
    public void WrongCustomer()
    {
        Debug.Log("wrong order!!!!!!!!");
    }


    #endregion

    #region Eating

    //customer has been served the right food and is eating it
    public void EatingFood(GameObject servedFood, OrderScript servedFoodScript)
    {
        //disable the order icon
        //orderIconPos.gameObject.SetActive(false);


        //move the dish from the player to the dishspawnpoint of the customer
        servedFoodScript.ToggleIcon(false);
        servedFood.transform.parent = dishSpawnPoint;
        servedFood.transform.position = dishSpawnPoint.position;

        //enable eating animation
        Debug.Log("Animating customer eating food");

        if (isServer)
        {
            RpcEatingFood();
        }
    }

    [ClientRpc]
    public void RpcEatingFood()
    {
        //disable the order icon
        orderIconPos.gameObject.SetActive(false); //may delete later, depending on if clients get this

        CustomerFeedbackScript.PlayEatingPFX();
        CustomerAnimScript.StartEatingAnim();
        //eat for customerEatingDuration amount of time
        TriggerCustomerCollider(false, false);
        Invoke("CustomerFinishedFood", CustomerPatienceStats.customerEatingDuration);
    }

    #endregion

    #region Finished Eating 

    //function to call once customer finishes eating food
    public void CustomerFinishedFood()
    {
        //remove the food in front of the customer
        foreach (Transform child in dishSpawnPoint)
        {
            NetworkServer.Destroy(child.gameObject);
        }

        //Instantiate dirty dish in front of customer
        Debug.Log("Spawning dirty dishes");
        finishedEating = true;

        //disable eating animation
        CustomerFeedbackScript.PlayEatingPFX(false);
        CustomerAnimScript.StopEatingAnim();
        Debug.Log("Customer is done eating food");

        if (isServer)
        {
            ServerSpawnDirtyDish();
        }

        //all customers leave if they have all finished eating
        if (tableSeatedAt.CheckIfAllFinishedEating())
        {
            tableSeatedAt.EmptyTable();
        }
    }

    [ClientRpc]
    public void RpcCustomerFinishedFood()
    {

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


        //Spawn on network
        NetworkServer.Spawn(dirtyDish);

        RpcSpawnDirtyDish(dirtyDish);


    }

    [ClientRpc]
    public void RpcSpawnDirtyDish(GameObject dirtyDish)
    {

        dirtyDish.GetComponent<DirtyDishScript>().SetTableScript(tableSeatedAt);

        dirtyDish.layer = LayerMask.NameToLayer("TableItem");
    }

    #endregion

    #region Leaving

    //customer leaving the restaurant. if angry, play angry anim
    [ServerCallback]
    public void LeaveRestaurant(bool isCustomerAngry)
    {
        //animate customer standing up
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
        CustomerAnimScript.LeaveAnim();
        Destroy(this.gameObject, 1f);
    }
    #endregion


}
