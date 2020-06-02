using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if the dirty plate prefab is a child of anything
/// if it is, then show the stacks, if it isn't then show the individual plates
/// </summary>
public class CheckDirtyPlate : MonoBehaviour
{
    public GameObject dirtyPlates;
    public GameObject stackedDirtyPlates;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.parent == null)
        {
            //if not child
            dirtyPlates.SetActive(true);
            stackedDirtyPlates.SetActive(false);
        }
        else
        {
            stackedDirtyPlates.SetActive(true);
            dirtyPlates.SetActive(false);
        }
    }
}
