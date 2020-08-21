using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation_OverallPlayerPerformance
{
    private static float customerServiceScore = 0f;
    private static float maxCustomerServiceScore = 0f; //total score that could have been achieved by server
    private static float cookingScore = 0f;
    private static float maxCookingScore = 0f; //total score that could have been achieved by chef
    private static float overallScore = 0f;

    #region Getters and Setters
    public static float CustomerServiceScore
    {
        get { return customerServiceScore; }
        private set { customerServiceScore = value; }
    }

    public static float MaxCustomerServiceScore
    {
        get { return maxCustomerServiceScore; }
        private set { maxCustomerServiceScore = value; }
    }

    public static float CookingScore
    {
        get { return cookingScore; }
        private set { cookingScore = value; }
    }

    public static float MaxCookingScore
    {
        get { return maxCookingScore; }
        private set { maxCookingScore = value; }
    }

    public static float OverallScore
    {
        get { return overallScore; }
        private set { overallScore = value; }
    }
    #endregion

    //reset all values to zero. to be called at the beginning of a level
    public static void ResetAllScores()
    {
        Evaluation_CustomerService.ResetNumbers_CustomerService();
        customerServiceScore = 0f;
        maxCustomerServiceScore = 0f;

        Evaluation_Cooking.ResetNumbers_Cooking();
        cookingScore = 0f;
        maxCookingScore = 0f;

        overallScore = 0f;
    }

    public static float UpdateCustomerServiceScore(float score, bool decrease = false)
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

    public static float UpdateMaxCustomerServiceScore(float score)
    {
        maxCustomerServiceScore += score;
        return maxCustomerServiceScore;
    }

    //returns the overall score the entire team attained
    public static float CalculateOverallScore()
    {
        //customerServiceScore = Evaluation_CustomerService.CalculateCustomerServiceScore();
        //cookingScore = Evaluation_Cooking.CalculateCookingScore();

        overallScore = customerServiceScore + cookingScore;

        return overallScore;
    }


    //returns the number of stars the player earned
    public static int EvaluateScore(float scoreEarned)
    {
        if (scoreEarned >= LevelStats.ThreeStarScore_current)
        {
            return 3;
        }
        else if (scoreEarned >= LevelStats.TwoStarScore_current)
        {
            return 2;
        }
        else if (scoreEarned >= LevelStats.OneStarScore_current)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

}


public class Evaluation_CustomerService
{
    private static float numCustomersServed = 0f;
    private static float numCustomersLost = 0f;

    #region Getters and Setters

    public static float NumCustomersServed
    {
        get { return numCustomersServed; }
        private set { numCustomersServed = value; }
    }

    public static float NumCustomersLost
    {
        get { return numCustomersLost; }
        private set { numCustomersLost = value; }
    }


    #endregion

    public static void UpdateNumCustomersServed(float customers, bool decrease = false)
    {
        if (decrease)
        {
            numCustomersLost += customers;
        }
        else
        {
            numCustomersServed += customers;
        }

        Debug.Log("Customers lost " + numCustomersLost);
    }

    //calculates the quality of customer service based on the speed at which the customers were served, 
    // the mood of the customers, the number of customers that left angrily and so on.
    public static float CalculateCustomerServiceScore()
    {
        Debug.Log("calculating customer service score...");
        float customerServiceScoreAttained =
            (Evaluation_OverallPlayerPerformance.CustomerServiceScore / Evaluation_OverallPlayerPerformance.MaxCustomerServiceScore) * 100;

        return customerServiceScoreAttained;
    }

    //resets all the values to their defaults at the beginning of the level
    public static void ResetNumbers_CustomerService()
    {
        numCustomersServed = 0;
        numCustomersLost = 0;
    }
}

public class Evaluation_Cooking
{

    private static float servedFoodQuality_avg = 0f,
                        servedFoodQuality_total = 0f; //the chef's cooking evaluation score
    private static int numServedDishes = 0; //total num of dishes the chef has served
    private static int numWastedIngredients = 0; //num of ingredients that have burnt / rotted / eaten / thrown away by chef

    #region Getters and Setters
    public static int NumWastedIngredients
    {
        get { return numWastedIngredients; }
        private set { numWastedIngredients = value; }
    }
    public static float ServedFoodQuality_avg
    {
        get { return servedFoodQuality_avg; }
        private set { servedFoodQuality_avg = value; }
    }
    public static int NumServedDishes
    {
        get { return numServedDishes; }
        private set { numServedDishes = value; }
    }
    #endregion

    //updates the number of ingredients burnt / rotted / eaten / thrown away by chef
    public static void IncreaseWastedIngredients()
    {
        numWastedIngredients++;
    }

    //updates the average quality of the chef's food
    public static void UpdateAverageFoodQuality(float newServedFoodQuality)
    {
        servedFoodQuality_total += newServedFoodQuality;
        numServedDishes++;

        servedFoodQuality_avg = newServedFoodQuality / numServedDishes;
    }

    //calculates the overall score the chef attained, taking into account the quality of his food, 
    // the speed of his cooking and the num of wasted ingredients
    public static float CalculateCookingScore()
    {
        Debug.Log("calculating cooking score...");
        return 0;
    }


    //resets all the values to their defaults at the beginning of the level
    public static void ResetNumbers_Cooking()
    {
        numWastedIngredients = 0;
        servedFoodQuality_total = 0f;
        servedFoodQuality_avg = 0f;
        numServedDishes = 0;
    }
}