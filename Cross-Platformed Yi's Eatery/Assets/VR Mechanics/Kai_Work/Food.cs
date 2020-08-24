using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Food : MonoBehaviour, IChoppable
{
    public GameObject choppedFoodPrefab;
    public int chopInteger;

    public XRInteractionManager interactionManager;

    VR_IngredientInteraction ingredient;

    //cutting values
    private bool enteredChop;
    private bool exitedChop;
    private int chopCounter;

    void Awake()
    {
        //chopInteger = 2; //setting chop limit
        chopCounter = 0;

        ingredient = GetComponent<VR_IngredientInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Knife" && ingredient.ingredientOnBoard)
        {
            enteredChop = true;
            Debug.Log("successfully entered chop");
        }
        else
        {
            enteredChop = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Knife" && enteredChop && ingredient.ingredientOnBoard)
        {
            exitedChop = true;
            UpdateChopProgress();
        }
        else
        {
            exitedChop = false;
        }
    }

    public void SpawnChoppedObject()
    {
        Instantiate(choppedFoodPrefab, transform.position, Quaternion.identity);
    }

    public void UpdateChopProgress()
    {
        if (exitedChop)
        {
            chopCounter++;
            Debug.Log("successfully chopped once");

            if (chopCounter == chopInteger)
            {
                Debug.Log("chopped hit integer");
                //Spawn the chopped object
                SpawnChoppedObject();
                //Destroy old game object, replacing it with this new one.
                Destroy(gameObject);
                Debug.Log("Chop successfully");

                //VFX, UI changes will be made here too
            }
        }
    }
}
