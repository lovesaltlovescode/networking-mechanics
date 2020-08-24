using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Image timerFill;
    public string randoString = "yes";

    private void Start()
    {
        timerFill = gameObject.GetComponent<Image>();
    }

    //update the timer
    public void UpdateProcessUI(float curAmount, float totalProcess)
    {
        if (timerFill != null)
            timerFill.fillAmount = 1 - (curAmount / totalProcess);

    }

    //toggle the UI
    public void ToggleHelper(bool shouldBeVisible)
    {
        Debug.Log("timer set to " + shouldBeVisible);

        gameObject.GetComponent<Image>().enabled = shouldBeVisible;
        gameObject.transform.GetChild(0).GetComponent<Image>().enabled = shouldBeVisible;

        timerFill.fillAmount = 0;
    }

}
