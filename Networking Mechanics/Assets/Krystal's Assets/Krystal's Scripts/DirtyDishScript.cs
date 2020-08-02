using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyDishScript : MonoBehaviour
{
    [SerializeField] private TableScript tableSeatedAt;

    //public method to set the table the dirty dish belongs to
    public void SetTableScript(TableScript _tableScript)
    {
        tableSeatedAt = _tableScript;

        AddToTable();
    }

    public void Update()
    {
        Debug.Log("Checking DirtyDishScript - is table script null? " + tableSeatedAt == null);

        if(tableSeatedAt != null)
        {
            Debug.Log("Checking DirtyDishScript - dish prefab is on table: " + tableSeatedAt.gameObject.name);
        }
    }

    //method to add this dirty dish to the list of dirty dishes on the table
    public void AddToTable()
    {
        if (tableSeatedAt != null)
        {
            tableSeatedAt.dirtyDishes.Add(this.gameObject);

            Debug.Log("Checking DirtyDishScript - is table script null? " + tableSeatedAt == null);

            if (tableSeatedAt != null)
            {
                Debug.Log("Checking DirtyDishScript - dish prefab is on table: " + tableSeatedAt.gameObject.name);
            }
        }
        else
        {
            Debug.Log("Table script wasn't assigned to dirty dish");
        }

    }

    //method to remove this dirty dish from the list of dirty dishes on the table
    public void RemoveFromTable() //--------- call this method when the player picks the dirty dish up
    {
        if (tableSeatedAt != null)
        {
            tableSeatedAt.dirtyDishes.Remove(this.gameObject);
            
            //----------------- You can call the function to move the plate to the player's head here.
        }
        else
        {
            Debug.Log("Table script wasn't assigned to dirty dish");

            Debug.Log("Checking DirtyDishScript - is table script null? " + tableSeatedAt == null);
        }
    }
}
