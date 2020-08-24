using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanScript : MonoBehaviour
{
    //public GameObject panMesh;
    public GameObject cookTimer, burnTimer;
    public GameObject oilLiquid;
    public float minDiameter = 0.05f;
    public ParticleSystem smokePFX;
    public bool isOilInPan = false;

    private TempIngredient ingredientInfo;
    Coroutine cookingStarted;


    private void Update()
    {
        if(oilLiquid.transform.localScale.x >= minDiameter)
        {
            isOilInPan = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered pan trigger zone");

        if (other.GetComponent<TempIngredient>())
        {
            ingredientInfo = other.GetComponent<TempIngredient>();
            Debug.Log("is oil in pan? " + isOilInPan);
            bool tempBool = ingredientInfo.CheckMat() == ingredientInfo.cookingMat;
            Debug.Log("is pancake alr burnt? " + tempBool);
            Debug.Log("cookingMat: " + ingredientInfo.cookingMat.name);
            Debug.Log("checkMat: " + ingredientInfo.CheckMat().name);

            //if oil has bee nadded tothe pan and the pancake is not already burnt
            if (isOilInPan && ingredientInfo.cooking.State != "burned")
            {
                Debug.Log("start cooking raw pancake");
                cookTimer.GetComponent<TimerUI>().ToggleHelper(true);
                cookingStarted = StartCoroutine(Cooking(cookTimer.GetComponent<TimerUI>(), ingredientInfo.heatingTime));

            } else
            {
                Debug.Log("Burning!");
                StartBurning();
            }
        }


    }


    private void OnTriggerExit(Collider other)
    {
        if(ingredientInfo != null)
        {
            if (other.gameObject == ingredientInfo.gameObject)
            {
                if (cookingStarted != null)
                {
                    StopCoroutine(cookingStarted);
                }

                smokePFX.Stop();
            }
        }

    }


    private void StartBurning()
    {
        if(cookingStarted != null)
        {
            StopCoroutine(cookingStarted);
        }

        //burnTimer.GetComponent<TimerUI>().ToggleHelper(true);
        Debug.Log("Calling switch state bc pancake is burning. Has burnTimer been assigned? " + burnTimer.GetComponent<TimerUI>().randoString);
        ingredientInfo.SwitchState(ingredientInfo.burningMat, burnTimer.GetComponent<TimerUI>());

        ingredientInfo.cooking.State = "burned";

        smokePFX.Play();

        ingredientInfo.isBurning = true;
        cookingStarted = StartCoroutine(Cooking(burnTimer.GetComponent<TimerUI>(), ingredientInfo.burnTime));
    }

    public IEnumerator Cooking(TimerUI timer, float timerFullAmt)
    {
        var curTime = timerFullAmt;

        while (curTime > 0)
        {
            timer.UpdateProcessUI(curTime, timerFullAmt);

            ingredientInfo.GetComponent<Renderer>().material.SetFloat("_Blend", timer.timerFill.fillAmount);

            curTime -= Time.deltaTime;
            ingredientInfo.cooking.TimeHeated += Time.deltaTime;

            yield return null;
        }

        //if the pancake is not alr burnt, start burning the pancake
        if (!ingredientInfo.isBurning)
        {
            StartBurning();
        }

        timer.ToggleHelper(false);

    }

    

}
