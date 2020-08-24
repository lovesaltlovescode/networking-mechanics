using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingProgress 
{
    //fields
    private float timeHeated = 0;
    private float optimalSize = 0;


    public float size;
    private string state;


    public float TimeHeated
    {
        get { return timeHeated; }
        set { timeHeated = value; }
    }

    public string State
    {
        get { return state; }
        set
        {
            if ( value == "new" ||
                value == "heated" ||
                value == "burned")
            {
                state = value;
            }
            else
            {
                state = "Unknown";
            }
        }
    }

    public float Size
    {
        get { return size; }
        set { size = value; }
    }

    public CookingProgress(float timeHeated = 0f, string state = "new", float size = 0f)
    {
        this.TimeHeated = timeHeated;
        this.State = state;
        this.Size = size;
    }

    //Compare the optimal values associated with ingredient and the values to get the final score
    public string RateDish(/*float optimalHeatingTime, Vector3 optimalSizeFraction*/)
    {
        if(State != "burned")
        {
            Debug.Log("state: " + State);
        } else
        {
            Debug.Log("yikes, it's burned");
        }
        return State;
    }
}
