using System.Collections;
using System.Collections.Generic;
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
    private static float passingScore_base = 60f;
    private static float passingScore_current = 60f;
    private static int highestLevel = 1;

    #region Getters and Setters
    public static int Level
    {
        get { return level; }
        private set { level = value; }
    }

    public static float PassingScore_base
    {
        get { return passingScore_base; }
        private set { passingScore_base = value; }
    }
    public static float PassingScore_current
    {
        get { return passingScore_current; }
        private set { passingScore_current = value; }
    }
    public static int HighestLevel
    {
        get { return highestLevel; }
        private set { highestLevel = value; }
    }
    #endregion

    //updates level number, the minimum score required to pass, and the highest level they've reached.
    public static void UpdateLevel()
    {
        Level++;
        PassingScore_current = GameBalanceFormulae.increasePassingScore_formula(PassingScore_base, Level);

        if(Level > HighestLevel)
        {
            HighestLevel = Level;
        }
    }

}


//All stats related to how much patience the customer has
public class CustomerPatienceStats
{
    public static float customerPatience_General = GameBalanceFormulae.customerPatience_base_General;
    public static float customerPatience_Queue = GameBalanceFormulae.customerPatience_base_Queue;
    public static float customerPatience_TakeOrder = GameBalanceFormulae.customerPatience_base_TakeOrder;
    public static float customerPatience_FoodWait = GameBalanceFormulae.customerPatience_base_FoodWait;
    public static float customerEatingDuration = 5f;

    public static void UpdateStats()
    {
        customerPatience_General = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_General, LevelStats.Level);
        customerPatience_Queue = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_Queue, LevelStats.Level);
        customerPatience_TakeOrder = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_TakeOrder, LevelStats.Level);
        //customerPatience_FoodWait = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_FoodWait, LevelStats.Level);
    }
}

//formula to calculate the patience of various stats based on the level number
public class GameBalanceFormulae
{
    public static float customerPatience_base_General = 33f;
    public static float customerPatience_base_Queue = 10f;
    public static float customerPatience_base_TakeOrder = 7f;
    public static float customerPatience_base_FoodWait = 120f;

    public static float customerPatience_formula_General(float minNum, float levelNum)
    {        
        return (Mathf.Pow(2, (float)((-1.5 / 5) * levelNum  + 2.4)) * 5 + minNum);
    }
    public static float increasePassingScore_formula(float baseNum, float levelNum)
    {
        return baseNum * levelNum;
    }
}
