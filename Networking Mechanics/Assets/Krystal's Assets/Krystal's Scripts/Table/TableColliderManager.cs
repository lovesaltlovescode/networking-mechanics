using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableColliderManager : MonoBehaviour
{
    #region Debug shortcut
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleTableDetection(true);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTableDetection(false);
        }
    }
    */
    #endregion


    private static List<GameObject> allTableColliders = new List<GameObject>();
    private static string tableLayer = "Table", environmentLayer = "Environment";


    //add current table to the list of tables in TableColliderManager script
    public static void AddTableToTableColliderManager(GameObject table)
    {
        allTableColliders.Add(table);
    }


    //switch the layers of all the tables 
    public static void ToggleTableDetection(bool allowDetection)
    {
        if (allowDetection)
        {
            foreach (GameObject table in allTableColliders)
            {
                //if the table is in the env layer, add it to the table layer
                if(table.gameObject.layer == LayerMask.NameToLayer(environmentLayer))
                {
                    table.gameObject.layer = LayerMask.NameToLayer(tableLayer);
                }                
            }
        } 
        else
        {
            foreach (GameObject table in allTableColliders)
            {
                //if the table is in the table layer, add it to the env layer
                if (table.gameObject.layer == LayerMask.NameToLayer(tableLayer))
                {
                    table.gameObject.layer = LayerMask.NameToLayer(environmentLayer);
                }
            }
        }
        
    }

    //switch the layers of a single table
    public static void ToggleTableDetection(bool allowDetection, GameObject table, string _layerName = "Table")
    {
        if (allowDetection)
        {
            table.gameObject.layer = LayerMask.NameToLayer(_layerName);
        }
        else
        {
            table.gameObject.layer = LayerMask.NameToLayer(environmentLayer);
        }

    }


    
}
