﻿//Usage: attach to the parentobj of THE CUSTOMER WAITING PREFAB.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class CustomerBehaviour_Queueing : CustomerBehaviour
{
    [Header("Group Size Icon Variables")]
    [SerializeField] private TextMeshProUGUI groupSizeText;
    [SerializeField] private GameObject groupSizeIcon;

    //group size variables
    [SyncVar]
    public int groupSizeNum = 0;
    public int GroupSizeNum
    {
        get { return groupSizeNum; }
        private set { groupSizeNum = value; }
    }



    //when the customer is first spawned, make sure that their group size icon and collider is disabled
    private void Awake()
    {
        EnableGroupSizeIcon(false);
        TriggerCustomerCollider(false, true);
    }



    //------------------------------------------METHODS TO GENERATE AND SHOW GROUP SIZE------------------------------------------

    #region Networked 

    [ServerCallback]
    public void ServerGenerateSizeOfGroup(numGuestsSpawnRates[] spawnRates = null)
    {
        RpcGenerateSizeOfGroup(spawnRates);

    }

    [ClientRpc]
    public void RpcGenerateSizeOfGroup(numGuestsSpawnRates[] spawnRates)
    {
        int i = Random.Range(0, 100);

        if (spawnRates != null)
        {
            foreach (numGuestsSpawnRates spawnableSize in spawnRates)
            {
                if (i >= spawnableSize.minProbability && i <= spawnableSize.maxProbability)
                {
                   // Debug.Log("Generate size of group: " + spawnableSize.numGuests);
                    groupSizeNum = spawnableSize.numGuests;

                    return;
                }
            }
        }

        //Debug.Log("Didn't get array, returning zero");
    }

    #endregion



    //generate the size of the customer group
    //public void GenerateSizeOfGroup(numGuestsSpawnRates[] spawnRates = null)
    //{
    //    int i = Random.Range(0, 100);

    //    if (spawnRates != null)
    //    {
    //        foreach (numGuestsSpawnRates spawnableSize in spawnRates)
    //        {
    //            if (i >= spawnableSize.minProbability && i <= spawnableSize.maxProbability)
    //            {
    //               // Debug.Log("Generate size of group: " + spawnableSize.numGuests);
    //                groupSizeNum = spawnableSize.numGuests;

    //                return;
    //            }
    //        }
    //    }

    //   // Debug.Log("Didn't get array, returning zero");
    //}



    //display the number of people in the group of customer queueing 
    public void ShowGroupSizeIcon(int groupSize)
    {
        //update the group size indicator to display the groupsize
        groupSizeText.text = groupSize.ToString();

        //enable the group icon display in the hierarchy
        EnableGroupSizeIcon(true);
    }



    //disable / enable the groupsizeIcon in the hierarchy
    public void EnableGroupSizeIcon(bool shouldEnable)
    {
        //set the group size icon false by default
        groupSizeIcon.SetActive(shouldEnable);
    }





    //-----------------------------------------METHODS TO CONTROL CUSTOMER BEHAVIOUR WHEN WAITING IN LINE-----------------------------------------
    [ServerCallback]
    public void CustomerStartsWaiting()
    {
        RpcCustomerStartsWaiting();
    }

    [ClientRpc]
    public void RpcCustomerStartsWaiting()
    {
        CustomerWaitingBehaviour();

        //enable the patience meter
        TriggerPatienceMeter(true, CustomerPatienceStats.CustomerPatience_Queue, CustomerWaitsTooLong);
    }

    //call this when the player puts the customer back down after picking them up
    //pass their prev patience level in

    //RPC THIS
    [Server]
    public void CustomerResumesWaiting(float resumedPatienceLevel, int groupSize)
    {

        //Debug.Log("Customer last patience level " + resumedPatienceLevel + " " + groupSize);

        RpcCustomerResumesWaiting(resumedPatienceLevel, groupSize);

    }

    [ClientRpc]
    public void RpcCustomerResumesWaiting(float resumedPatienceLevel, int groupSize)
    {
        GroupSizeNum = groupSize;

        //show group size num and activate collider
        CustomerWaitingBehaviour();

        //enable the patience meter
        TriggerPatienceMeter(resumedPatienceLevel, CustomerPatienceStats.CustomerPatience_Queue, CustomerWaitsTooLong);
    }

    //to be called when customer begins waiting
    private void CustomerWaitingBehaviour()
    {
        //enable the customer's group size indicator
        ShowGroupSizeIcon(GroupSizeNum);

        //enable the customer's collider so that the player can interact with the customer
        TriggerCustomerCollider(true, true);

        //animate the customer
        //<insert animation here>
    }

    #region Customer picked up or waits too long

    public void CustomerPickedUp(Transform carryPos)
    {
        //stop the patience meter
        TriggerPatienceMeter(false);

        // Debug.Log("Animating the customer curling up: " + carryPos);
    }

    [ServerCallback]
    public void CustomerWaitsTooLong()
    {
        Debug.Log("Customer impatient method successfully invoked. Customer waited too long");


        //customer fades out of existence
        RpcCustomerWaitsForTooLong();

        Debug.Log("Customer fading out of existence");
        GameManager.Instance.currentNumWaitingCustomers -= 1;
    }

    [ClientRpc]
    public void RpcCustomerWaitsForTooLong()
    {

        TriggerCustomerCollider(false, false);
        CustomerFeedbackScript.PlayAngryPFX();
        CustomerFeedbackScript.CustomerLeaves();

        CustomerAnimScript.LeaveAnim();
        Destroy(this.gameObject, 1f);

    }

    #endregion




}