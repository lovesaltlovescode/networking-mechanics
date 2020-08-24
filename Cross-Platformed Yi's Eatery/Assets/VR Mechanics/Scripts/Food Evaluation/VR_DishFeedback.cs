using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_DishFeedback : MonoBehaviour
{
    public void CorrectDish()
    {
        Debug.Log("feedback: correct dish");
    }

    public void WrongDish()
    {
        Debug.Log("feedback: wrong dish");
    }

    public void ServeDish()
    {
        Debug.Log("feedback: serving dish");
    }

    public void SpawnDish()
    {
        Debug.Log("feedback: new dish being spawned");
    }

    public void DisableEffects()
    {
        Debug.Log("disabling feedback now");
    }
}
