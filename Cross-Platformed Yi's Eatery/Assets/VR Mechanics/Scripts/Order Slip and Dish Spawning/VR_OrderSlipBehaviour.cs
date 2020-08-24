using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class VR_OrderSlipBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject orderSlipVisualsParent; //parent of the order slip gameobj
    [SerializeField] private Collider orderSlipCollider;
    private ChickenRice orderSlipOrder;

    public ChickenRice OrderSlipOrder
    {
        get { return orderSlipOrder; }
    }
    public GameObject OrderSlipVisualsParent
    {
        get { return orderSlipVisualsParent; }
    }

    //Order slip icons to toggle
    [Header("Toggled icons")]
    [SerializeField] private GameObject[] isChicRoasted = new GameObject[2];
    [SerializeField] private GameObject[] isRicePlain = new GameObject[2];
    [SerializeField] private GameObject[] includesEgg = new GameObject[2];

    //background aging related variables
    [Header("Background Color Changes Over Time")]
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private Color freshColor, oldColor;
    [SerializeField, Range(0, 300)] private float secondsTakenToAge = 60;

    private bool fadeBackground = true, isBackgroundFading = false;
    private Coroutine backgroundFadeCoroutine;

    //background tints to give feedback to player
    [Header("Feedback Colors")]
    [SerializeField] private Color correctTint;
    [SerializeField] private Color wrongTint;




    #region Debug
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            CustomizeOrderSlip(OrderGeneration.Instance.CreateCustomOrder(Random.value > 0.5f, Random.value > 0.5f, Random.value > 0.5f));
            Debug.Log("customizing order slip");
        }
    }
    */
    #endregion

    public void ToggleColliderAndVisuals(bool enable, bool collider = true, bool visuals = true)
    {
        if (collider)
            orderSlipCollider.enabled = enable;

        if (visuals)
            orderSlipVisualsParent.SetActive(enable);
    }


    //assign an order to the order slip and customize its appearance. start aging the background color
    public void CustomizeOrderSlip(ChickenRice order)
    {
        orderSlipOrder = order;

        //enable all the relevant icons
        ToggleIcons(true);

        //orderSlipParent.SetActive(true);

        //start aging the background
        //StartChangingBackgroundColor();
    }



    //reset all the values and stop the aging
    public void ResetOrderSlip()
    {
        //stop aging the background
        if (isBackgroundFading)
        {
            StopChangingBackgroundColor();
        }

        fadeBackground = true;

        //disable the orderSlip
        //orderSlipParent.SetActive(false);

        //disable all the relevant icons
        ToggleIcons(false);

        orderSlipOrder = null;
    }


    #region Methods to change appearance
    //---------------------------------------------------CHANGE THE COLOUR OF THE BACKGROUND OVER TIME----------------------------------------------
    //call the coroutine that changes the background color of the order slip as time passes
    public void StartChangingBackgroundColor()
    {
        if (isBackgroundFading)
        {
            return;
        }

        isBackgroundFading = true;

        backgroundFadeCoroutine = StartCoroutine(ChangeBackgroundColor(freshColor, oldColor, secondsTakenToAge));
    }

    public void StopChangingBackgroundColor()
    {
        if (!isBackgroundFading)
        {
            return;
        }

        isBackgroundFading = false;

        StopCoroutine(backgroundFadeCoroutine);
    }


    //change the colour of the order slip as time passes
    private IEnumerator ChangeBackgroundColor(Color startColor, Color endColor, float totalFadeDuration)
    {
        float timePassed = 0, colorLerpAmt = 0;

        while (colorLerpAmt < 1)
        {
            timePassed += Time.deltaTime;

            if (fadeBackground)
            {
                if (timePassed > totalFadeDuration)
                {
                    timePassed = totalFadeDuration;
                }

                colorLerpAmt = timePassed / totalFadeDuration;

                backgroundSprite.color = Color.Lerp(startColor, endColor, colorLerpAmt);
            }

            yield return null;
        }

        isBackgroundFading = false;

        yield return null;
    }








    //-----------------------------------------------------TOGGLE APPEARANCE OF ORDER SLIP---------------------------------------------------------
    //Toggle icons on order slip depending on order assigned
    private void ToggleIcons(bool makeVisible)
    {
        //toggle relevant icons
        isChicRoasted[Convert.ToInt32(orderSlipOrder.RoastedChic)].SetActive(makeVisible);
        isRicePlain[Convert.ToInt32(orderSlipOrder.RicePlain)].SetActive(makeVisible);
        includesEgg[Convert.ToInt32(orderSlipOrder.HaveEgg)].SetActive(makeVisible);
    }


    //set the background back to its neutral color. resume fading to the old color.
    public void ToggleBackgroundColor()
    {
        fadeBackground = true;

        if (!isBackgroundFading)
        {
            backgroundSprite.color = oldColor;
        }
    }

    //pause background fading to tint the background a specific color
    public void ToggleBackgroundColor(Color newTint)
    {
        fadeBackground = false;
        backgroundSprite.color = newTint;
    }
    #endregion
}
