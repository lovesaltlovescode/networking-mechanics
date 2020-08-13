using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_VRSpawnOrder : MonoBehaviour
{
    #region Singleton

    private static Temp_VRSpawnOrder _instance;
    public static Temp_VRSpawnOrder Instance { get { return _instance; } }

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

    [SerializeField] Transform spawnPoint;
    public List<ChickenRice> OrderIconList = new List<ChickenRice>();

    public void AddOrderToList(bool roastedChic, bool ricePlain, bool haveEgg)
    {
        ChickenRice tempHolder = OrderGeneration.Instance.CreateCustomOrder(roastedChic, ricePlain, haveEgg);

        OrderIconList.Add(tempHolder);
        SpawnOrderIcon(tempHolder.OrderIcon);
    }

    public void SpawnOrderIcon(GameObject _chickenRiceIcon)
    {
        Instantiate(_chickenRiceIcon, spawnPoint.position, spawnPoint.rotation);
    }
}