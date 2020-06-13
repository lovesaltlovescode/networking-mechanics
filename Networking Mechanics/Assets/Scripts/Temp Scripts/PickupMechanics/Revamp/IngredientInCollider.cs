using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if there is an ingredient in the current collider
/// if there is an object, and the tag is ingredient, add it to a list
/// when the object exits, remove it from the list
/// set a bool to true/debug log when ever items enter or exit
/// if there is an ingredient in the collider, no other ingredient can be placed
/// </summary>
public class IngredientInCollider : MonoBehaviour
{

    public List<GameObject> IngredientsInCollider = new List<GameObject>();

    public bool ingredientPresent;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("IngredientInCollider: Script Initialised");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if its an ingredient
        if (other.gameObject.tag == "Rice")
        {
            IngredientsInCollider.Add(other.gameObject);
            Debug.Log("IngredientInCollider: Ingredient is in collider! Ingredient is " + other.gameObject.tag);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Rice")
        {
            IngredientsInCollider.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if its an ingredient
        if(other.gameObject.tag == "Rice")
        {
         //   IngredientsInCollider.Remove(other.gameObject);
            Debug.Log("IngredientInCollider: Ingredient has left the collider! Ingredient was " + other.gameObject.tag);
        }
    }
}
