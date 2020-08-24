using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServeDish : MonoBehaviour
{
    public string foodTag = "Food";
    public ParticleSystem successPFX, failurePFX;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision with plate detected");

        if (collision.gameObject.tag == foodTag)
        {
            Debug.Log("food collided ");

            if (collision.gameObject.GetComponent<TempIngredient>().cooking.RateDish() != "burned")
            {
                Debug.Log("food not burned");
                successPFX.Play();
            }
            else
            {
                Debug.Log("food burned");
                failurePFX.Play();
            }
        }
    }

}
