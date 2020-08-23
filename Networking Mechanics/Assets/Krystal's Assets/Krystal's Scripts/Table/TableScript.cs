using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

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
    //public GameObject dirtyDishPrefab;
    public GameObject customerSeatedPrefab;
    [SerializeField] private Transform seatedCustomerParent;

    //list of customers that are seated at table
    public List<GameObject> customersSeated = new List<GameObject>();
    [SyncVar]
    public float customersAtTable;

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


    //[HideInInspector] public bool isTableDirty = false;
    public List<GameObject> dirtyDishes = new List<GameObject>();

    void Awake()
    {

        //add current table to table collider manager list
        //TableColliderManager.Instance.AddTableToTableColliderManager(gameObject);
    }


    void Start()
    {
        ResetTables();
    }

    [ServerCallback]
    public void ResetTables()
    {
        RpcResetTables();
    }

    [ClientRpc]
    public void RpcResetTables()
    {

        //clear the customer and orders lists
        customersSeated.Clear();
        customersAtTable = 0;
        tableOrders.Clear();
        dirtyDishes.Clear(); //-------------------------------------change here

        TableColliderManager.Instance.ToggleTableDetection(false);
        VR_OrderManagement.Instance.ClearOrderList();
        tableFeedbackScript.ToggleOrderIcon(false);

        //update the number of seats the table has
        numSeats = seatPositions.Count;
    }

    public void Update()
    {
        if (LevelTimer.Instance.hasLevelEnded)
        {
            ResetTables();
            return;
        }
    }

    //-------------------------------------------------------- METHODS RELATED TO CUSTOMERS INTERACTING WITH TABLE AND SEATS
    //check number of customers
    public bool CheckSufficientSeats(int numGuests)
    {
        if (customersSeated.Count > 0)
        {
            RpcTableOccupied();
            return false;
        }
        else if (dirtyDishes.Count > 0)
        {
            RpcTableDirty();
            return false;
        }



       // Debug.Log("checking if there are sufficient seats");

        if (numGuests <= numSeats)
        {
            if (numGuests < numSeats)
            {
                //Debug.Log("less guests than seats");
            }
            else if (numGuests > numSeats)
            {
                //Debug.Log("enough seats for guests");
            }

            //seat the guests
            ServerSeatGuests(numGuests);

            return true;
        }
        else
        {
            RpcNotEnoughSeats();
            return false;
        }
        
    }

    #region Networked Feedback

    [ClientRpc]
    public void RpcTableOccupied()
    {
       // Debug.Log("CustomersSeated.Count: " + customersSeated.Count);
        tableFeedbackScript.TableOccupied();
    }

    [ClientRpc]
    public void RpcTableDirty()
    {
        //Debug.Log("dirtyDishes.Count: " + dirtyDishes.Count);
        tableFeedbackScript.TableDirty();
    }

    [ClientRpc]
    public void RpcNotEnoughSeats()
    {
       // Debug.Log("more guests than seats");

        //feedback to player that there are insufficient seats
        tableFeedbackScript.NotEnoughSeats();
    }

    #endregion


    #region Seat Guests

    [ServerCallback]
    public void ServerSeatGuests(int numGuests)
    {
        //Debug.Log("TableScript - Server: Guests are being seated");

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
        //Debug.Log("TableScript - RpcUpdateList called");

        //animate customer sitting, assign this table to the customer, and get it to generate an order
        newSittingCustomer.GetComponent<CustomerBehaviour_Seated>().CustomerJustSeated(this);


        customersSeated.Add(newSittingCustomer);
        tableOrders.Add(newSittingCustomer.GetComponent<CustomerBehaviour_Seated>().CustomersOrder);

        customersAtTable = CustomersSeated.Count;

        //Debug.Log("TableScript - Table orders: " + tableOrders.Count);
    }

    [ClientRpc]
    public void RpcSeatGuests(int numGuests)
    {
        //Debug.Log("TableScript - RpcSeatGuests called");
        numSeated = numGuests;
        //Debug.Log("numGuests: " + numSeated + ", customersSeated: " + customersSeated.Count);

        //after a random amount of time, call a server to take their order
        Invoke("ReadyToOrder", Random.Range(minAndMaxOrderGenTime.x, minAndMaxOrderGenTime.y));
    }


    #endregion

    #region Guests Ordering

    [ServerCallback]
    public void ReadyToOrder()
    {

       // Debug.Log("Table Script - ServerReadyToOrder");
        RpcReadyToOrder();
    }

    [ClientRpc]
    public void RpcReadyToOrder()
    {
        //move the table collider to a separate layer
        TableColliderManager.Instance.ToggleTableDetection(true, this.gameObject, takeOrderLayer);

        //enable the UI
        tableFeedbackScript.ToggleOrderIcon(true);

        //animate the customers ordering food
        foreach (GameObject customer in customersSeated)
        {
            customer.GetComponent<CustomerBehaviour_Seated>().CustomerAnimScript.OrderAnim();
        }

        //start the patience script
        patienceScript.StartPatienceMeter(CustomerPatienceStats.CustomerPatience_TakeOrder, OrderNotTaken);
    }

    #region Take orders

    [ServerCallback]
    public void ServerTakeOrder()
    {
       // Debug.Log("Table Script - ServerTakeOrder");
        RpcServerTakeOrder();
    }

    [ClientRpc]
    public void RpcServerTakeOrder()
    {
       // Debug.Log("Table Script - RpcServerTakeOrder");
        //stop the patience script
        patienceScript.StopPatienceMeter();

        //disable the order icon UI
        tableFeedbackScript.ToggleOrderIcon(false);

        //pass all the orders to the kitchen
       // Debug.Log("All orders: " + tableOrders);

        //display the customer's order and make them wait
        foreach (GameObject customer in customersSeated)
        {
            customer.GetComponent<CustomerBehaviour_Seated>().DisplayOrderAndWait();

            //pass customer orders to kitchen
            VR_OrderManagement.Instance.AddOrderToList(customer.GetComponent<CustomerBehaviour_Seated>().CustomersOrder.RoastedChic,
                customer.GetComponent<CustomerBehaviour_Seated>().CustomersOrder.RicePlain,
                customer.GetComponent<CustomerBehaviour_Seated>().CustomersOrder.HaveEgg);
        }


        //move the table collider back to the environment layer
        TableColliderManager.Instance.ToggleTableDetection(false, this.gameObject);
    }

    //call this method when customer waits too long for their order
    public void OrderNotTaken()
    {
        //disable the order icon
        tableFeedbackScript.ToggleOrderIcon(false);

        //clear the table of customers and have them leave angrily
        EmptyTable(true);
    }

    #endregion


    #endregion

    #region Guests Leaving

    //call this method when the table has no guests seated at it
    [ServerCallback]
    public void EmptyTable(bool isCustomerAngry = false)
    {
        RpcEmptyTable(isCustomerAngry);
    }

    [ClientRpc]
    public void RpcEmptyTable(bool isCustomerAngry)
    {
        //animate customers leaving
        foreach (GameObject customer in customersSeated)
        {
            CustomerBehaviour_Seated customerScript = customer.GetComponent<CustomerBehaviour_Seated>();
            customerScript.LeaveRestaurant(isCustomerAngry);
        }

        if (!isCustomerAngry)
        {
            tableFeedbackScript.SuccessfulCustomerService();
            Evaluation_CustomerService.Instance.UpdateNumCustomersServed(customersAtTable);
            GameManager.Instance.IncreaseMood(5);
        }

        //clear the lists
        customersSeated.Clear();
        customersAtTable = 0;
        tableOrders.Clear();

        TableColliderManager.Instance.ToggleTableDetection(false, gameObject, "Table");
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

    #endregion

}//end of tablescript class
