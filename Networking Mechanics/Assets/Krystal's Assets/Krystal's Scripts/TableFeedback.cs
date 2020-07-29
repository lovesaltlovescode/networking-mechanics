using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TableFeedback : MonoBehaviour
{
    [SerializeField] private Animator canvasAnim, wordAnim;

    [Header("ready to order icon")]
    [SerializeField] private GameObject readyToOrderIcon;

    [Header("text to display")]
    [SerializeField] private TextMeshProUGUI word_tmpObj; 
    [SerializeField] private string insufficientSeats = "Not enough seats", tableOccupied = "Table occupied", handsFull = "Your hands are full!";


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


    //feedback that shows the table has insufficient seats
    public void NotEnoughSeats()
    {
        Debug.Log("Table feedback: Not enough seats");

        StartCoroutine(FadeInFadeOutText(insufficientSeats));
    }

    //feedback that shows that the table is occupied and no customers can be seated there
    public void TableOccupied()
    {
        Debug.Log("Table feedback: Not enough seats");

        StartCoroutine(FadeInFadeOutText(tableOccupied));
    }

    //feedback that shows that the server's hands are too full to take the customers' order
    public void HandsFullFeedback()
    {
        Debug.Log("Table feedback: Hands full");

        StartCoroutine(FadeInFadeOutText(handsFull));
    }


    //feedback that shows that the customers are ready to order
    public void ToggleOrderIcon(bool enable)
    {
        if (enable)
        {
            Debug.Log("Table feedback: customers ready to order");
        }
        else
        {
            Debug.Log("Table feedback: done ordering");
        }

        //toggle the order icon and make it bob
        readyToOrderIcon.SetActive(enable);
        canvasAnim.SetTrigger("bob");

    }


    IEnumerator FadeInFadeOutText(string _text, bool _fadeIn = true, bool _fadeOut = true)
    {
        word_tmpObj.text = _text;
        word_tmpObj.gameObject.SetActive(true);

        //make the canvas rise
        canvasAnim.SetTrigger("rise");

        if (_fadeIn)
        {
            wordAnim.SetBool("fadeIn", true);
            Debug.Log("fade in bool set to true");
            yield return null;

            Debug.Log("fade in clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

            yield return new WaitForSeconds(wordAnim.GetCurrentAnimatorStateInfo(0).length);

        }

        if (_fadeOut)
        {
            wordAnim.SetBool("fadeIn", false);
            Debug.Log("fade in bool set to false");
            yield return null;

            Debug.Log("fade out clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

            yield return new WaitForSeconds(wordAnim.GetCurrentAnimatorStateInfo(0).length);

        }

        word_tmpObj.gameObject.SetActive(false);
        Debug.Log("set words to false");

        yield return null;
    }




}
