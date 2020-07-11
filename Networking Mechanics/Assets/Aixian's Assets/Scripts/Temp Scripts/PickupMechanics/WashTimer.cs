using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handle timer for washing
//Animate timer image
public class WashTimer : MonoBehaviour
{

    private Image washTimer; //image to be filled
    public Image washIcon; //image that will be greyed
    private float waitTime = 4f; //time to wait until image is filled

    private void Start()
    {
        washTimer = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //only start timer if wash icn is grayed out
        if (washIcon.color == Color.gray)
        {
            StartTimer();
        }
        else
        {
            //fill amount always 0
            washTimer.fillAmount = 0;
        }

    }

    public void StartTimer()
    {
        //Increase fill amount over waittime seconds
        washTimer.fillAmount += 1.0f / waitTime * Time.deltaTime;
    }
}