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
    [SerializeField] private string wrongOrder = "Wrong order -20";

    [Header("Right order feedback / Gain points")]
    [SerializeField] private Animator pointsAnim;
    [SerializeField] private TextMeshProUGUI greenText;
    
    [SerializeField] private string loseOneCustomer = "-10", loseTwoCustomers = "-20", loseThreeCustomers = "-30", loseFourCustomers = "-40"; //for queueing customers
    
    [Header("Order served")]
    [SerializeField] private string gain40Points = "+60", gain20Points = "+40", gain10Points = "+10";

    [Header("Audio Feedback")]
    [SerializeField] private AudioSource customerAudioSource;
    [SerializeField] private AudioClip angrySFX;
    [SerializeField] private AudioClip happySFX;
    [SerializeField] private AudioClip enterSFX; //played when customer enter shop

    public void Start()
    {
        if (redText && greenText)
        {
            redText.gameObject.SetActive(false);
            greenText.gameObject.SetActive(false);
        }
    }

    #region Queueing customers

    public void EnterShop()
    {
        if (!enterSFX)
        {
            return;
        }

        customerAudioSource.PlayOneShot(enterSFX);
    }

    public void CustomerLeaves()
    {
        CustomerBehaviour_Queueing customerBehaviour_Queueing = GetComponent<CustomerBehaviour_Queueing>();
        CustomerPatience customerPatienceScript = GetComponent<CustomerPatience>();

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

            GameManager.Instance.DecreaseMood(5);
            Evaluation_CustomerService.Instance.UpdateNumCustomersServed(1, true);
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
            GameManager.Instance.ReduceServerScore(20);
            GameManager.Instance.DecreaseMood(5);
        }
    }

    public void RightOrderServed()
    {
        CustomerPatience customerPatienceScript = GetComponent<CustomerPatience>();

        if (greenText)
        {
            if (customerPatienceScript.customerMood == CurrentCustomerMood.customerHappy)
            {
                StartCoroutine(FadeInFadeOutText(gain40Points, greenText, true));
                GameManager.Instance.AddServerScore(40);
                GameManager.Instance.IncreaseMood(5);
            }
            else if (customerPatienceScript.customerMood == CurrentCustomerMood.customerImpatient)
            {
                StartCoroutine(FadeInFadeOutText(gain20Points, greenText, true));
                GameManager.Instance.AddServerScore(20);
                GameManager.Instance.IncreaseMood(2);
            }
            else if (customerPatienceScript.customerMood == CurrentCustomerMood.customerAngry)
            {
                StartCoroutine(FadeInFadeOutText(gain10Points, greenText, true));
                GameManager.Instance.AddServerScore(10);
            }

            Evaluation_OverallPlayerPerformance.Instance.UpdateMaxCustomerServiceScore(40);
        }
    }

    #endregion

    #region PFX

    public void PlayAngryPFX()
    {
        if (angry_PFX != null && angrySFX != null)
        {
            if (!angry_PFX.isPlaying)
            {
                angry_PFX.Play();
                customerAudioSource.PlayOneShot(angrySFX);
            }
        }

    }

    public void PlayHappyPFX()
    {
        if (happy_PFX != null && happySFX != null)
        {
            happy_PFX.Play();
            customerAudioSource.PlayOneShot(happySFX);
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
