using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public class VR_OrderManagement_OG : MonoBehaviour
{
    private static VR_OrderManagement_OG _instance;
    public static VR_OrderManagement_OG Instance { get { return _instance; } }
    private void Awake()
    {
        #region Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        #endregion

        //ensure that all the order slips are empty
        foreach(GameObject orderSlip in orderSlips)
        {
            orderSlip.SetActive(false);
        }

        //hide the hidden order count game obj
        hiddenOrderCount_GameObj.SetActive(false);
    }

    [SerializeField] private GameObject[] orderSlips = new GameObject[10]; //the order slip spawn points
    [SerializeField] private GameObject hiddenOrderCount_GameObj; // gameobject that shows how many order slips are hidden from VR player's view
    [SerializeField] private TextMeshProUGUI hiddenOrderCount_numText;

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



    //if there is enough space to display another order slip, display the order slip and add it to the list of orders being displayed
    //if not, add the order to the list of hidden orders + update the hidden order count obj
    public void AddOrderToList(bool roastedChic, bool ricePlain, bool haveEgg)
    {
        ChickenRice tempOrderHolder = OrderGeneration.Instance.CreateCustomOrder(roastedChic, ricePlain, haveEgg);

        if (currentlyDisplayedOrders.Count() < orderSlips.Length)
        {
            currentlyDisplayedOrders.Add(tempOrderHolder);
            SpawnOrderSlip(tempOrderHolder);
        }
        else
        {
            AddHiddenOrder(tempOrderHolder);
        }
        
    }

    //checks for an empty order slip, prints the order passed in on the order slip and displays it.
    //If there are no empty order slips, the order is added to the hidden orders 
    public void SpawnOrderSlip(ChickenRice _chickenRiceOrder)
    {
        Debug.Log("Spawn Order slip called for " + _chickenRiceOrder.ChickenRiceLabel);

        //pick an empty order slip
        GameObject emptyOrderSlip = CheckEmptyOrderSlip();

        if (emptyOrderSlip != null)
        {
            //customize the order slip
            emptyOrderSlip.GetComponent<VR_OrderSlipBehaviour>().CustomizeOrderSlip(_chickenRiceOrder);

            //make the order slip visible
            emptyOrderSlip.SetActive(true);
        }
        else
        {
            AddHiddenOrder(_chickenRiceOrder);
        }

    }


    //returns the first empty order slip in the array of order slips
    private GameObject CheckEmptyOrderSlip()
    {
        foreach(GameObject orderSlip in orderSlips)
        {
            if (!orderSlip.activeInHierarchy)
            {
                return orderSlip;
            }
        }

        return null;
    }


    //-----------------------------------------------------------------SERVING AN ORDER-----------------------------------------------------------------------
    //method to serve the dish that the order slip is being placed on
    public void CheckCanServeDish(VR_OrderSlipBehaviour _orderSlip)
    {
        ChickenRice orderDetails = _orderSlip.OrderSlipOrder;

        //get the index num of the empty space on the counter
        int indexNum = SpawnDishOnCounter.Instance.CheckCounterHasSpace(); //returns -1 if there is no space

        // if there is space on the counter, remove the order slip and spawn the dish
        if (indexNum > -1) 
        {
            //SpawnDishOnCounter.Instance.SpawnDish(indexNum, orderDetails.RoastedChic, orderDetails.RicePlain, orderDetails.HaveEgg);

            RemoveOrderSlip(_orderSlip);
        }
        else //if the serve counter is full, give feedback
        {
            Debug.Log("Service counter is too full to spawn dish");
        }

    }


    //removes the order slip from the the currentlyDisplayedOrders list and hides its gameobj
    //then, it checks for hidden orders to display
    public void RemoveOrderSlip(VR_OrderSlipBehaviour orderSlip)
    {
        Debug.Log("remove order slip called");

        //disable the orderSlip
        orderSlip.gameObject.SetActive(false);

        //remove the order from the order slip list
        currentlyDisplayedOrders.Remove(orderSlip.OrderSlipOrder);
        orderSlip.ResetOrderSlip();

        //display a hidden order (if any)
        CheckAndDisplayHiddenOrders();
    }



    //-----------------------------------------------------HIDDEN ORDERS-----------------------------------------------------------
    public void CheckAndDisplayHiddenOrders()
    {
        //check if there are hidden orders
        if(hiddenOrders.Count > 0)
        {
            //display the first hidden order 
            SpawnOrderSlip(hiddenOrders[0]);

            //remove the displayed order from the list and update the number
            hiddenOrders.RemoveAt(0);
            UpdateHiddenOrdersCount();
        }
    }


    //updates the hidden order count number displayed. If there are no hidden orders, 
    private void UpdateHiddenOrdersCount()
    {
        if (hiddenOrders.Count > 0)
        {
            hiddenOrderCount_numText.text = ("+" + hiddenOrders.Count).ToString();
            hiddenOrderCount_GameObj.SetActive(true);
        }
        else
        {
            hiddenOrderCount_GameObj.SetActive(false);
        }

        Debug.Log("hiddenOrders: " + hiddenOrders.Count);
    }


    //add an order to the hidden order list
    private void AddHiddenOrder(ChickenRice _chickenRiceOrder)
    {
        hiddenOrders.Add(_chickenRiceOrder);
        UpdateHiddenOrdersCount();
    }


}
