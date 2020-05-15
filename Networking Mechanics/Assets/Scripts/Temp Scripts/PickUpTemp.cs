using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains all pickupable objects, that pop up in inventory when collided with
public class PickUpTemp : MonoBehaviour
{

    private InventoryTemp inventoryTemp;

    //item button object
    public GameObject itemIcon;

    // Start is called before the first frame update
    void Start()
    {
        //get inventory component
        inventoryTemp = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryTemp>();

    }

    //if item collides with a player
    //player will pick up if inventory is not full
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for(int i = 0; i < inventoryTemp.slots.Length; i++)
            {
                if(inventoryTemp.isFull[i] == false)
                {
                    //if not full, item can be added
                    //so slot becomes full
                    inventoryTemp.isFull[i] = true;
                    //Instantiate the button as a child of the inventory slot of index  i
                    //and do not spawn in world coordinates because UI
                    Instantiate(itemIcon, inventoryTemp.slots[i].transform, false);
                    break;
                }
            }
        }
    }
}
