using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;


//Synchronize player name information so they appear the same on all clients
//Using syncvar and static and procedure calls
public class SyncPlayerInfo : NetworkBehaviour
{


    //List of tag names to be given 
    public List<string> tagNames = new List<string> { "XiaoBen", "DaFan", "DaLi", "XiaoFan", "XiaoLi" };

    //[SyncVar]
    //increment this number
    public static int increment = 0;

    [SyncVar]
    //string to contain the current tag
    //synchronized across the network, when this value is changed, all clients will get the updated value
    public string myTag;

    //name displayed on top of character
    public TextMeshProUGUI nameDisplay;


    //On start local player, called on the client's local player
    //when a new client spawns, run some initialization for player
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //initialization for player when first joining the game

        //call function to increment the number everytime a new client spawns
        //IncreaseNo();
        Debug.Log("Started local player!"); //only called once
        //Debug.Log(increment); //since this is run on the client's local player, it will only show up once in the editor


    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        Debug.Log("Started authority!"); //called once on every player when they start
        IncreaseNo();
    }

    private void IncreaseNo()
    {
        //call the command function to increase increment
        CmdIncreaseNo();
    }

    [Command]
    //command to increase the increment on the server
    //sent from client to server, run on the SERVER
    private void CmdIncreaseNo()
    {
        //assign the current tag increment as a string
        string name = tagNames[increment];

        //increment the number
        increment += 1;
        Debug.Log(increment);

        //pass in the name and set it on local player, as well as all clients
        //ie. xiaoben will passed to all clients 
        SetTagName(name);
        RpcSetTagName(name);
    }

    private void SetTagName(string tagN)
    {
        myTag = tagN; //the myTag variable has changed, and will be synced across the network
    }

    [ClientRpc]
    private void RpcSetTagName(string tagN)
    {
        myTag = tagN;
    }

    void Update()
    {
        //update player tags every frame so every client knows who each other is
        CmdUpdateTags();
        Debug.Log("Player tag is " + gameObject.tag);
    }

    [Command]
    private void CmdUpdateTags()
    {
        //update tag name locally
        UpdateTagName();

        //update tag name on all clients
        RpcUpdateTagName();
    }

    private void UpdateTagName()
    {
        nameDisplay.text = myTag;
        gameObject.tag = myTag; //change tag name to be the current tag ie. xiaoben
    }

    [ClientRpc]
    private void RpcUpdateTagName()
    {
        nameDisplay.text = myTag;
        gameObject.tag = myTag;
    }


    //coroutine to call cmdrandomnumbers every 5 s
    //private IEnumerator randomNumbers()
    //{
    //    WaitForSeconds wait = new WaitForSeconds(2f);
    //    while (true)
    //    {
    //        // CmdRandomNumbers();
    //        //health = (int)Mathf.Floor(Random.Range(0.0f, 100.0f));
    //        yield return wait;
    //    }


    //}

}

