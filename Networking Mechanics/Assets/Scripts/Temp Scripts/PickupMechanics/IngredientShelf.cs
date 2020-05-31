using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//attached to ingredient shelf
//spawns ingredient prefab as ingredient object
//when players stand in front of shelf and press the button, it will spawn an ingredient prefab

public class IngredientShelf : MonoBehaviour
{
    public GameObject ingredientPrefab;

    public GameObject ingredientObject;

    //the object that the player will be holding if they pick up the object
    //ie. a cup of rice on their head
    //set active/inactive depending on if the player is holding the object
    public GameObject heldObject;

    public GameObject playerPrefab;

    public Image ingredientIcon; //icon to be set active when spawn ingredient



    public void SpawnIngredient()
    {
        

        Vector3 playerTransform = new Vector3(playerPrefab.transform.position.x, 0.3f, playerPrefab.transform.position.z);

        ingredientObject = Instantiate(ingredientPrefab, playerTransform, Quaternion.identity);

        ingredientObject.transform.parent = playerPrefab.transform;

        PickUppable.objectsInInventory.Add(ingredientObject);
        Debug.Log(PickUppable.objectsInInventory);

        heldObject.GetComponentInChildren<Renderer>().enabled = true;

        ingredientIcon.gameObject.SetActive(true);
        

        Debug.Log("Ingredient Shelf: Spawned an ingredient");
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        ingredientPrefab.GetComponentInChildren<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
