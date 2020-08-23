using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Evaluation_OverallPlayerPerformance : NetworkBehaviour
{
    #region Singleton

    private static Evaluation_OverallPlayerPerformance _instance;
    public static Evaluation_OverallPlayerPerformance Instance { get { return _instance; } }

    private void Awake()
    {
        //Debug.Log(this.gameObject.name);

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


    [SyncVar]
    public float customerServiceScore = 0f;
    [SyncVar]
    public float maxCustomerServiceScore = 0f; //total score that could have been achieved by server
    [SyncVar]
    public float cookingScore = 0f;
    [SyncVar]
    public float maxCookingScore = 0f; //total score that could have been achieved by chef
    [SyncVar]
    public float overallScore = 0f;

    //#region Getters and Setters
    //public float CustomerServiceScore
    //{
    //    get { return customerServiceScore; }
    //    private set { customerServiceScore = value; }
    //}

    //public float MaxCustomerServiceScore
    //{
    //    get { return maxCustomerServiceScore; }
    //    private set { maxCustomerServiceScore = value; }
    //}

    //public float CookingScore
    //{
    //    get { return cookingScore; }
    //    private set { cookingScore = value; }
    //}

    //public float MaxCookingScore
    //{
    //    get { return maxCookingScore; }
    //    private set { maxCookingScore = value; }
    //}

    //public float OverallScore
    //{
    //    get { return overallScore; }
    //    private set { overallScore = value; }
    //}
    //#endregion

    //reset all values to zero. to be called at the beginning of a level


    public void ResetAllScores()
    {
        Evaluation_CustomerService.Instance.ResetNumbers_CustomerService();
        customerServiceScore = 0f;
        maxCustomerServiceScore = 0f;

        Evaluation_Cooking.ResetNumbers_Cooking();
        cookingScore = 0f;
        maxCookingScore = 0f;

        overallScore = 0f;
    }

    public float UpdateCustomerServiceScore(float score, bool decrease = false)
    {
        if (decrease)
        {
            customerServiceScore -= score;
        }
        else
        {
            customerServiceScore += score;
        }

        return customerServiceScore;
    }

    public float UpdateMaxCustomerServiceScore(float score)
    {
        maxCustomerServiceScore += score;
        return maxCustomerServiceScore;
    }

    //returns the overall score the entire team attained
    public float CalculateOverallScore()
    {
        //customerServiceScore = Evaluation_CustomerService.CalculateCustomerServiceScore();
        //cookingScore = Evaluation_Cooking.CalculateCookingScore();

        overallScore = customerServiceScore + cookingScore;

        return overallScore;
    }


    //returns the number of stars the player earned
    public int EvaluateScore(float scoreEarned)
    {
        if (scoreEarned >= LevelStats.Instance.threeStarScore_current)
        {
            return 3;
        }
        else if (scoreEarned >= LevelStats.Instance.twoStarScore_current)
        {
            return 2;
        }
        else if (scoreEarned >= LevelStats.Instance.oneStarScore_current)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

}