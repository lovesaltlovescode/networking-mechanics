using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerFeedback : MonoBehaviour
{
    [SerializeField] private ParticleSystem eating_PFX, angry_PFX, happy_PFX;

    [Header("Wrong order feedback")]
    [SerializeField] private Animator canvasAnim, wordAnim;
    [SerializeField] private TextMeshProUGUI word_tmpObj;
    [SerializeField] private string wrongOrder = "Wrong order";

    public void Start()
    {
        if (word_tmpObj)
        {
            word_tmpObj.gameObject.SetActive(false);
        }
    }

    public void PlayEatingPFX(bool play = true)
    {
        if(eating_PFX != null)
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


    //feedback that shows that the customer was served the wrong order
    public void WrongOrderServed()
    {
        //Debug.Log("Table feedback: Not enough seats");

        if (word_tmpObj)
        {
            StartCoroutine(FadeInFadeOutText(wrongOrder));
        }
    }

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


    IEnumerator FadeInFadeOutText(string _text, bool _fadeIn = true, bool _fadeOut = true)
    {
        word_tmpObj.text = _text;
        word_tmpObj.gameObject.SetActive(true);

        //make the canvas rise
        canvasAnim.SetTrigger("rise");

        if (_fadeIn)
        {
            wordAnim.SetBool("fadeIn", true);
            //Debug.Log("fade in bool set to true");
            yield return null;

            //Debug.Log("fade in clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

            yield return new WaitForSeconds(wordAnim.GetCurrentAnimatorStateInfo(0).length);

        }

        if (_fadeOut)
        {
            wordAnim.SetBool("fadeIn", false);
            //Debug.Log("fade in bool set to false");
            yield return null;

            //Debug.Log("fade out clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

            yield return new WaitForSeconds(wordAnim.GetCurrentAnimatorStateInfo(0).length);

        }

        word_tmpObj.gameObject.SetActive(false);
        // Debug.Log("set words to false");

        yield return null;
    }
}
