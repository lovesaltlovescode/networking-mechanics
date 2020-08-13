using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    private static bool isPaused = false;
    private static float currentTime = 0f, timeLeft,
        levelLength = 30f;
    private static bool hasLevelEnded = false;

    public static bool IsPaused
    {
        get { return isPaused; }
    }
    public static float CurrentTime
    {
        get { return currentTime; }
    }

    private void Awake()
    {
        timeLeft = levelLength;
    }

    
    private void Update()
    {
        if (!isPaused)
        {
            //the total amount of time that has passed
            currentTime += Time.deltaTime; //public get, for use elsewhere

            //time left (to be used to display countdown timer for players)
            timeLeft = levelLength - currentTime;

            if(timeLeft <= 0)
            {
                EndLevel();
            }
        }

        #region Debug
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Debug.Log("time left " + TimingToString());
        }
        #endregion
    }

    //returns the time as a string in MM:SS format to be displayed in the clocks
    public static string TimingToString()
    {
        if(timeLeft < 0)
        {
            return "00:00";
        }

        //get the time left in minutes and seconds
        string string_min = Mathf.Floor(timeLeft / 60).ToString("00");
        string string_sec = (timeLeft % 60).ToString("00");

        return string_min + ":" + string_sec;
    }

    //announce that the game is over
    private void EndLevel()
    {
        //used to avoid callnig the method more than once
        if (hasLevelEnded)
        {
            return;
        }

        Debug.Log("level time is up");

        //evaluate player scores
        //call the ui manager, which should have the evaluation screen method. then, pass the following methods into it
        //Evaluation_OverallPlayerPerformance.EvaluateScore(Evaluation_OverallPlayerPerformance.CalculateOverallScore());

        hasLevelEnded = true;
    }

}
