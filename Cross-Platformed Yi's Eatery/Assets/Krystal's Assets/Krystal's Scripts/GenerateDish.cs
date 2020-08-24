using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDish : MonoBehaviour
{
    #region Singleton

    private static GenerateDish _instance;
    public static GenerateDish Instance { get { return _instance; } }

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

    [SerializeField] private GameObject roastedPlain, roastedPlain_egg, roastedBall, roastedBall_egg;
    [SerializeField] private GameObject steamedPlain, steamedPlain_egg, steamedBall, steamedBall_egg;

    [SerializeField] private Transform[] dishSpawnPoints = new Transform[3];
    private GameObject[] dishesSpawned = new GameObject[3];

    //Instantiates the order being served
    public void SpawnDish(ChickenRice dishDetails)
    {
        //returns -1 if the counter is full
        int indexNum = CheckCounterSpace();

        if(indexNum > -1)
        {
            //instantiate a new dish as a child of an empty spot on the counter
            Instantiate(IdentifyOrder(dishDetails.ChickenRiceLabel), dishSpawnPoints[indexNum]);
        }
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


    //checks if there is empty space on the counter to spawn a dish and returns the index num of the empty space. 
    //If the counter is full, return -1. 
    private int CheckCounterSpace()
    {
        for(int i = 0; i < dishSpawnPoints.Length; i++)
        {
            if (dishSpawnPoints[i].childCount == 0)
            {
                return i;
            }
        }

        return -1;
    }

}
