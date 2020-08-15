using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

//All stats related to the level number and score
public class LevelStats : MonoBehaviour
{
    #region Singleton

    private static LevelStats _instance;
    public static LevelStats Instance { get { return _instance; } }

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

    //private fields
    private static int level = 1;
    private static int highestLevel = 1;

    private static float oneStarScore_current = UpdatePassingScore(),
        twoStarScore_current, threeStarScore_current;

    #region Getters and Setters
    public static int Level
    {
        get { return level; }
        private set { level = value; }
    }
    public static int HighestLevel
    {
        get { return highestLevel; }
        private set { highestLevel = value; }
    }
    public static float OneStarScore_current
    {
        get { return oneStarScore_current; }
        private set { oneStarScore_current = value; }
    }
    public static float TwoStarScore_current
    {
        get { return twoStarScore_current; }
        private set { twoStarScore_current = value; }
    }
    public static float ThreeStarScore_current
    {
        get { return threeStarScore_current; }
        private set { threeStarScore_current = value; }
    }

    #endregion

    //updates level number, the minimum score required to pass, and the highest level they've reached.
    public static void UpdateLevel()
    {
        Level++;

        OneStarScore_current = UpdatePassingScore();

        if (Level > HighestLevel)
        {
            HighestLevel = Level;
        }
    }

    //updates the passing score and the higher achievements
    private static float UpdatePassingScore()
    {
        float currentPassingScore = GameBalanceFormulae.increaseOneStarScore_formula(Level);

        twoStarScore_current = currentPassingScore * 2;
        threeStarScore_current = currentPassingScore * 4;

        return currentPassingScore;
    }

}

#region unchanged class
//All stats related to how much patience the customer has
public class CustomerPatienceStats
{
    public static float customerPatience_General = GameBalanceFormulae.customerPatience_base_General;
    public static float customerPatience_Queue = GameBalanceFormulae.customerPatience_base_Queue;
    public static float customerPatience_TakeOrder = GameBalanceFormulae.customerPatience_base_TakeOrder;
    public static float customerPatience_FoodWait = GameBalanceFormulae.customerPatience_base_FoodWait;
    public static float customerEatingDuration = 5f;
    public static float drinkPatienceIncrease = 5f;

    public static void UpdateStats()
    {
        customerPatience_General = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_General, LevelStats.Level);
        customerPatience_Queue = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_Queue, LevelStats.Level);
        customerPatience_TakeOrder = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_TakeOrder, LevelStats.Level);
        //customerPatience_FoodWait = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_FoodWait, LevelStats.Level);
    }
}
#endregion

//formula to calculate the patience of various stats based on the level number
public class GameBalanceFormulae
{
    public static float customerPatience_base_General = 33f;
    public static float customerPatience_base_Queue = 10f;
    public static float customerPatience_base_TakeOrder = 7f;
    public static float customerPatience_base_FoodWait = 120f;

    private static float oneStarScore_base = 60f; //amount of points needed to earn one star (the passing score)
    private static float oneStarScore_max = 500f;
    public static float OneStarScore_base
    {
        get { return oneStarScore_base; }
    }
    public static float OneStarScore_max
    {
        get { return oneStarScore_max; }
    }

    public static float customerPatience_formula_General(float minNum, float levelNum)
    {
        return (Mathf.Pow(2, (float)((-1.5 / 5) * levelNum + 2.4)) * 5 + minNum);
    }
    public static float increaseOneStarScore_formula(float levelNum)
    {
        float newOneStarScore = oneStarScore_base * levelNum;

        if (newOneStarScore <= OneStarScore_max)
        {
            return newOneStarScore;
        }
        else
        {
            return OneStarScore_max;
        }
    }

}
