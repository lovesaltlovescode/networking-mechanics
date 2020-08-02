﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton game manager to contain all server-controlled variables
/// positions and counts of ingredients and plates
/// </summary>
public class GameManager : MonoBehaviour
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
    public float timeSinceLastSpawn = 0f, currentNumWaitingCustomers = 0f;


    [Header("Ingredient Tray")]
    public Transform[] trayPositions; //where ingredients should be placed
    public GameObject[] ingredientsOnTray = new GameObject[4];
    public int ingredientsOnTrayCount; //number of ingredients on the tray

    [Header("Sink Positions")]
    public Transform[] sinkPositions; //where dirty plates should be placed in the sink

    public GameObject[] platesInSink = new GameObject[4]; 

    [Header("Plate counts")]
    public int platesInSinkCount; //number of plates in the sink

    [Header("Drinks")]
    public Transform[] drinkPositions; //where the drink should be spawned
    public GameObject[] drinksOnCounter = new GameObject[2]; //number of drinks on the counter
    public int drinksCount; //number of drinks on the counter

    [Header("DrinksCooldown")]
    public Image cooldownImg;
    public float cooldown = 5f;
    public bool isCooldown = false;

    [Header("Dishes")]
    public Transform[] dishPositions;
    public GameObject[] dishesOnCounter = new GameObject[3];
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

}
