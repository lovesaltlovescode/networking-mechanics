using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_CentralisedOrderContainer : MonoBehaviour
{
    #region Singleton

    private static Temp_CentralisedOrderContainer _instance;
    public static Temp_CentralisedOrderContainer Instance { get { return _instance; } }

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

    public static List<ChickenRice> OrderIconList = new List<ChickenRice>();

    public static void AddOrderToList(ChickenRice _order)
    {
        OrderIconList.Add(_order);
        
        Temp_VRSpawnOrder.Instance.SpawnOrderIcon(_order.OrderIcon);
    }


}
