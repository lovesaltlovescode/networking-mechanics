using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DirtyDishScript : NetworkBehaviour
{
    [SerializeField] private TableScript tableSeatedAt;
    [SerializeField] private GameObject table;

    //public method to set the table the dirty dish belongs to
    public void SetTableScript(TableScript _tableScript)
    {
        tableSeatedAt = _tableScript;

        AddToTable();
    }

    public void Update()
    {
        //Debug.Log("Checking DirtyDishScript - is table script null? " + tableSeatedAt == null);

        if(tableSeatedAt != null)
        {
            //Debug.Log("Checking DirtyDishScript - dish prefab is on table: " + tableSeatedAt.gameObject.name);
        }
    }

    //method to add this dirty dish to the list of dirty dishes on the table
    [ServerCallback]
    public void AddToTable()
    {
        RpcAddToTable();
    }

    [ClientRpc]
    public void RpcAddToTable()
    {
        if (tableSeatedAt != null)
        {
            tableSeatedAt.dirtyDishes.Add(this.gameObject);

            //Debug.Log("Checking DirtyDishScript - is table script null? " + tableSeatedAt == null);

            if (tableSeatedAt != null)
            {
                //Debug.Log("Checking DirtyDishScript - dish prefab is on table: " + tableSeatedAt.gameObject.name);
            }
        }
        else
        {
            //Debug.Log("Table script wasn't assigned to dirty dish");
        }
    }

    
    //method to remove this dirty dish from the list of dirty dishes on the table
    public void RemoveFromTable() //--------- call this method when the player picks the dirty dish up
    {
        //RpcRemoveFromTable();
        if (tableSeatedAt != null)
        {
            tableSeatedAt.dirtyDishes.Remove(this.gameObject);

            //----------------- You can call the function to move the plate to the player's head here.
        }
        else
        {
            //Debug.Log("Table script wasn't assigned to dirty dish");

            //Debug.Log("Checking DirtyDishScript - is table script null? " + tableSeatedAt == null);
        }
    }

    public void TooManyPlates()
    {
        if(tableSeatedAt != null)
        {
            table = tableSeatedAt.gameObject;
            //Debug.Log("Table name " + table.name);
            table.GetComponent<TableFeedback>().SinkFull();
        }
    }

    [ClientRpc]
    //Not working, throws 'unspawned object' error
    public void RpcRemoveFromTable()
    {
        if (tableSeatedAt != null)
        {
            tableSeatedAt.dirtyDishes.Remove(this.gameObject);

            //----------------- You can call the function to move the plate to the player's head here.
        }
        else
        {
            //Debug.Log("Table script wasn't assigned to dirty dish");

            //Debug.Log("Checking DirtyDishScript - is table script null? " + tableSeatedAt == null);
        }
    }
}
