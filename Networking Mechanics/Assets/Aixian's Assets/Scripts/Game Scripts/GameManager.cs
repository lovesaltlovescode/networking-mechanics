using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// Singleton game manager to contain all server-controlled variables
/// positions and counts of ingredients and plates
/// </summary>
public class GameManager : NetworkBehaviour
{

    #region Singleton

    private static GameManager _instance;

    //property
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion

    #region Variables

    [Header("Customers")]

    [SyncVar]
    public float timeSinceLastSpawn = 0f;

    [SyncVar]
    public float currentNumWaitingCustomers = 0f;


    [Header("Ingredient Tray")]
    public Transform[] trayPositions; //where ingredients should be placed
    public GameObject[] ingredientsOnTray = new GameObject[4];
    public int ingredientsOnTrayCount; //number of ingredients on the tray

    [Header("Sink Positions")]
    public Transform[] sinkPositions; //where dirty plates should be placed in the sink

    public GameObject[] platesInSink = new GameObject[4]; 

    [Header("Plate counts")]
    public int platesInSinkCount; //number of plates in the sink
    public int cleanPlatesCount;
    public Transform[] cleanPlateSpawnPositions;
    public GameObject[] cleanPlatesOnTable;

    [Header("Drinks")]
    public Transform[] drinkPositions; //where the drink should be spawned
    public GameObject[] drinksOnCounter = new GameObject[2]; //number of drinks on the counter
    public int drinksCount; //number of drinks on the counter

    [Header("DrinksCooldown")]

    public Image cooldownImg;
    [SyncVar]
    public float cooldown = 5f;
    [SyncVar]
    public bool isCooldown = false;
    [SyncVar]
    public bool isDrinkCoroutineRunning = false;


    [Header("Dishes")]
    public Transform[] dishSpawnPoints;
    public GameObject[] dishesOnCounter = new GameObject[3] { null, null, null };
    //public int dishCount; //number of dishes on the counter


    #endregion

    private void Awake()
    {
        //if there is already an instance of this object in the scene, destroy this
        if(_instance && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        LevelTimer.Instance.StartTimer();
    }
}
