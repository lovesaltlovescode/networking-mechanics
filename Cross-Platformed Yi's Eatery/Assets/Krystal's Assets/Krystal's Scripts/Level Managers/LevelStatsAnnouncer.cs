﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStatsAnnouncer : MonoBehaviour
{
    #region Singleton

    private static LevelStatsAnnouncer _instance;
    public static LevelStatsAnnouncer Instance { get { return _instance; } }

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


    // Start is called before the first frame update
    void Start()
    {
        CustomerPatienceStats.UpdateStats();
    }

    public void MoveToNextLevel()
    {
        LevelStats.Instance.UpdateLevel();
        CustomerPatienceStats.UpdateStats();
    }

    #region Debug Stuff
    /*
    Debug.Log("current level: " + LevelStats.Level);
    Debug.Log("customerPatience_base_General: " + CustomerPatienceStats.customerPatience_General);
    Debug.Log("customerPatience_base_Queue: " + CustomerPatienceStats.customerPatience_Queue);
    Debug.Log("customerPatience_base_TakeOrder: " + CustomerPatienceStats.customerPatience_TakeOrder);
    Debug.Log("customerPatience_base_FoodWait: " + CustomerPatienceStats.customerPatience_FoodWait);
    */

    /*
    public int sceneNum = 1;
     
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MoveToNextLevel();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneNum);
    }
    */
    #endregion


}
