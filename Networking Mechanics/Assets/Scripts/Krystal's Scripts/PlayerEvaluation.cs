using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation_OverallPlayerPerformance
{
    private static float customerServiceScore = 0f;
    private static float cookingScore = 0f;
    private static float overallScore = 0f;

    #region Getters and Setters
    public static float CustomerServiceScore
    {
        get { return customerServiceScore; }
        private set { customerServiceScore = value; }
    }

    public static float CookingScore
    {
        get { return cookingScore; }
        private set { cookingScore = value; }
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

        Evaluation_Cooking.ResetNumbers_Cooking();
        cookingScore = 0f;

        overallScore = 0f;
    }

    //returns the overall score the entire team attained
    public static float CalculateOverallScore()
    {
        customerServiceScore = Evaluation_CustomerService.CalculateCustomerServiceScore();
        cookingScore = Evaluation_Cooking.CalculateCookingScore();

        overallScore = (customerServiceScore + cookingScore) / 2;

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
    private static int numCustomersServedSuccessfully = 0, //num of customers served their food (regardless of whether they waited too long for their food)
        numHappyCustomers = 0,
        numNeutralCustomers = 0,
        numAngryCustomers = 0, //number of customers that left after waiting too long (regardless of whether they left the restaurant or continue sitting and waiting for their food)
        totalNumInteractions = 0; //total num of customer interactions

    private static float happy_minPatience = 0.4f; //min patience level the customers have to have when served to be considered happy

    private static float restaurantMood = 5f; //WIP. tracks the overall level of patience of customers

    private static float totalServingSpeed = 0f, averageServingSpeed = 0f;

    #region Getters and Setters
    public static int NumCustomersServedSuccessfully
    {
        get { return numCustomersServedSuccessfully; }
        private set { numCustomersServedSuccessfully = value; }
    }

    public static int NumHappyCustomers
    {
        get { return numHappyCustomers; }
        private set { numHappyCustomers = value; }
    }
    public static int NumNeutralCustomers
    {
        get { return numNeutralCustomers; }
        private set { numNeutralCustomers = value; }
    }
    public static int NumAngryCustomers
    {
        get { return numAngryCustomers; }
        private set { numAngryCustomers = value; }
    }

    public static float RestaurantMood
    {
        get { return restaurantMood; }
        private set { restaurantMood = value; }
    }

    public static float AverageServingSpeed
    {
        get { return averageServingSpeed; }
        private set { averageServingSpeed = value; }
    }
    #endregion


    //update the number of customers served their food
    //call when a customer has been served their food, regardless of their patience level
    public static void IncreaseCustomersServed()
    {
        NumCustomersServedSuccessfully++;
    }

    //updates the number of customers served, the mood they were in and the avg serving speed
    //call when a customer has been interacted with (seated, order taken, food served)
    public static void UpdateCustomerServiceStats(float fractionOfPatienceLeft)
    {
        if (fractionOfPatienceLeft >= happy_minPatience)
        {
            numHappyCustomers++;
        }
        else if (fractionOfPatienceLeft > 0)
        {
            numNeutralCustomers++;
        }
        else
        {
            numAngryCustomers++;
        }

        //calculating the average serving speed
        totalServingSpeed += fractionOfPatienceLeft;
        totalNumInteractions = numHappyCustomers + numNeutralCustomers + NumAngryCustomers;

        averageServingSpeed = totalServingSpeed / totalNumInteractions;
    }


    //calculates the quality of customer service based on the speed at which the customers were served, 
    // the mood of the customers, the number of customers that left angrily and so on.
    public static float CalculateCustomerServiceScore()
    {
        Debug.Log("calculating customer service score...");
        return 0;
    }


    //resets all the values to their defaults at the beginning of the level
    public static void ResetNumbers_CustomerService()
    {
        numCustomersServedSuccessfully = 0;
        numHappyCustomers = 0;
        numNeutralCustomers = 0;
        numAngryCustomers = 0;
        totalNumInteractions = 0;

        totalServingSpeed = 0f;
        averageServingSpeed = 0f;

        restaurantMood = 5f;
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
    public static void increaseWastedIngredients()
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