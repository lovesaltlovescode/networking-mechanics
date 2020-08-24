using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public class VR_OrderManagement : MonoBehaviour
{
    #region Singleton
    private static VR_OrderManagement _instance;
    public static VR_OrderManagement Instance { get { return _instance; } }
    private void Awake()
    {
        
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

    private List<ChickenRice> newOrders = new List<ChickenRice>();
    public List<ChickenRice> NewOrders
    {
        get { return newOrders; }
    }


    public void AddOrderToList(bool roastedChic, bool ricePlain, bool haveEgg)
    {
        ChickenRice tempOrderHolder = OrderGeneration.Instance.CreateCustomOrder(roastedChic, ricePlain, haveEgg);

        newOrders.Add(tempOrderHolder);
    }


    public void RemoveOrder(ChickenRice _chickenRiceOrder)
    {
        newOrders.Remove(_chickenRiceOrder);

    }

    public void ClearList()
    {
        newOrders.Clear();
    }

}
