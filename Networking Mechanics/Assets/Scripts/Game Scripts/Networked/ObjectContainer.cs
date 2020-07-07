using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// A container to contain dropped ingredients
/// ingredients cannot be networked so they have to be nested within this object
/// </summary>

public class ObjectContainer : NetworkBehaviour
{
    //sync held item and call onchangeingredinet method
    [SyncVar(hook = nameof(OnChangeIngredient))]
    public HeldIngredient heldIngredient;

    //PREFABS to be dropped
    public GameObject cucumberPrefab;
    public GameObject eggPrefab;
    public GameObject chickenPrefab;

    void OnChangeIngredient(HeldIngredient oldIngredient, HeldIngredient newIngredient)
    {
        //Debug.Log("NetworkedIngredientInteraction - Starting coroutine!");
        StartCoroutine(ChangeIngredient(newIngredient));
    }

    //destroy is delayed to end of current frame, so we use a coroutine
    //clear any child object before instantiating the new one
    IEnumerator ChangeIngredient(HeldIngredient newIngredient)
    {
        //if this object has a child, destroy them
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            yield return null;
        }

        //use the new value
        SetHeldIngredient(newIngredient);
    }

    //called on client from onchangeingredient method
    //and on the server from cmddropitem in networkedingredientinteraction script
    public void SetHeldIngredient(HeldIngredient heldIngredient)
    {
        //instantiates the right prefab as a child of this object
        switch (heldIngredient)
        {
           
            case HeldIngredient.chicken:
                Instantiate(chickenPrefab, transform);
                break;
            case HeldIngredient.egg:
                Instantiate(eggPrefab, transform);
                break;
            case HeldIngredient.cucumber:
                Instantiate(cucumberPrefab, transform);
                break;
        }

        //change game object tag to fit the child tag, so players know what they are picking up
        gameObject.tag = gameObject.transform.GetChild(0).tag;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
