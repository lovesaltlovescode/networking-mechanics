using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Script attached to scene object prefab
/// since the item held by player cannot have a netid and cannot be dropped into the world
/// item dropped from player will appear as a child of this empty prefab 
/// 
/// </summary>

public class SceneObject : NetworkBehaviour
{
    ////sync held item and call onchangeitem method
    //[SyncVar(hook = nameof(OnChangeItem))]
    //public HeldItems heldItem;

    ////PREFABS to be dropped
    //public GameObject cucumberPrefab;
    //public GameObject eggPrefab;
    //public GameObject chickenPrefab;

    ////method that starts the coroutine
    //void OnChangeItem(HeldItems oldHeldItem, HeldItems newHeldItem)
    //{
    //    StartCoroutine(ChangeItem(newHeldItem));
    //}

    ////destroy is delayed to end of current frame, so we use a coroutine
    ////clear any child object before instantiating the new one
    //IEnumerator ChangeItem(HeldItems newHeldItem)
    //{
    //    while (transform.childCount > 0)
    //    {
    //        Destroy(transform.GetChild(0).gameObject);
    //        yield return null;
    //    }

    //    //use the new value
    //    SetHeldItem(newHeldItem);
    //}


    ////SetHeldItem is called on client from onchangeitem method
    ////and on the server from cmddropitem in playerholditem script
    //public void SetHeldItem(HeldItems newHeldItem)
    //{
    //    switch (newHeldItem)
    //    {
    //        case HeldItems.cucumber:
    //            Instantiate(cucumberPrefab, transform);
    //            break;
    //        case HeldItems.egg:
    //            Instantiate(eggPrefab, transform);
    //            break;
    //        case HeldItems.chicken:
    //            Instantiate(chickenPrefab, transform);
    //            break;
    //    }
    //}

    ////call cmdpickupitem
    //private void OnMouseDown()
    //{
    //    //network client connecting to the server ie.host
    //    //get its net id and get the playerholditem script

    //    //pass the sceneobject prefab (this gameobject) directly through to the cmdpickupitem 
    //    //as it is networked
    //    Debug.Log("SceneObject - " + NetworkClient.connection.identity);

    //    //TODO: FIGURE OUT HOW TO DO THIS FOR EACH PLAYER
    //    //probably use some boolean to check if player dropped anything?
    //    PlayerMovement.ActivePlayers[1].GetComponent<PlayerHoldItem>().CmdPickUpItem(gameObject);
    //}
}
