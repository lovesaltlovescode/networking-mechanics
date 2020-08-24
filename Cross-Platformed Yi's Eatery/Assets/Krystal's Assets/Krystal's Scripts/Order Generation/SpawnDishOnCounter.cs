using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnDishOnCounter : NetworkBehaviour
{
    #region Singleton

    private static SpawnDishOnCounter _instance;
    public static SpawnDishOnCounter Instance { get { return _instance; } }

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

    [SerializeField] private GameObject objectContainerPrefab;


    ////Instantiates the order being served

    //[ServerCallback]
    //public void SpawnDish(int _indexNum, bool isRoasted, bool ricePlain, bool haveEgg)
    //{
    //    ChickenRice dishToBeSpawned = OrderGeneration.Instance.CreateCustomOrder(isRoasted, ricePlain, haveEgg);

    //    //instantiate a new dish on empty spot on the counter
    //    GameObject newDish = Instantiate(IdentifyOrder(dishToBeSpawned.ChickenRiceLabel), GameManager.Instance.dishSpawnPoints[_indexNum].position, GameManager.Instance.dishSpawnPoints[_indexNum].rotation);

    //    NetworkServer.Spawn(newDish); //spawn on the network for clients to see

    //    RpcSpawnDish(_indexNum, newDish);
    //}

    [ServerCallback]
    public void ServerSpawnDish(int _indexNum, bool isRoasted, bool ricePlain, bool haveEgg)
    {
        ChickenRice dishToBeSpawned = OrderGeneration.Instance.CreateCustomOrder(isRoasted, ricePlain, haveEgg);

        GameObject newDish = Instantiate(objectContainerPrefab, GameManager.Instance.dishSpawnPoints[_indexNum].position, GameManager.Instance.dishSpawnPoints[_indexNum].rotation);

        newDish.GetComponent<Rigidbody>().isKinematic = false;

        ObjectContainer dishContainer = newDish.GetComponent<ObjectContainer>();

        //Instantiate the right held item
        dishContainer.SetObjToSpawn(IdentifyOrder(dishToBeSpawned.ChickenRiceLabel));

        //Sync var
        dishContainer.objToSpawn = IdentifyOrder(dishToBeSpawned.ChickenRiceLabel);

        //Spawn on network
        NetworkServer.Spawn(newDish);

        RpcSpawnDish(_indexNum, newDish);

    }


    [ClientRpc]
    public void RpcSpawnDish(int _indexNum, GameObject newDish)
    {
        //spawn as a dish item
        newDish.layer = LayerMask.NameToLayer("Dish");

        //add the dish to the dish on counter array
        GameManager.Instance.dishesOnCounter[_indexNum] = newDish;
    }

    //Identifies which dish should be instantiated
    private HeldItem IdentifyOrder (ChickenRice.PossibleChickenRiceLabel chickenRiceLabel)
    {
        switch (chickenRiceLabel)
        {
            #region Roasted Chicken
            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRice:
                return HeldItem.roastedChicWPlainRice;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRiceEgg:
                return HeldItem.roastedChicWPlainRiceEgg;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBall:
                return HeldItem.roastedChicWRiceBall;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBallEgg:
                return HeldItem.roastedChicWRiceBallEgg;

            #endregion

            #region Steamed Chicken
            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRice:
                return HeldItem.steamedChicWPlainRice;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRiceEgg:
                return HeldItem.steamedChicWPlainRiceEgg;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBall:
                return HeldItem.steamedChicWRiceBall;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBallEgg:
                return HeldItem.steamedChicWRiceBallEgg;

            #endregion

            default:
                Debug.Log("The dish does not have a label.");
                return HeldItem.nothing;
        }

    }


    //checks if there is empty space on the counter to spawn a dish. 
    //If the counter is full, return false. 
    public int CheckCounterHasSpace()
    {
        int i = 0;

        foreach (GameObject dish in GameManager.Instance.dishesOnCounter)
        {
            if (dish == null)
            {
                return i;
            }

            i++;
        }

        return -1;
    }
}
