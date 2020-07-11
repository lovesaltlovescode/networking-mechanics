using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handle the inventory slots
public class PlayerInventory : MonoBehaviour
{

    public List<bool> isFull = new List<bool>(); //check if there is already an item inside the slot(s)
    public GameObject[] slots; //assign the item we pick up to the inventory slot



}
