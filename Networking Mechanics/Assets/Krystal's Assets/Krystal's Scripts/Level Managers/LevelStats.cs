using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using Mirror;

//All stats related to the level number and score
public class LevelStats : NetworkBehaviour
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
    [SyncVar]
    public int level = 1;

    [SyncVar]
    public int highestLevel = 1;

    [SyncVar]
    public float oneStarScore_current,
        twoStarScore_current, threeStarScore_current;

    //#region Getters and Setters
    //public static int Level
    //{
    //    get { return level; }
    //    private set { level = value; }
    //}
    //public static int HighestLevel
    //{
    //    get { return highestLevel; }
    //    private set { highestLevel = value; }
    //}
    //public static float OneStarScore_current
    //{
    //    get { return oneStarScore_current; }
    //    private set { oneStarScore_current = value; }
    //}
    //public static float TwoStarScore_current
    //{
    //    get { return twoStarScore_current; }
    //    private set { twoStarScore_current = value; }
    //}
    //public static float ThreeStarScore_current
    //{
    //    get { return threeStarScore_current; }
    //    private set { threeStarScore_current = value; }
    //}


    //#endregion

    private void Start()
    {
        oneStarScore_current = UpdatePassingScore();
    }

    [Header("Evaluation")]
    [SerializeField] private GameObject evaluationScreen;

    //Spawn evaluation canvas
    [ServerCallback]
    public void ShowEvaluationScreen()
    {
        GameObject spawnedEvaluationScreen = Instantiate(evaluationScreen, evaluationScreen.transform.position, evaluationScreen.transform.rotation);
        spawnedEvaluationScreen.GetComponent<LevelEvaluation>().UpdateEvaluationValues();
        NetworkServer.Spawn(spawnedEvaluationScreen);

    }

    //updates level number, the minimum score required to pass, and the highest level they've reached.
    public void UpdateLevel()
    {
        level++;

        oneStarScore_current = UpdatePassingScore();

        if (level > highestLevel)
        {
            highestLevel = level;
        }
    }

    //updates the passing score and the higher achievements
    private float UpdatePassingScore()
    {
        float currentPassingScore = GameBalanceFormulae.increaseOneStarScore_formula(level);

        twoStarScore_current = currentPassingScore * 2;
        threeStarScore_current = currentPassingScore * 5;

        return currentPassingScore;
    }

}

#region unchanged class
//All stats related to how much patience the customer has
public class CustomerPatienceStats
{
    private static float _customerPatience_General = GameBalanceFormulae.customerPatience_base_General;
    private static float _customerPatience_Queue = GameBalanceFormulae.customerPatience_base_Queue;
    private static float _customerPatience_TakeOrder = GameBalanceFormulae.customerPatience_base_TakeOrder;
    private static float _customerPatience_FoodWait = GameBalanceFormulae.customerPatience_base_FoodWait;

    public static float CustomerPatience_General
    {
        get { return _customerPatience_General; }
        private set { _customerPatience_General = value;  }
    }

    public static float CustomerPatience_Queue
    {
        get { return _customerPatience_Queue; }
        private set { _customerPatience_Queue = value; }
    }

    public static float CustomerPatience_TakeOrder
    {
        get { return _customerPatience_TakeOrder; }
        private set { _customerPatience_TakeOrder = value; }
    }

    public static float CustomerPatience_FoodWait
    {
        get { return _customerPatience_FoodWait; }
        private set { _customerPatience_FoodWait = value; }
    }

    public static float customerEatingDuration = 5f;
    public static float drinkPatienceIncrease = 5f;
    public static float angryPatienceDecrease = 5f;

    public static float customerQueuePatienceLimit_80;
    public static float customerQueuePatienceLimit_90;
    public static float customerQueuePatienceLimit_100;
    public static float customerQueuePatienceLimit_110;
    public static float customerQueuePatienceLimit_120;


    public static void UpdateStats()
    {
        CustomerPatience_General = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_General, LevelStats.Instance.level);
        CustomerPatience_Queue = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_Queue, LevelStats.Instance.level);
        CustomerPatience_TakeOrder = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_TakeOrder, LevelStats.Instance.level);
        //customerPatience_FoodWait = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_FoodWait, LevelStats.Level);
    }

    public static void CheckPatienceLimit()
    {
        customerQueuePatienceLimit_80 = _customerPatience_Queue * 80 / 100;
        customerQueuePatienceLimit_90 = _customerPatience_Queue * 90 / 100;
        customerQueuePatienceLimit_100 = _customerPatience_Queue;
        customerQueuePatienceLimit_110 = _customerPatience_Queue * 110 / 100;
        customerQueuePatienceLimit_120 = _customerPatience_Queue * 120 / 100;
    }

    public static void UpdatePatienceFromMood(float percentage)
    {
        //CheckPatienceLimit();

        //switch (percentage)
        //{
        //    case 80:
        //        if (CustomerPatience_Queue == customerQueuePatienceLimit_80)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            CustomerPatience_Queue = CustomerPatience_Queue * (percentage / 100);
        //        }
        //        Debug.Log("Updated queue patience: " + CustomerPatience_Queue);
        //        break;

        //    case 90:
        //        if (CustomerPatience_Queue == customerQueuePatienceLimit_80)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            CustomerPatience_Queue = CustomerPatience_Queue * (percentage / 100);
        //        }
        //        Debug.Log("Updated queue patience: " + CustomerPatience_Queue);
        //        break;

        //    case 100:
        //        if (CustomerPatience_Queue == _customerPatience_Queue)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            CustomerPatience_Queue = CustomerPatience_Queue * (percentage / 100);
        //        }
        //        Debug.Log("Updated queue patience: " + CustomerPatience_Queue);
        //        break;

        //    case 110:
        //        if (CustomerPatience_Queue == customerQueuePatienceLimit_80)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            CustomerPatience_Queue = CustomerPatience_Queue * (percentage / 100);
        //        }
        //        Debug.Log("Updated queue patience: " + CustomerPatience_Queue);
        //        break;

        //    case 120:
        //        if (CustomerPatience_Queue == customerQueuePatienceLimit_80)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            CustomerPatience_Queue = CustomerPatience_Queue * (percentage / 100);
        //        }
        //        Debug.Log("Updated queue patience: " + CustomerPatience_Queue);
        //        break;
        //}

        //float normal_patience_general = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_General, LevelStats.Level);
        //float normal_patience_queue = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_Queue, LevelStats.Level);
        //float normal_patience_order = GameBalanceFormulae.customerPatience_formula_General(GameBalanceFormulae.customerPatience_base_TakeOrder, LevelStats.Level);

        //if (CustomerPatience_General == (percentage / 100) * normal_patience_general 
        //    && CustomerPatience_Queue == (percentage / 100) * normal_patience_queue
        //    && CustomerPatience_TakeOrder == (percentage / 100) * normal_patience_order)
        //{
        //    return;
        //}
        //else
        //{
        //    Debug.Log("Updated general patience: " + customerPatience_General);
        //    Debug.Log("Updated queue patience: " + customerPatience_Queue);
        //    Debug.Log("Updated order patience: " + customerPatience_TakeOrder);
        //    Debug.Log("Updated wait patience: " + customerPatience_FoodWait);

        //    customerPatience_General = customerPatience_General * (percentage / 100);
        //    customerPatience_Queue = customerPatience_Queue * (percentage / 100);
        //    customerPatience_TakeOrder = customerPatience_TakeOrder * (percentage / 100);
        //}


        //customerPatience_FoodWait = customerPatience_FoodWait * (percentage / 100);
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

    private static float oneStarScore_base = 150f; //amount of points needed to earn one star (the passing score)
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
        //float newOneStarScore = oneStarScore_base * levelNum;

        float newOneStarScore = oneStarScore_base * levelNum - ((levelNum - 1) / 2);

        if (newOneStarScore <= OneStarScore_max)
        {
            return Mathf.RoundToInt(newOneStarScore);
        }
        else
        {
            return OneStarScore_max;
        }
    }

}
