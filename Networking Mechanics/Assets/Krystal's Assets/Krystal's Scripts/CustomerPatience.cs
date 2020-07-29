/*
 *  Usage: attach to parent of customer prefab
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerPatience : MonoBehaviour
{
    //variables
    [SerializeField] private float updateFrequency = 0.2f;
    [SerializeField] private Image patienceMeterImg;
    [SerializeField] private Color finalColor = Color.red;
    private Color startColor;

    private Coroutine patienceMeterCoroutine;
    private bool isCoroutineRunning = false; //bool used to ensure that coroutine does not get called while coroutine is running

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

        StopCoroutine(patienceMeterCoroutine);
    }


    //method that updates customers' patience meter, then, when patience runs out, calls the method (callback) passed into it 
    //understanding callbacks: https://stackoverflow.com/questions/54772578/passing-a-function-as-a-function-parameter/54772707
    private IEnumerator UpdatePatienceMeter(float totalPatience, Action callback = null, bool changeColor = true)
    {
        float currentPatience = totalPatience;

        //enable the patience meter img so player can see
        patienceMeterImg.enabled = true;

        while (currentPatience > 0)
        {
            //calculate amount of patience left
            currentPatience -= updateFrequency;
            patienceMeterImg.fillAmount = currentPatience / totalPatience;

            if (changeColor)
            {
                patienceMeterImg.color = Color.Lerp(finalColor, startColor, currentPatience / totalPatience);
            }

            yield return new WaitForSeconds(updateFrequency);
        }

        Debug.Log("Calling the impatient method");
        if (callback != null)
        {
            callback?.Invoke();
        }

        //disable the image
        patienceMeterImg.enabled = false;

        isCoroutineRunning = false;

        yield return null;
    }


}
