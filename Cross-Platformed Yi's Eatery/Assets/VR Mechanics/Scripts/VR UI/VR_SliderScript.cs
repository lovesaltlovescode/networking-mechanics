using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_SliderScript : MonoBehaviour
{
    [SerializeField] private GameObject sliderVisuals;
    [SerializeField] private Transform leftPoint, rightPoint, sliderIndicator;
    //[SerializeField] private float updateFrequencyInSec = 1/30f;

    private Vector3 leftPoint_pos, rightPoint_pos;
    private float distBetweenPoints = 0;

    private Coroutine updateSliderCoroutine;
    private bool isCoroutineRunning = false; //bool used to ensure that coroutine does not get called while coroutine is running

    private float currentPercentage = 0f;

    private float timePassed = 0f;

    private void Awake()
    {
        GetStartAndEndPos();

        ToggleVisibility(false);
    }


    #region Debug Commands

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartSlider(true, 5, null, true);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            StartSlider(false, 5, null);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            StopSlider();
        }
    }

    #endregion


    //start and enable the slider
    public void StartSlider(bool _pingPong, float _durationInSec, Action _callback = null, bool randomStartValue = false)
    {
        if (isCoroutineRunning) //ensures that the coroutine is not called twice
            return;

        isCoroutineRunning = true;

        //set the start position of the slider indicator
        if (randomStartValue)
        {
            currentPercentage = UnityEngine.Random.Range(0.0f, 0.9f);
        }
        else
        {
            currentPercentage = 0;
        }

        sliderIndicator.localPosition = Vector3.Lerp(leftPoint_pos, rightPoint_pos, currentPercentage);

        Debug.Log("starting current percentage: " + currentPercentage);

        ToggleVisibility(true);

        //start the coroutine
        updateSliderCoroutine = StartCoroutine(UpdateSlider(_pingPong, _durationInSec, _callback));
    }


    //stop the slider prematurely
    public void StopSlider()
    {
        if (!isCoroutineRunning) //ensures that the coroutine is not called twice
            return;

        isCoroutineRunning = false;

        ToggleVisibility(false);

        Debug.Log("ending current percentage: " + ReturnPercentage());

        //start the coroutine
        StopCoroutine(updateSliderCoroutine);
    }



    IEnumerator UpdateSlider(bool pingPong, float durationInSec, Action callback = null)  
    {
        Vector3 startLerpPos = leftPoint_pos;
        Vector3 endLerpPos = rightPoint_pos;

        timePassed = 0f;

        while (true)
        {
            Debug.Log("while true");
            yield return null;

            while (currentPercentage < 1)
            {
                timePassed += Time.deltaTime;

                currentPercentage = timePassed / durationInSec;

                if(currentPercentage > 1)
                {
                    currentPercentage = 1;
                }

                //move the slider to indicate the current percentage
                sliderIndicator.localPosition = Vector3.Lerp(startLerpPos, endLerpPos, currentPercentage);

                yield return null;
            }
            Debug.Log("Time passed since coroutine started: " + timePassed);

            if (pingPong)
            {
                if(startLerpPos == leftPoint_pos)
                {
                    startLerpPos = rightPoint_pos;
                    endLerpPos = leftPoint_pos;

                    currentPercentage = 0;
                }
                else
                {
                    startLerpPos = leftPoint_pos;
                    endLerpPos = rightPoint_pos;

                    currentPercentage = 0;
                }
            }
            else
            {
                break;
            }
        }

        isCoroutineRunning = false;

        if(callback != null)
        {
            callback?.Invoke();
        }

        ToggleVisibility(false);
        Debug.Log("coroutine ended");

        yield return null;
    }


    //returns the current percentage of the slider for evaluation
    public float ReturnPercentage()
    {
        return currentPercentage;
    }

    //get the positions the slider indicator has to move between
    private void GetStartAndEndPos()
    {
        leftPoint_pos = leftPoint.localPosition;
        rightPoint_pos = rightPoint.localPosition;

        distBetweenPoints = Vector3.Distance(leftPoint_pos, rightPoint_pos);
    }

    //toggle the visibility of the visual elements of the slider
    private void ToggleVisibility(bool enable)
    {
        sliderVisuals.SetActive(enable);
    }

}
