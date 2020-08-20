using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TableFeedback : MonoBehaviour
{
    [SerializeField] private Animator canvasAnim, wordAnim;

    [Header("Points feedback")]
    [SerializeField] private Animator pointsAnim;
    [SerializeField] private TextMeshProUGUI pointsText;


    [Header("ready to order icon")]
    [SerializeField] private GameObject readyToOrderIcon;

    [Header("text to display")]
    [SerializeField] private TextMeshProUGUI word_tmpObj;
    [SerializeField] private string insufficientSeats = "Not enough seats", tableOccupied = "Table occupied", handsFull = "Your hands are full!", tableDirty = "Table dirty", sinkFull = "Sink full";
    [SerializeField] private string gain20Points = "+20", gain10Points = "+10", gain5Points = "+5", lose10Points = "-10";

    [Header("Successfully serve customers")]
    [SerializeField] private string servedOneCustomer = "+50", servedTwoCustomers = "+100", servedThreeCustomers = "+150", servedFourCustomers = "+200";

    #region Debugging
    /*
    private bool tempBool = true;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            NotEnoughSeats();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleOrderIcon(tempBool);
            tempBool = !tempBool;
        }
    }
    */
    #endregion

    private void Start()
    {
        //deactivating all icons / feedback
        readyToOrderIcon.SetActive(false);
        word_tmpObj.gameObject.SetActive(false);
    }

    #region Table Feedback when Seating Customers

    //feedback that shows the table has insufficient seats
    public void NotEnoughSeats()
    {
        //Debug.Log("Table feedback: Not enough seats");

        StartCoroutine(FadeInFadeOutText(insufficientSeats, word_tmpObj));
    }

    //feedback that shows that the table is occupied and no customers can be seated there
    public void TableOccupied()
    {
        //Debug.Log("Table feedback: Not enough seats");

        StartCoroutine(FadeInFadeOutText(tableOccupied, word_tmpObj));
    }

    //feedback that shows that the table is dirty and no customers can be seated there
    public void TableDirty()
    {
        //Debug.Log("Table feedback: Has dirty dishes");

        StartCoroutine(FadeInFadeOutText(tableDirty, word_tmpObj));
    }

    //Feedback that shows that the sink is full
    public void SinkFull()
    {
        StartCoroutine(FadeInFadeOutText(sinkFull, word_tmpObj));
    }

    //feedback that shows that the server's hands are too full to take the customers' order
    public void HandsFullFeedback()
    {
        //Debug.Log("Table feedback: Hands full");

        StartCoroutine(FadeInFadeOutText(handsFull, word_tmpObj));
    }

    #endregion

    #region Points Feedback

    //when customers have been seated
    public void CustomerSeated(string pointsGained)
    {
        StartCoroutine(FadeInFadeOutText(pointsGained, pointsText, true));
    }

    public void CustomerOrderTaken(string pointsGained)
    {
        StartCoroutine(FadeInFadeOutText(pointsGained, pointsText, true));
    }

    public void CustomerLeaves()
    {
        StartCoroutine(FadeInFadeOutText(lose10Points, word_tmpObj));
    }

    public void SuccessfulCustomerService()
    {
        //TODO: Change according to how many customers there are
        StartCoroutine(FadeInFadeOutText(servedOneCustomer, pointsText, true));
    }

    #endregion


    //feedback that shows that the customers are ready to order
    public void ToggleOrderIcon(bool enable)
    {
        if (enable)
        {
            //Debug.Log("Table feedback: customers ready to order");
        }
        else
        {
            //Debug.Log("Table feedback: done ordering");
        }

        //toggle the order icon and make it bob
        readyToOrderIcon.SetActive(enable);
        canvasAnim.SetTrigger("bob");

    }


    //if gain points, play green text anim 
    IEnumerator FadeInFadeOutText(string _text, TextMeshProUGUI textToDisplay, bool _gainPoints = false, bool _fadeIn = true, bool _fadeOut = true)
    {
        textToDisplay.text = _text;

        textToDisplay.gameObject.SetActive(true);

        //make the canvas rise
        canvasAnim.SetTrigger("rise");


        if (_fadeIn)
        {
            if (_gainPoints)
            {
                pointsAnim.SetBool("fadeIn", true);
                //Debug.Log("fade in bool set to true");
                yield return null;

                //Debug.Log("fade in clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitForSeconds(pointsAnim.GetCurrentAnimatorStateInfo(0).length);
            }
            else
            {

                wordAnim.SetBool("fadeIn", true);
                //Debug.Log("fade in bool set to true");
                yield return null;

                //Debug.Log("fade in clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitForSeconds(wordAnim.GetCurrentAnimatorStateInfo(0).length);
            }


        }

        if (_fadeOut)
        {
            if (_gainPoints)
            {
                pointsAnim.SetBool("fadeIn", false);
                //Debug.Log("fade in bool set to false");
                yield return null;

                //Debug.Log("fade out clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitForSeconds(pointsAnim.GetCurrentAnimatorStateInfo(0).length);
            }
            else
            {
                wordAnim.SetBool("fadeIn", false);
                //Debug.Log("fade in bool set to false");
                yield return null;

                //Debug.Log("fade out clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitForSeconds(wordAnim.GetCurrentAnimatorStateInfo(0).length);
            }


        }

        textToDisplay.gameObject.SetActive(false);
        // Debug.Log("set words to false");

        yield return null;
    }




}
