using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VR_OrderManagement : MonoBehaviour
{
    #region Singleton

    private static VR_OrderManagement _instance;
    public static VR_OrderManagement Instance { get { return _instance; } }

    private void Awake()
    {
        Debug.Log(this.gameObject.name);

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    #region Variables

    [SerializeField] private GameObject[] orderSlipSpawnPoints = new GameObject[10]; //the order slip spawn points
    [SerializeField] private GameObject hiddenOrderCount; // gameobject that shows how many order slips are hidden from VR player's view
    private List<ChickenRice> currentlyDisplayedOrders = new List<ChickenRice>();
    public List<ChickenRice> CurrentlyDisplayedOrders
    {
        get { return currentlyDisplayedOrders; }
        private set { currentlyDisplayedOrders = value; }
    }
    private List<ChickenRice> hiddenOrders = new List<ChickenRice>();

    public List<ChickenRice> HiddenOrders
    {
        get { return hiddenOrders; }
        private set { hiddenOrders = value; }
    }

    #endregion

    //add the passed in chicken rice order to the list of orders
    public void AddOrderToList(bool roastedChic, bool ricePlain, bool haveEgg)
    {
        ChickenRice tempOrderHolder = OrderGeneration.Instance.CreateCustomOrder(roastedChic, ricePlain, haveEgg);

        //Check if should spawn in order slip or hide order first (order slip full)
        if (currentlyDisplayedOrders.Count() < orderSlipSpawnPoints.Length)
        {
            currentlyDisplayedOrders.Add(tempOrderHolder);
            SpawnOrderSlip(tempOrderHolder);
        }
        else
        {
            hiddenOrders.Add(tempOrderHolder);
            Debug.Log("hiddenOrders: " + hiddenOrders.Count);
        }

    }

    public void SpawnOrderSlip(ChickenRice _chickenRiceOrder)
    {
        Debug.Log("Spawn Order slip called for " + _chickenRiceOrder.ChickenRiceLabel);
        //pick an empty order slip

        //customize the order slip

        //make the order slip visible
    }

    public void CheckCanServeDish(VR_OrderSlipBehaviour _orderSlip = null, ChickenRice _chickenRiceOrder = null)
    {
        //get the index num of the empty space on the counter
        int indexNum = SpawnDishOnCounter.Instance.CheckCounterHasSpace(); //returns -1 if there is no space

        ChickenRice orderDetails;

        if (_orderSlip != null)
        {
            //get the details of the order indicated on the orderslip
            orderDetails = _orderSlip.orderSlipOrder;
        }
        else if (_chickenRiceOrder != null)
        {
            orderDetails = _chickenRiceOrder;
        }
        else
        {
            Debug.Log("Please pass an order slip into this function");
            return;
        }


        if (indexNum > -1) // if there is space on the counter, remove the order slip and spawn the dish
        {
            SpawnDishOnCounter.Instance.ServerSpawnDish(indexNum, orderDetails.RoastedChic, orderDetails.RicePlain, orderDetails.HaveEgg);

            RemoveOrderSlip(orderDetails);
        }
        else //if the serve counter is full, give feedback
        {
            Debug.Log("Service counter is too full to spawn dish");
        }

    }

    public void RemoveOrderSlip(ChickenRice _chickenRiceOrder)
    {
        Debug.Log("remove order slip called");

        //remove the order from the order slip list
        currentlyDisplayedOrders.Remove(_chickenRiceOrder);

        //check if there are hidden orders
        //display the first hidden order 
        //update the hidden order count obj


    }
}
