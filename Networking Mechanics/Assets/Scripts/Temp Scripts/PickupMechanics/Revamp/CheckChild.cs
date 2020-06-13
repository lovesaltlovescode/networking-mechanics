using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if gameobject has child
/// </summary>
public class CheckChild : MonoBehaviour
{

    public bool hasChild; //checks if object has child

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount > 0)
        {
            Debug.Log($"{gameObject.name} has a child!");
            hasChild = true;
        }
        else
        {
            hasChild = false;
            Debug.Log($"{gameObject.name} has no children");
        }   
    }
}
