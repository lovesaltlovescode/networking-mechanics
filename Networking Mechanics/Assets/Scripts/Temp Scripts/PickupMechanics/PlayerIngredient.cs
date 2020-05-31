using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Checks for collisions/trigger enters and calls functions
//Houses functions for interact button and checks for the state of the picked up ingredient
//For ingredient shelves only

public class PlayerIngredient : MonoBehaviour
{

    //reference ingredient script
    [SerializeField] private IngredientShelf playerIngredientShelf;

    //Use this reference if static variable cannot be used
    public PlayerRadar playerRadar;

    #region Ingredient Shelf and Ingredients Booleans

    [SerializeField] bool canSpawnIngredient = true;

    [SerializeField] bool canDropIngredient = false;

    [SerializeField] bool canPlaceIngredientOnTable = false;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("PlayerIngredient: is running");
    }
}
