using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

//Attached to player object
//synchronize hp and mp ints across network

public class RandomGenerate : NetworkBehaviour
{

    public TextMeshProUGUI hpMeter;
    //public TextMeshProUGUI mpMeter;

    [SyncVar(hook = nameof(RandomNumberSyncCallback))]
    public int hpNumber;

    //[SyncVar]
    //public int mpNumber;


    public override void OnStartAuthority()
    {
        hpMeter.gameObject.SetActive(true);
        //mpMeter.gameObject.SetActive(true);
    }

    public void Update()
    {

        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                GenerateRandom();
            }
            
        }


            hpMeter.text = "My hp is: " + hpNumber;
            //mpMeter.text = "My mp is " + mpNumber;

            Debug.Log("Hp is..." + hpNumber);
            //Debug.Log("Mp is..." + mpNumber);

    }

    public void GenerateRandom()
    {
        hpNumber = Random.Range(0, 100);
        //mpNumber = Random.Range(0, 50);
    }

    void RandomNumberSyncCallback(int oldValue, int newValue)
    {
        if (isServer) return;
        Debug.Log("I'm a client, got a new number!" + newValue);
        hpNumber = newValue;
        hpMeter.text = hpNumber.ToString();
    }


}
