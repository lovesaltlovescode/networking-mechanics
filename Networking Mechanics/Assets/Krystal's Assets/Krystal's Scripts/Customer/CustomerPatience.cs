/*
 *  Usage: attach to parent of customer prefab
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

//Current customer mood
public enum CurrentCustomerMood
{
    customerHappy,
    customerImpatient,
    customerAngry,
    customerStewing, //if customer is waiting for order past 0 patience
    customerDefault
}

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

    private bool coroutineIsPaused = false;

    [Header("Identify customer state")]
    [SerializeField] private string customerState = null;
    [SerializeField] private string customerQueueing = "Queueing";
    [SerializeField] private string customerOrdering = "Ordering";
    [SerializeField] private string customerWaiting = "Waiting For Food";

    [SyncVar]
    public CurrentCustomerMood customerMood;

    private bool isPenaltyCoroutineRunning = false;
    private Coroutine penaltyCoroutine;

    #region Getters and setters

    #region Customer States

    public string CustomerState
    {
        get { return customerState; }
        set { customerState = value; }
    }
    public string CustomerQueueing
    {
        get { return customerQueueing; }
        set { customerQueueing = value; }
    }
    public string CustomerOrdering
    {
        get { return customerOrdering; }
        set { customerOrdering = value; }
    }
    public string CustomerWaiting
    {
        get { return customerWaiting; }
        set { customerWaiting = value; }
    }

    #endregion

    #endregion




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

    private void Awake()
    {
        //disable the image
        patienceMeterImg.enabled = false;

        startColor = patienceMeterImg.color;

        if(gameObject.layer == LayerMask.NameToLayer("Queue"))
        {
            customerState = customerQueueing;
        }
        else if(gameObject.layer == LayerMask.NameToLayer("Table"))
        {
            customerState = customerOrdering;
        }
    }

    #region Toggle Patience Meter

    //public method to call to start coroutine
    public void StartPatienceMeter(float totalPatience, Action callback = null)
    {
        if (isCoroutineRunning)
        {
            //bool used to ensure that coroutine does not get called while coroutine is running

            if (LevelTimer.Instance.hasLevelEnded)
            {
                StopPatienceMeter();
            }

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

        if (LevelTimer.Instance.hasLevelEnded)
        {
            gameObject.SetActive(false);
        }

        isCoroutineRunning = false;

        //disable the image
        patienceMeterImg.enabled = false;

        //reset the patience
        currentPatience = 0f; //--------------------------------------------------------------------------- change here

        StopCoroutine(patienceMeterCoroutine);
    }

    //public method to call to start the patience meter at less than 100% patience
    public void RestartPatienceMeter(float totalPatience, float startingPatience, Action callback = null)
    {
        if (isCoroutineRunning)
        {
            //bool used to ensure that coroutine does not get called while coroutine is running
            return;
        }

        isCoroutineRunning = true;

        //able the image
        patienceMeterImg.enabled = true;
        //Debug.Log("Total patience " + totalPatience + ", Starting patience " + startingPatience);

        patienceMeterCoroutine = StartCoroutine(UpdatePatienceMeter(totalPatience, startingPatience, callback));

    }


    //method used to un/pause the coroutine
    //currently not in use. Perhaps in the future, if we implement a pause feature of some sort
    public void TogglePausePatienceMeter(bool pause)
    {
        if (!isCoroutineRunning || coroutineIsPaused == pause)
        {
            //bool used to ensure that coroutine is only stopped once
            return;
        }
        //update the bool
        coroutineIsPaused = pause;

        //toggle the image
        patienceMeterImg.enabled = !pause; //if paused, patience meter should not be visible

    }

    #endregion

    #region Penalty

    public IEnumerator OrderPenaltyTimer()
    {
        while (true)
        {

            GameManager.Instance.IncrementMood(1, true);
            GetComponent<CustomerFeedback>().PlayAngryPFX();
            Debug.Log("Current shop mood " + GameManager.Instance.currentShopMood);

            yield return new WaitForSeconds(5f);
        }

    }

    //public method to call to start coroutine
    public void StartOrderPenaltyTimer()
    {
        if (isPenaltyCoroutineRunning)
        {
            return;
        }

        isPenaltyCoroutineRunning = true;
        penaltyCoroutine = StartCoroutine(OrderPenaltyTimer());

    }

    public void StopOrderPenaltyTimer()
    {
        if (!isPenaltyCoroutineRunning)
        {
            return;
        }
        isPenaltyCoroutineRunning = false;
        StopCoroutine(penaltyCoroutine);
    }

    #endregion


    #region Check Customer Mood


    public void CheckCustomerMood()
    {
        if (customerState == customerQueueing)
        {
            Debug.Log("Customer is queueing");
            CalculatePatiencePercentage();
        }
        else if (customerState == customerOrdering)
        {
            Debug.Log("Customer is ordering");
            CalculatePatiencePercentage(true);
        }
        else if (customerState == customerWaiting)
        {
            Debug.Log("Customer is waiting for food");
            CalculatePatiencePercentage(false, true);
        }
    }

    //reset patience set to true when customer has been seated
    public void CalculatePatiencePercentage(bool ordering = false, bool waitingForFood = false, bool resetPatience = false)
    {
        if (resetPatience)
        {
            ResetCustomerPatience();
            return;
        }

        if (ordering)
        {
            float customerOrderingPatience = (currentPatience / CustomerPatienceStats.CustomerPatience_TakeOrder) * 100;
            Debug.Log("Current ordering customer patience: " + customerOrderingPatience);

            if (customerOrderingPatience >= 50 && customerOrderingPatience > 0)
            {
                customerMood = CurrentCustomerMood.customerHappy;
            }
            else if (customerOrderingPatience >= 30 && customerOrderingPatience < 50)
            {
                customerMood = CurrentCustomerMood.customerImpatient;
            }
            else if (customerOrderingPatience >= 20 || customerOrderingPatience < 30 && customerOrderingPatience > 0)
            {
                customerMood = CurrentCustomerMood.customerAngry;
            }
        }
        else if (waitingForFood)
        {
            float customerWaitingPatience = (currentPatience / CustomerPatienceStats.CustomerPatience_FoodWait) * 100;
            Debug.Log("Current waiting customer patience: " + customerWaitingPatience);

            if (customerWaitingPatience >= 50 && customerWaitingPatience > 0)
            {
                customerMood = CurrentCustomerMood.customerHappy;
            }
            else if (customerWaitingPatience >= 30 && customerWaitingPatience < 50)
            {
                customerMood = CurrentCustomerMood.customerImpatient;
            }
            else if (customerWaitingPatience >= 20 || customerWaitingPatience < 30 && customerWaitingPatience > 0)
            {
                customerMood = CurrentCustomerMood.customerAngry;
            }
            else if(customerWaitingPatience < 1)
            {
                customerMood = CurrentCustomerMood.customerStewing;
            }
        }
        //queueing
        else
        {
            float customerQueueingPatience = (currentPatience / CustomerPatienceStats.CustomerPatience_Queue) * 100;
            Debug.Log("Current queueing customer patience: " + customerQueueingPatience);

            if (customerQueueingPatience >= 50 && customerQueueingPatience > 0)
            {
                customerMood = CurrentCustomerMood.customerHappy;
            }
            else if (customerQueueingPatience >= 30 && customerQueueingPatience < 50)
            {
                customerMood = CurrentCustomerMood.customerImpatient;
            }
            else if (customerQueueingPatience >= 20 || customerQueueingPatience < 30 && customerQueueingPatience > 0)
            {
                customerMood = CurrentCustomerMood.customerAngry;
            }
        }



    }

    public void ResetCustomerPatience()
    {
        customerMood = CurrentCustomerMood.customerDefault;
    }

    #endregion


    #region Check Shop Mood

    public void CheckCurrentShopMood()
    {
        Debug.Log("Current shop mood " + GameManager.Instance.currentShopMood);

        if (GameManager.Instance.currentShopMood <= 25 || GameManager.Instance.currentShopMood == 0)
        {
            if(reductionRate == 1f * 120 / 100)
            {
                return;
            }
            else
            {
                reductionRate = 1f * 120 / 100;
            }
           
            Debug.Log("120% reduction rate: " + reductionRate);
        }
        else if (GameManager.Instance.currentShopMood > 25 && GameManager.Instance.currentShopMood < 50)
        {
            if(reductionRate == 1f * 110 / 100)
            {
                return;
            }
            else
            {
                reductionRate = 1f * 110 / 100;
            }
            
            Debug.Log("110% reduction rate: " + reductionRate);
        }
        else if (GameManager.Instance.currentShopMood == 50)
        {
            if(reductionRate == 1f)
            {
                return;
            }
            else
            {
                reductionRate = 1f;
            }
            
            Debug.Log("100% reduction rate: " + reductionRate);
        }
        else if (GameManager.Instance.currentShopMood > 50 && GameManager.Instance.currentShopMood < 75)
        {
            if(reductionRate == 1f * 90 / 100)
            {
                return;
            }
            else
            {
                reductionRate = 1f * 90 / 100;
            }
            
            Debug.Log("90% reduction rate: " + reductionRate);
        }
        else if (GameManager.Instance.currentShopMood >= 75 || GameManager.Instance.currentShopMood == 100)
        {
            
            if(reductionRate == 1f * 80 / 100)
            {
                return;
            }
            else
            {
                reductionRate = 1f * 80 / 100;
            }
            
            Debug.Log("80% reduction rate: " + reductionRate);
        }
    }

    #endregion


    #region Update patience meter

    //method that updates customers' patience meter, then, when patience runs out, calls the method (callback) passed into it 
    //understanding callbacks: https://stackoverflow.com/questions/54772578/passing-a-function-as-a-function-parameter/54772707
    private IEnumerator UpdatePatienceMeter(float totalPatience, Action callback = null, bool changeColor = true, bool delayColorChange = true, float colorChangingPoint = 0.5f)
    {

        currentPatience = totalPatience; //------------------------------------------------------------------------- change here

        //enable the patience meter img so player can see
        patienceMeterImg.enabled = true;

        while (currentPatience > 0 && !LevelTimer.Instance.hasLevelEnded)
        {
            //caps the current patience
            if (currentPatience > totalPatience)
            {
                currentPatience = totalPatience;
            }
            

            //calculate amount of patience left
            CheckCurrentShopMood(); //get updated reduction rate
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

    //overload that accepts a starting patience level for the patience meter (has an extra required parameter "startingPatience")
    //called when the customer is supposed to start their patience meter at less than 100% ie. after being held and put down
    private IEnumerator UpdatePatienceMeter(float totalPatience, float startingPatience, Action callback = null, bool changeColor = true, bool delayColorChange = true, float colorChangingPoint = 0.4f)
    {

        currentPatience = startingPatience;

        //enable the patience meter img so player can see
        patienceMeterImg.enabled = true;

        while (currentPatience > 0 && !LevelTimer.Instance.hasLevelEnded)
        {
            //Debug.Log("UpdatePatienceMeter - Total patience " + totalPatience + " , current patience " + currentPatience);

            //caps the current patience
            if (currentPatience > totalPatience)
            {
                currentPatience = totalPatience;
            }

            //calculate amount of patience left
            currentPatience -= updateFrequency * reductionRate; //-------------------------------------------------- change here
            //Debug.Log("UpdatePatienceMeter - Current patience " + currentPatience);

            patienceMeterImg.fillAmount = currentPatience / totalPatience;
            //Debug.Log("UpdatePatienceMeter - Patience meter image fill amt " + patienceMeterImg.fillAmount);

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

        Debug.Log("Calling the impatient method");
        if (callback != null)
        {
            callback?.Invoke();
        }

        //add angry customer to evaluation
        //Evaluation_CustomerService.UpdateCustomerServiceStats(0);

        //disable the image
        patienceMeterImg.enabled = false;

        isCoroutineRunning = false;

        yield return null;

    }


    #endregion




    #region Affect customer patience level

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

    #endregion




}