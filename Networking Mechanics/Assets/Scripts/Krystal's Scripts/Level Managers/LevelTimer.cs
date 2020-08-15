using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class LevelTimer : NetworkBehaviour
{
    #region Singleton

    private static LevelTimer _instance;
    public static LevelTimer Instance { get { return _instance; } }

    public TextMeshProUGUI timerText;

    private void Awake()
    {
        Debug.Log(this.gameObject.name);

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion


    private static bool isPaused = false;

    private float currentTime = 0f;

    [SyncVar]
    private float timeLeft;
    private float levelLength = 240f; //4 minutes long


    public float TimeLeft
    {
        get { return timeLeft; }
        private set { timeLeft = value; }
    }

    private static bool hasLevelEnded = false;
    private bool isCoroutineRunning = false;
    private Coroutine timerCoroutine;

    public static bool IsPaused
    {
        get { return isPaused; }
    }
    public float CurrentTime
    {
        get { return currentTime; }
    }

    //called at the start of the level
    public void StartTimer()
    {
        if (isCoroutineRunning)
        {
            return;
        }

        timeLeft = levelLength;

        isCoroutineRunning = true;

        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    public static void PauseTimer(bool setPauseTrue)
    {
        isPaused = setPauseTrue;
    }

    IEnumerator TimerCoroutine()
    {
        while (true)
        {

            timerText.text = TimingToString();

            //the total amount of time that has passed
            currentTime += Time.deltaTime; //public get, for use elsewhere

            if (!isPaused)
            {
                //time left (to be used to display countdown timer for players)
                timeLeft = levelLength - currentTime;

                if (timeLeft <= 0)
                {
                    EndLevel();

                    break;
                }
            }
            else
            {
                currentTime = levelLength - timeLeft;
            }

            yield return null;
        }

    }

    //returns the time as a string in MM:SS format to be displayed in the clocks
    public string TimingToString()
    {
        if (timeLeft < 0)
        {
            return "00:00";
        }

        //get the time left in minutes and seconds
        string string_min = Mathf.Floor(timeLeft / 60).ToString("00");
        string string_sec = (timeLeft % 60).ToString("00");

        if (string_sec == "60")
        {
            string_min = (Mathf.Floor(timeLeft / 60) + 1).ToString("00");
            string_sec = "00";
        }

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

        hasLevelEnded = true;

        timerText.text = "Dead";

        Debug.Log("level time is up");

        //evaluate player scores
        //call the ui manager, which should have the evaluation screen method. then, pass the following methods into it
        Evaluation_OverallPlayerPerformance.EvaluateScore(Evaluation_OverallPlayerPerformance.CalculateOverallScore());

    }

}
