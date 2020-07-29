using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerFeedback : MonoBehaviour
{
    [SerializeField] private ParticleSystem eating_PFX, angry_PFX, happy_PFX;

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

        Debug.Log("Eating pfx called");
    }

    public void PlayAngryPFX()
    {
        if (angry_PFX != null)
        {
            angry_PFX.Play();
        }

        Debug.Log("Eating pfx called");
    }

    public void PlayHappyPFX()
    {
        if (happy_PFX != null)
        {
            happy_PFX.Play();
        }

        Debug.Log("Eating pfx called");
    }
}
