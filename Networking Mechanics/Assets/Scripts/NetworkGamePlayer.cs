using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

#region Summary

//Handles player in the game map, outside of the lobby
//Game players != room players
//Room players are temporary connections used only in the lobby

#endregion

public class NetworkGamePlayer : NetworkBehaviour
{
    //Player in the game, outside of the lobby

    public static CustomNetworkManager networkRoomManager; //network manager object

    //property for custom network manager
    private CustomNetworkManager NetworkRoomManager
    {

        get
        {
            if (networkRoomManager != null) //if there is a network manager object, return that
            {
                return networkRoomManager; //If not null, then just return the current room
            }

            //if its null, then just get it
            return networkRoomManager = NetworkManager.singleton as CustomNetworkManager; //Network manager from mirror has a singleton variable
            //but we want it as our custom network manager, so we have to do a cast
        }
    }


    //called on every network behaviour when its active on a client
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject); //gameobject not destroyed when going between different scenes ie. different rooms, levels

        NetworkRoomManager.GamePlayers.Add(this); //add this instance to our list of game players

    }

    //Invoked on clients from servers, so even if someone else that is not us disconnects
    //we will still recognise and remove them from the list 
    public override void OnNetworkDestroy()
    {
        NetworkRoomManager.GamePlayers.Remove(this); //Remove the player from our list of total game players
    }
}
