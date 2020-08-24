using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableColliderManager : MonoBehaviour
{

    #region Singleton

    private static TableColliderManager _instance;
    public static TableColliderManager Instance { get { return _instance; } }

    private void Awake()
    {
        Debug.Log(this.gameObject.name);

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

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


    [SerializeField] private List<GameObject> allTableColliders = new List<GameObject>();
    private string tableLayer = "Table", environmentLayer = "Environment";


    //add current table to the list of tables in TableColliderManager script
    public void AddTableToTableColliderManager(GameObject table)
    {
        allTableColliders.Add(table);
        Debug.Log("Table? " + table.name);
    }

    
    public void ClearTableList()
    {
        allTableColliders.Clear();
    }


    //switch the layers of all the tables 
    public void ToggleTableDetection(bool allowDetection)
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
    public void ToggleTableDetection(bool allowDetection, GameObject table, string _layerName = "Table")
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
