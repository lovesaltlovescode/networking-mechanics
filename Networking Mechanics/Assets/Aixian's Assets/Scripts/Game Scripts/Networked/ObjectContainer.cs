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
    public HeldItem objToSpawn;

    //PREFABS to be dropped
    [Header("Ingredients")]
    public GameObject cucumberPrefab;
    public GameObject eggPrefab;
    public GameObject chickenPrefab;
    public GameObject ricePrefab;
    public GameObject rottenPrefab;
    public GameObject drinkPrefab;

    [Header("Plates")]
    public GameObject dirtyPlatePrefab;
    public GameObject cleanPlatePrefab;

    [Header("Customer")]
    public GameObject queueingCustomerPrefab;

    [Header("Dishes")]
    public GameObject roastedChicWRiceBall;
    public GameObject roastedChicWPlainRice;
    public GameObject roastedChicWRiceBallEgg;
    public GameObject roastedChicWPlainRiceEgg;
    public GameObject steamedChicWRiceBall;
    public GameObject steamedChicWPlainRice;
    public GameObject steamedChicWRiceBallEgg;
    public GameObject steamedChicWPlainRiceEgg;

    private DirtyDishScript dirtyDishScript;

    private void Awake()
    {
        dirtyDishScript = GetComponent<DirtyDishScript>();
        dirtyDishScript.enabled = false;

    }
    void OnChangeIngredient(HeldItem oldItem, HeldItem newItem)
    {
        //Debug.Log("NetworkedIngredientInteraction - Starting coroutine!");
        StartCoroutine(ChangeItem(newItem));
    }

    //destroy is delayed to end of current frame, so we use a coroutine
    //clear any child object before instantiating the new one
    IEnumerator ChangeItem(HeldItem newItem)
    {
        //if this object has a child, destroy them
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            yield return null;
        }

        //use the new value
        SetObjToSpawn(newItem);
    }

    //called on client from onchangeingredient method
    //and on the server from cmddropitem in networkedingredientinteraction script
    public void SetObjToSpawn(HeldItem objToSpawn)
    {
        //instantiates the right prefab as a child of this object
        switch (objToSpawn)
        {
           //INGREDIENTS
            case HeldItem.chicken:
                Instantiate(chickenPrefab, transform);
                break;
            case HeldItem.egg:
                Instantiate(eggPrefab, transform);
                break;
            case HeldItem.cucumber:
                Instantiate(cucumberPrefab, transform);
                break;
            case HeldItem.rice:
                Instantiate(ricePrefab, transform);
                break;
            case HeldItem.dirtyplate:
                Instantiate(dirtyPlatePrefab, transform);
                break;
            case HeldItem.cleanplate:
                Instantiate(cleanPlatePrefab, transform);
                break;
            case HeldItem.rotten:
                Instantiate(rottenPrefab, transform);
                break;
            case HeldItem.drink:
                Instantiate(drinkPrefab, transform);
                break;


            case HeldItem.customer:
                Instantiate(queueingCustomerPrefab, transform);
                break;

            //DISHES
            case HeldItem.roastedChicWRiceBall:
                Instantiate(roastedChicWRiceBall, transform);
                break;

            case HeldItem.roastedChicWPlainRice:
                Instantiate(roastedChicWPlainRice, transform);
                break;

            case HeldItem.roastedChicWRiceBallEgg:
                Instantiate(roastedChicWRiceBallEgg, transform);
                break;

            case HeldItem.roastedChicWPlainRiceEgg:
                Instantiate(roastedChicWPlainRiceEgg, transform);
                break;

            case HeldItem.steamedChicWRiceBall:
                Instantiate(steamedChicWRiceBall, transform);
                break;

            case HeldItem.steamedChicWPlainRice:
                Instantiate(steamedChicWPlainRice, transform);
                break;

            case HeldItem.steamedChicWRiceBallEgg:
                Instantiate(steamedChicWRiceBallEgg, transform);
                break;

            case HeldItem.steamedChicWPlainRiceEgg:
                Instantiate(steamedChicWPlainRiceEgg, transform);
                break;
        }

        //change game object tag to fit the child tag, so players know what they are picking up
        gameObject.tag = gameObject.transform.GetChild(0).tag;

        if(gameObject.tag == "DirtyPlate")
        {
            dirtyDishScript.enabled = true;
        }
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
