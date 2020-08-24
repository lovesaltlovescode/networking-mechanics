using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDishOnCounter_NotNetworked : MonoBehaviour
{
    #region Singleton

    private static SpawnDishOnCounter_NotNetworked _instance;
    public static SpawnDishOnCounter_NotNetworked Instance { get { return _instance; } }

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

    //variables
    //dish prefabs
    [SerializeField] private GameObject roastedPlain, roastedPlain_egg, roastedBall, roastedBall_egg;
    [SerializeField] private GameObject steamedPlain, steamedPlain_egg, steamedBall, steamedBall_egg;

    //spawn points
    [SerializeField] private Transform[] dishSpawnPoints = new Transform[3];
    private GameObject[] dishOnCounter = new GameObject[3] { null, null, null };

    //Instantiates the order being served
    public void SpawnDish(int _indexNum, bool isRoasted, bool ricePlain, bool haveEgg)
    {
        ChickenRice dishToBeSpawned = OrderGeneration_NotNetworked.Instance.CreateCustomOrder(isRoasted, ricePlain, haveEgg);

        //instantiate a new dish on empty spot on the counter
        GameObject newDish = Instantiate(IdentifyOrder(dishToBeSpawned.ChickenRiceLabel), dishSpawnPoints[_indexNum].position, dishSpawnPoints[_indexNum].rotation);

        //add the dish to the dish on counter array
        dishOnCounter[_indexNum] = newDish;
    }

    //Identifies which dish should be instantiated
    private GameObject IdentifyOrder(ChickenRice.PossibleChickenRiceLabel chickenRiceLabel)
    {
        switch (chickenRiceLabel)
        {
            #region Roasted Chicken
            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRice:
                return roastedPlain;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRiceEgg:
                return roastedPlain_egg;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBall:
                return roastedBall;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBallEgg:
                return roastedBall_egg;
            #endregion

            #region Steamed Chicken
            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRice:
                return steamedPlain;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRiceEgg:
                return steamedPlain_egg;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBall:
                return steamedBall;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBallEgg:
                return steamedBall_egg;
            #endregion

            default:
                Debug.Log("The dish does not have a label.");
                return null;
        }
        
    }


    //checks if there is empty space on the counter to spawn a dish. 
    //If the counter is full, return false. 
    public int CheckCounterHasSpace()
    {
        int i = 0;

        foreach(GameObject dish in dishOnCounter)
        {
            if(dish == null)
            {
                return i;
            }

            i++;
        }

        return -1;
    }
}
