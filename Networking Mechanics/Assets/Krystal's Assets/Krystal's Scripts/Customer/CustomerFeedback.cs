using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerFeedback : MonoBehaviour
{
    [SerializeField] private ParticleSystem eating_PFX, angry_PFX, happy_PFX;

    [Header("Wrong order feedback / Lose points")]
    [SerializeField] private Animator canvasAnim, wordAnim;
    [SerializeField] private TextMeshProUGUI redText;
    [SerializeField] private string wrongOrder = "Wrong order";

    [Header("Right order feedback / Gain points")]
    [SerializeField] private Animator pointsAnim;
    [SerializeField] private TextMeshProUGUI greenText;
    
    [SerializeField] private string loseOneCustomer = "-10", loseTwoCustomers = "-20", loseThreeCustomers = "-30", loseFourCustomers = "-40"; //for queueing customers
    
    [Header("Order served")]
    [SerializeField] private string gain60Points = "+60", gain40Points = "+40", gain10Points = "+10";

    public void Start()
    {
        if (redText && greenText)
        {
            redText.gameObject.SetActive(false);
            greenText.gameObject.SetActive(false);
        }
    }

    #region Queueing customers

    public void CustomerLeaves()
    {
        CustomerBehaviour_Queueing customerBehaviour_Queueing = GetComponent<CustomerBehaviour_Queueing>();

        if (redText)
        {
            switch (customerBehaviour_Queueing.groupSizeNum)
            {
                case 1:
                    StartCoroutine(FadeInFadeOutText(loseOneCustomer, redText));
                    GameManager.Instance.ReduceServerScore(10);
                    break;

                case 2:
                    StartCoroutine(FadeInFadeOutText(loseTwoCustomers, redText));
                    GameManager.Instance.ReduceServerScore(20);
                    break;

                case 3:
                    StartCoroutine(FadeInFadeOutText(loseThreeCustomers, redText));
                    GameManager.Instance.ReduceServerScore(30);
                    break;

                case 4:
                    StartCoroutine(FadeInFadeOutText(loseFourCustomers, redText));
                    GameManager.Instance.ReduceServerScore(40);
                    break;
            }
            
            
        }
    }

    #endregion

    #region Customer Orders

    //feedback that shows that the customer was served the wrong order
    public void WrongOrderServed()
    {
        //Debug.Log("Table feedback: Not enough seats");

        if (redText)
        {
            StartCoroutine(FadeInFadeOutText(wrongOrder, redText));
        }
    }

    public void RightOrderServed()
    {
        CustomerPatience customerPatienceScript = GetComponent<CustomerPatience>();

        if (greenText)
        {
            if (customerPatienceScript.customerMood == CurrentCustomerMood.customerHappy)
            {
                StartCoroutine(FadeInFadeOutText(gain60Points, greenText, true));
                GameManager.Instance.AddServerScore(60);
            }
            else if (customerPatienceScript.customerMood == CurrentCustomerMood.customerImpatient)
            {
                StartCoroutine(FadeInFadeOutText(gain40Points, greenText, true));
                GameManager.Instance.AddServerScore(40);
            }
            else if (customerPatienceScript.customerMood == CurrentCustomerMood.customerAngry)
            {
                StartCoroutine(FadeInFadeOutText(gain10Points, greenText, true));
                GameManager.Instance.AddServerScore(10);
            }
        }
    }

    #endregion

    #region PFX

    public void PlayAngryPFX()
    {
        if (angry_PFX != null)
        {
            if (!angry_PFX.isPlaying)
            {
                angry_PFX.Play();
            }
        }

    }

    public void PlayHappyPFX()
    {
        if (happy_PFX != null)
        {
            happy_PFX.Play();
        }

        //if (eating_PFX)
        //{
        //    eating_PFX.Play();
        //}

    }

    public void PlayEatingPFX(bool play = true)
    {
        if (eating_PFX != null)
        {
            if (play)
            {
                eating_PFX.Play();
            }
            else
            {
                eating_PFX.Stop();
            }

        }

    }

    #endregion


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
