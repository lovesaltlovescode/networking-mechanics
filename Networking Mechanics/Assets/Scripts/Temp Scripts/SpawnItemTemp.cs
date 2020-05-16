using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to item icon 
//Spawns item at player position when dropped
public class SpawnItemTemp : MonoBehaviour
{
    public GameObject droppedItem;

    public void SpawnDroppedItem()
    {
        //Spawn dropped item
        Debug.Log("Dropped an item");
        Instantiate(droppedItem, PlayerMovementTest.playerPos, Quaternion.identity);
        Debug.Log("Instantiated at" + PlayerMovementTest.playerPos);
    }
}
