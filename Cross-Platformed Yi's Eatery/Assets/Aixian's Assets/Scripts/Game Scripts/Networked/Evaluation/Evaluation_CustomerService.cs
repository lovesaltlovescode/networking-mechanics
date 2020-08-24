using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Evaluation_CustomerService : NetworkBehaviour
{

    #region Singleton

    private static Evaluation_CustomerService _instance;
    public static Evaluation_CustomerService Instance { get { return _instance; } }

    private void Awake()
    {
        //Debug.Log(this.gameObject.name);

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
    [SyncVar]
    public float numCustomersServed = 0f;

    [SyncVar]
    public float numCustomersLost = 0f;

    //#region Getters and Setters

    //public static float NumCustomersServed
    //{
    //    get { return numCustomersServed; }
    //    private set { numCustomersServed = value; }
    //}

    //public static float NumCustomersLost
    //{
    //    get { return numCustomersLost; }
    //    private set { numCustomersLost = value; }
    //}


    //#endregion

    public void UpdateNumCustomersServed(float customers, bool decrease = false)
    {
        if (decrease)
        {
            numCustomersLost += customers;
        }
        else
        {
            numCustomersServed += customers;
        }

        Debug.Log("Customers lost " + numCustomersLost);
    }

    //calculates the quality of customer service based on the speed at which the customers were served, 
    // the mood of the customers, the number of customers that left angrily and so on.
    public float CalculateCustomerServiceScore()
    {
        Debug.Log("calculating customer service score...");

        if (Evaluation_OverallPlayerPerformance.Instance.customerServiceScore <= 0)
        {
            return 0;
        }

        float customerServiceScoreAttained =
            (Evaluation_OverallPlayerPerformance.Instance.customerServiceScore / Evaluation_OverallPlayerPerformance.Instance.maxCustomerServiceScore);
        Debug.Log("Final customer service score" + customerServiceScoreAttained);

        return customerServiceScoreAttained;
    }

    //resets all the values to their defaults at the beginning of the level
    public void ResetNumbers_CustomerService()
    {
        numCustomersServed = 0;
        numCustomersLost = 0;
    }
}