/*
 *  Usage: attach to parent of customer prefab
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CustomerPatience : NetworkBehaviour
{
    //variables
    [SerializeField] private float updateFrequency = 0.2f;
    [SerializeField] private Image patienceMeterImg;
    [SerializeField] private Color finalColor = Color.red;
    private Color startColor;

    private Coroutine patienceMeterCoroutine;
    private bool isCoroutineRunning = false; //bool used to ensure that coroutine does not get called while coroutine is running

    [SyncVar]
    [HideInInspector] public float currentPatience = 0f;

    //float to alter the rate at which the patience decreases
    [SyncVar]
    [HideInInspector] public float reductionRate = 1f;

    [Header("Optional Variables")]
    [SerializeField] private GameObject increaseFeedbackPFX;
    [SerializeField] private GameObject decreaseFeedbackPFX;
    [SerializeField] private Transform overheadFeedbackGameObj;

    #region Debug Shortcuts
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartPatienceMeter(CustomerPatienceStats.customerPatience_Queue);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StopPatienceMeter();
        }
    } 
    */
    #endregion

    private void Start()
    {
        //disable the image
        patienceMeterImg.enabled = false;
        startColor = patienceMeterImg.color;
    }


    //public method to call to start coroutine
    public void StartPatienceMeter(float totalPatience, Action callback = null)
    {
        if (isCoroutineRunning)
        {
            //bool used to ensure that coroutine does not get called while coroutine is running
            return;
        }

        isCoroutineRunning = true;

        patienceMeterCoroutine = StartCoroutine(UpdatePatienceMeter(totalPatience, callback));

    }

    //public method to call to stop and reset coroutine
    public void StopPatienceMeter()
    {
        if (!isCoroutineRunning)
        {
            //bool used to ensure that coroutine is only stopped once
            return;
        }

        isCoroutineRunning = false;

        //disable the image
        patienceMeterImg.enabled = false;

        //reset the patience
        currentPatience = 0f; //--------------------------------------------------------------------------- change here

        StopCoroutine(patienceMeterCoroutine);
    }


    //method that updates customers' patience meter, then, when patience runs out, calls the method (callback) passed into it 
    //understanding callbacks: https://stackoverflow.com/questions/54772578/passing-a-function-as-a-function-parameter/54772707
    private IEnumerator UpdatePatienceMeter(float totalPatience, Action callback = null, bool changeColor = true, bool delayColorChange = true, float colorChangingPoint = 0.5f)
    {
        currentPatience = totalPatience; //------------------------------------------------------------------------- change here

        //enable the patience meter img so player can see
        patienceMeterImg.enabled = true;

        while (currentPatience > 0)
        {
            //caps the current patience
            if (currentPatience > totalPatience)
            {
                currentPatience = totalPatience;
            }

            //calculate amount of patience left
            currentPatience -= updateFrequency * reductionRate; //-------------------------------------------------- change here
            patienceMeterImg.fillAmount = currentPatience / totalPatience;

            if (changeColor)
            {
                float colorLerpAmt = currentPatience / totalPatience;

                //if delayColorChange is set to true, 
                //slider will not change colour until 
                //the customer has colorChangingPoint amt of patience left 
                if (delayColorChange && colorLerpAmt > colorChangingPoint) //--------------------------------------- change here
                {
                    colorLerpAmt = 1f;
                }

                patienceMeterImg.color = Color.Lerp(finalColor, startColor, colorLerpAmt);
            }

            yield return new WaitForSeconds(updateFrequency);
        }


        //Debug.Log("Calling the impatient method");
        if (callback != null)
        {
            callback?.Invoke();
        }

        //disable the image
        patienceMeterImg.enabled = false;

        isCoroutineRunning = false;

        yield return null;
    }

    //method that increases the patience meter of the customer
    //call the method like this: IncreasePatience(CustomerPatienceStats.drinkPatienceIncrease);
    public void IncreasePatience(float amtIncrease)
    {
        if (isCoroutineRunning)
        {
            //Debug.Log("IncreasePatience(): Coroutine is running");

            //increase the customer's current patience
            currentPatience += amtIncrease;

            //if particle effects have been assigned, feedback to player that patience has increased
            if (increaseFeedbackPFX != null && overheadFeedbackGameObj != null)
            {
                GameObject increaseFeedback = Instantiate(increaseFeedbackPFX, overheadFeedbackGameObj.position + increaseFeedbackPFX.transform.position, overheadFeedbackGameObj.rotation);
                Destroy(increaseFeedback, 30f);
            }
        }
        else
        {
            // Debug.Log("IncreasePatience(): Coroutine is not running. Cannot increase patience.");
        }

    }

    //method that decreases the patience meter of the customer
    //call the method like this: DecreasePatience(CustomerPatienceStats.angryPatienceDecrease);
    public void DecreasePatience(float amtDecrease)
    {
        if (isCoroutineRunning)
        {
            Debug.Log("IncreasePatience(): Coroutine is running");

            //increase the customer's current patience
            currentPatience -= amtDecrease;

            //if particle effects have been assigned, feedback to player that patience has increased
            if (increaseFeedbackPFX != null && overheadFeedbackGameObj != null)
            {
                GameObject decreaseFeedback = Instantiate(decreaseFeedbackPFX, overheadFeedbackGameObj.position + decreaseFeedbackPFX.transform.position, overheadFeedbackGameObj.rotation);
                Destroy(decreaseFeedback, 20f);
            }
        }
        else
        {
            Debug.Log("IncreasePatience(): Coroutine is not running. Cannot increase patience.");
        }

    }


}