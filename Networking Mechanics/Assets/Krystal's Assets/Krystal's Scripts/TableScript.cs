using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using UnityEngine;
using Mirror;

public class TableScript : NetworkBehaviour
{
    [HideInInspector, Range(0, 6)] public int numSeats = 0; //number of seats the table has

    [SyncVar]
    public int numSeated = 0; //number of customers seated at table

    [SerializeField] private TableFeedback tableFeedbackScript;
    public TableFeedback TableFeedbackScript
    {
        get { return tableFeedbackScript; }
        private set { tableFeedbackScript = value; }
    }
    [SerializeField] private CustomerPatience patienceScript;

    //customer-related fields
    [SerializeField] private List<Transform> seatPositions = new List<Transform>();
    [SerializeField] private Vector2 minAndMaxOrderGenTime = new Vector2(3f, 5f);

    //prefabs required
    public GameObject dirtyDishPrefab;
    public GameObject customerSeatedPrefab;
    [SerializeField] private Transform seatedCustomerParent;

    //list of customers that are seated at table
    public List<GameObject> customersSeated = new List<GameObject>();

    public List<GameObject> CustomersSeated
    {
        get { return customersSeated; }
        private set { customersSeated = value; }
    }


    //list of orders of customers that are seated at table
    [SerializeField] private string takeOrderLayer = "Ordering";
    public List<ChickenRice> tableOrders = new List<ChickenRice>();
    public List<ChickenRice> TableOrders
    {
        get { return tableOrders; }
        private set { tableOrders = value; }
    }


    [HideInInspector] public bool isTableDirty = false;


    void Start()
    {
        //add current table to table collider manager list
        TableColliderManager.AddTableToTableColliderManager(gameObject);

        //clear the customer and orders lists
        customersSeated.Clear();
        tableOrders.Clear();

        //update the number of seats the table has
        numSeats = seatPositions.Count;

    }


    //-------------------------------------------------------- METHODS RELATED TO CUSTOMERS INTERACTING WITH TABLE AND SEATS
    //check number of customers
    public bool CheckSufficientSeats(int numGuests)
    {
        if (customersSeated.Count > 0)
        {
            Debug.Log("CustomersSeated.Count: " + customersSeated.Count);
            tableFeedbackScript.TableOccupied();
            return false;
        }

        Debug.Log("checking if there are sufficient seats");

        if (numGuests <= numSeats)
        {
            if (numGuests < numSeats)
            {
                Debug.Log("less guests than seats");
            }
            else if (numGuests > numSeats)
            {
                Debug.Log("enough seats for guests");
            }

            //seat the guests
            ServerSeatGuests(numGuests);

            return true;
        }
        else
        {
            Debug.Log("more guests than seats");

            //feedback to player that there are insufficient seats
            tableFeedbackScript.NotEnoughSeats();

            return false;
        }
        
    }


    #region Networked

    #region Seat Guests

    [ServerCallback]
    public void ServerSeatGuests(int numGuests)
    {
        Debug.Log("TableScript - Server: Guests are being seated");

        for (int i = 0; i < numGuests; i++)
        {
            //Instantiate customer
            GameObject newSittingCustomer = Instantiate(customerSeatedPrefab, seatPositions[i].position, seatPositions[i].rotation).gameObject;

            CustomerBehaviour_Seated newCustomerScript = newSittingCustomer.GetComponent<CustomerBehaviour_Seated>();

            //Spawn
            NetworkServer.Spawn(newSittingCustomer);

            //RPC List
            RpcUpdateList(newSittingCustomer);
        }

        RpcSeatGuests(numGuests);
    }

    [ClientRpc]
    public void RpcUpdateList(GameObject newSittingCustomer)
    {
        Debug.Log("TableScript - RpcUpdateList called");

        //animate customer sitting, assign this table to the customer, and get it to generate an order
        newSittingCustomer.GetComponent<CustomerBehaviour_Seated>().CustomerJustSeated(this);

        //add customer and their to list of customers seated at table
        if (newSittingCustomer.GetComponent<CustomerBehaviour_Seated>().CustomersOrder != null)
        {
            customersSeated.Add(newSittingCustomer);
            tableOrders.Add(newSittingCustomer.GetComponent<CustomerBehaviour_Seated>().CustomersOrder);
            Debug.Log("TableScript - Table orders: " + tableOrders.Count);
        }
        else
        {
            Debug.Log("tried to add customer to list, but customer's order was null");
        }
    }

    [ClientRpc]
    public void RpcSeatGuests(int numGuests)
    {
        Debug.Log("TableScript - RpcSeatGuests called");
        numSeated = numGuests;
        Debug.Log("numGuests: " + numSeated + ", customersSeated: " + customersSeated.Count);

        //after a random amount of time, call a server to take their order
        Invoke("ReadyToOrder", Random.Range(minAndMaxOrderGenTime.x, minAndMaxOrderGenTime.y));
    }


    #endregion

    #region Guests Ordering

    [ServerCallback]
    public void ReadyToOrder()
    {
        Debug.Log("Table Script - ServerReadyToOrder");
        RpcReadyToOrder();
    }

    [ClientRpc]
    public void RpcReadyToOrder()
    {
        //move the table collider to a separate layer
        TableColliderManager.ToggleTableDetection(true, this.gameObject, takeOrderLayer);

        //enable the UI
        tableFeedbackScript.ToggleOrderIcon(true);

        //animate the customers ordering food
        foreach (GameObject customer in customersSeated)
        {
            customer.GetComponent<CustomerBehaviour_Seated>().CustomerAnimScript.OrderAnim();
        }

        //start the patience script
        patienceScript.StartPatienceMeter(CustomerPatienceStats.customerPatience_TakeOrder, RpcOrderNotTaken);
    }

    #endregion

    #region Take orders

    [ServerCallback]
    public void ServerTakeOrder()
    {
        Debug.Log("Table Script - ServerTakeOrder");
        RpcServerTakeOrder();
    }

    [ClientRpc]
    public void RpcServerTakeOrder()
    {
        Debug.Log("Table Script - RpcServerTakeOrder");
        //stop the patience script
        patienceScript.StopPatienceMeter();

        //disable the order icon UI
        tableFeedbackScript.ToggleOrderIcon(false);

        //move the table collider back to the environment layer
        TableColliderManager.ToggleTableDetection(false, this.gameObject);

        //pass all the orders to the kitchen
        Debug.Log("All orders: " + tableOrders);

        //display the customer's order and make them wait
        foreach (GameObject customer in customersSeated)
        {
            customer.GetComponent<CustomerBehaviour_Seated>().DisplayOrderAndWait();
        }
    }

    [ClientRpc]
    public void RpcOrderNotTaken()
    {
        Debug.Log("TableScript - RpcOrderNotTaken");
        //disable the order icon
        tableFeedbackScript.ToggleOrderIcon(false);
        isTableDirty = false;

        //clear the table of customers and have them leave angrily
        EmptyTable(true);
    }

    #endregion

    #endregion

    //instantiate 1 customer at every seat, add them to a list, then call the method on the customer to manage their sitting animation + order
    public void SeatGuests(int numGuests)
    {
        Debug.Log("Guests are being seated");
        
        //call the seated event
        for (int i = 0; i < numGuests; i++)
        {
            //instantiate customer and get its script
            GameObject newSittingCustomer = Instantiate(customerSeatedPrefab, seatPositions[i].position, seatPositions[i].rotation).gameObject;
            newSittingCustomer.transform.parent = seatedCustomerParent;
            CustomerBehaviour_Seated newCustomerScript = newSittingCustomer.GetComponent<CustomerBehaviour_Seated>();

            //animate customer sitting, assign this table to the customer, and get it to generate an order
            newCustomerScript.CustomerJustSeated(this);

            //add customer and their to list of customers seated at table
            if (newCustomerScript.CustomersOrder != null)
            {
                customersSeated.Add(newSittingCustomer);
                tableOrders.Add(newCustomerScript.CustomersOrder);
            }
            else
            {
                Debug.Log("tried to add customer to list, but customer's order was null");
            }
        }

        numSeated = numGuests;
        Debug.Log("numGuests: " + numSeated + ", customersSeated: " + customersSeated.Count);

        //after a random amount of time, call a server to take their order
        Invoke("ServerReadyToOrder", Random.Range(minAndMaxOrderGenTime.x, minAndMaxOrderGenTime.y));

    }


    //enable the ui and start the patience meter.
    public void TestReadyToOrder()
    {
        Debug.Log("TableScript - ReadyToOrder");
        //move the table collider to a separate layer
        TableColliderManager.ToggleTableDetection(true, this.gameObject, takeOrderLayer);

        //enable the UI
        tableFeedbackScript.ToggleOrderIcon(true);

        //animate the customers ordering food
        foreach (GameObject customer in customersSeated)
        {
            customer.GetComponent<CustomerBehaviour_Seated>().CustomerAnimScript.OrderAnim();
        }

        //start the patience script
        patienceScript.StartPatienceMeter(CustomerPatienceStats.customerPatience_TakeOrder, OrderNotTaken);
    }


    public void TakeOrder()
    {
        //stop the patience script
        patienceScript.StopPatienceMeter();

        //disable the order icon UI
        tableFeedbackScript.ToggleOrderIcon(false);

        //move the table collider back to the environment layer
        TableColliderManager.ToggleTableDetection(false, this.gameObject);

        //pass all the orders to the kitchen
        Debug.Log("All orders: " + tableOrders);

        //display the customer's order and make them wait
        foreach (GameObject customer in customersSeated)
        {
            customer.GetComponent<CustomerBehaviour_Seated>().DisplayOrderAndWait();
        }
    }


    //call this method when customer waits too long for their order
    public void OrderNotTaken()
    {
        //disable the order icon
        tableFeedbackScript.ToggleOrderIcon(false);
        isTableDirty = false; 

        //clear the table of customers and have them leave angrily
        EmptyTable(true);
    }



    //call this method when the table has no guests seated at it
    public void EmptyTable(bool isCustomerAngry = false)
    {
        //animate customers leaving
        foreach (GameObject customer in customersSeated)
        {
            CustomerBehaviour_Seated customerScript = customer.GetComponent<CustomerBehaviour_Seated>();
            customerScript.LeaveRestaurant(isCustomerAngry);
        }

        //clear the lists
        customersSeated.Clear();
        tableOrders.Clear();
    }


    //check whether all customers at the table are done eating
    public bool CheckIfAllFinishedEating()
    {
        foreach (GameObject customer in customersSeated)
        {
            CustomerBehaviour_Seated customerScript = customer.GetComponent<CustomerBehaviour_Seated>();

            if (!customerScript.FinishedEating)
            {
                return false;
            }
        }

        return true;
    }



}//end of tablescript class
