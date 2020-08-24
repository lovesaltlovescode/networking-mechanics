using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderGeneration_NotNetworked : MonoBehaviour
{
    #region Singleton

    private static OrderGeneration_NotNetworked _instance;
    public static OrderGeneration_NotNetworked Instance { get { return _instance; } }

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

    #region Debug Shortcuts
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateNewOrder();
        }
    }
    */
    #endregion
    [SerializeField] private GameObject roastedPlain, roastedPlain_egg, roastedBall, roastedBall_egg;
    [SerializeField] private GameObject steamedPlain, steamedPlain_egg, steamedBall, steamedBall_egg;

    //method to randomly generate an order
    public ChickenRice CreateNewOrder()
    {
        ChickenRice newOrder = new ChickenRice(Random.value > 0.5f, Random.value > 0.5f, Random.value > 0.5f);
        newOrder.ChickenRiceLabel = newOrder.IdentifyChickenRice();
        newOrder.OrderIcon = this.IdentifyIcon(newOrder.ChickenRiceLabel);
        //Debug.Log("Order generated. Does customer want chicken roasted? " + newOrder.RoastedChic + ". Rice plain? " + newOrder.RicePlain + ". Include egg? " + newOrder.HaveEgg + ". Include cucumber? " + newOrder.Cucumber + ". Have label? " + newOrder.ChickenRiceLabel + ". Have icon? " + (newOrder.OrderIcon != null));

        return newOrder;
    }

    public ChickenRice CreateCustomOrder(bool roastedChic, bool ricePlain, bool haveEgg)
    {
        ChickenRice newOrder = new ChickenRice(roastedChic, ricePlain, haveEgg);
        newOrder.ChickenRiceLabel = newOrder.IdentifyChickenRice();
        newOrder.OrderIcon = this.IdentifyIcon(newOrder.ChickenRiceLabel);
        //Debug.Log("Order generated. Does customer want chicken roasted? " + newOrder.RoastedChic + ". Rice plain? " + newOrder.RicePlain + ". Include egg? " + newOrder.HaveEgg + ". Include cucumber? " + newOrder.Cucumber + ". Have label? " + newOrder.ChickenRiceLabel + ". Have icon? " + (newOrder.OrderIcon != null));

        return newOrder;
    }

    //Identifies which food order icon should be displayed
    public GameObject IdentifyIcon(ChickenRice.PossibleChickenRiceLabel chickenRiceLabel)
    {
        switch (chickenRiceLabel)
        {
            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRice:
                return roastedPlain;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRiceEgg:
                return roastedPlain_egg;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBall:
                return roastedBall;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBallEgg:
                return roastedBall_egg;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRice:
                return steamedPlain;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRiceEgg:
                return steamedPlain_egg;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBall:
                return steamedBall;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBallEgg:
                return steamedBall_egg;

            default:
                return null;
        }
    }
}
