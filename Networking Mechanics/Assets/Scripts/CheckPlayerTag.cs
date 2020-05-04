﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Summary

//Attached to player object in game scene
//Player will have their own tag based on the order they were spawned in
//Players will enable the models according to their tag
//Check for tag name 
//if it matches, then display that model

#endregion
public class CheckPlayerTag : MonoBehaviour
{

    //array of possible models to display
    //assign in the inspector
    public GameObject[] characterModels = new GameObject[5];




    // Start is called before the first frame update
    void Start()
    {
        CheckTag();

    }

    void CheckTag()
    {
        switch(gameObject.tag)
        {
            case "XiaoBen":
                Debug.Log("This player is Xiao Ben!");
                characterModels[0].SetActive(true);
                break;

            case "DaFan":
                Debug.Log("This player is Da Fan!");
                characterModels[1].SetActive(true);
                break;

            case "DaLi":
                Debug.Log("This player is Da Li!");
                characterModels[2].SetActive(true);
                break;

            case "XiaoFan":
                Debug.Log("This player is Xiao Dan!");
                characterModels[3].SetActive(true);
                break;

            case "XiaoLi":
                Debug.Log("This player is Xiao Li!");
                characterModels[4].SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
