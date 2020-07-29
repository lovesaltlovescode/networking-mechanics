//Usage: attach to the parent object of ALL CUSTOMER PREFAB.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CustomerBehaviour : NetworkBehaviour
{
    [Header("Customer Feedback Scripts Variables")]
    [SerializeField] public CustomerAnimationManager CustomerAnimScript;
    [SerializeField] public CustomerFeedback CustomerFeedbackScript;
    [SerializeField] public CustomerPatience CustomerPatienceScript;
    [SerializeField] private Collider CustomerCollider;
    

    //Patience Meter is started / stopped, depending on the bool passed into it
    public void TriggerPatienceMeter(bool startPatience, float waitTime = 0f, Action callback = null)
    {
        if(CustomerPatienceScript != null)
        {
            if (startPatience)
            {
                CustomerPatienceScript.StartPatienceMeter(waitTime, callback);
            }
            else
            {
                CustomerPatienceScript.StopPatienceMeter();
            }
        }
        else
        { 
            Debug.Log("Please assign the customer patience script to the customer prefab");
        }
        
    }


    //enable / disable the customer's collider, and/or set it to trigger
    public void TriggerCustomerCollider(bool isEnabled, bool isTrigger)
    {
        CustomerCollider.isTrigger = isTrigger;
        CustomerCollider.enabled = isEnabled;
    }



} //end of customer behaviour class
