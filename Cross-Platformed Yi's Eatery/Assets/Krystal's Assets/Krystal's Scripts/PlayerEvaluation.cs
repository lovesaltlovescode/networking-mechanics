using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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